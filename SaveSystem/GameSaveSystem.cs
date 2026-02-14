using BB.Di;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using BB.Ui;

namespace BB
{
    public readonly struct WriteFileContext
    {
        public string Path { get; init; }
        public object Data { get; init; }
    }
    public readonly struct ReadFileContext
    {
        public string Path { get; init; }
    }
    public readonly struct SerializableEntity
    {
        public Entity Entity { get; init; }
        public string Path { get; init; }
        public Scene Scene { get; init; }
    }
    public readonly struct SaveGameContext
    {
        public string FilePath { get; init; }
    }
    public readonly struct LoadGameContext
    {
        public string FilePath { get; init; }
    }
    public readonly struct LoadGameData
    {
        public ISerializableComponent Component { get; init; }
        public IEntityComponentSerializer Serializer { get; init; }
        public object SerializedData { get; init; }
    }
    public interface IGameSaveSystem
    {
        UniTask SaveGame(SaveGameContext e);
        UniTask LoadGame(LoadGameContext e);
        string GetFullPath(string fileName);
    }
    public sealed class GameSaveSystem : EntitySystem, IGameSaveSystem
    {
        [Inject] ILoadableAssets _assets;
        [Inject] IFileSystem _fileSystem;
        [Inject] ISerializedEntities _serializedEntities;
        [Inject] ISceneManager _sceneManager;
        [Inject] IGameManager _gameManager;
        [Inject] IEvent<BeforeGameLoadEvent> _beforeGameLoad;
        [Inject] IEvent<AfterGameLoadEvent> _afterGameLoad;

        public async UniTask SaveGame(SaveGameContext e)
        {
            if (Game._ref is not IEntityDetails gameDetails
                || !_assets.HasAssetKey(gameDetails.Installer, out var gameInstallerKey))
            {
                LogError($"Installer for Game entity is not registered as a loadable asset. Can't save game.");
                return;
            }

            var noSceneDatas = new List<EntitySaveData>();

            var openScenes = _sceneManager.GetAllOpenScenes();
            var scenes = PooledDictionary<GameScene, List<EntitySaveData>>.GetPooled();
            foreach (var openScene in openScenes)
                scenes.Add(openScene, new());

            var entities = GetSerializableEntities(Core);
            foreach (var ed in entities)
            {
                var datas = GetDataList(ed.Entity);
                var data = GetEntitySaveData(ed.Entity, ed.Path);
                datas.Add(data);
            }

            await UniTask.NextFrame();

            var sceneDatas = new List<SceneSaveData>
            {
                new()
                {
                    EntitySaveDatas = noSceneDatas
                }
            };

            foreach (var kvp in scenes)
            {
                var sceneName = kvp.Key;
                sceneDatas.Add(new()
                {
                    SceneName = sceneName.SceneName,
                    EntitySaveDatas = kvp.Value
                });
            }

            var gameSaveData = new GameSaveData
            {
                Version = 1,
                SceneSaveDatas = sceneDatas,
                GameInstaller = gameInstallerKey
            };

            var filePath = GetSavePath(e.FilePath);

            await _fileSystem.Write(new()
            {
                Path = filePath,
                Data = gameSaveData
            });

            scenes.DisposeAndClear();
            sceneDatas.DisposeElementsAndClear();
            EntitySaveData GetEntitySaveData(Entity entity, string serializedPath = null)
            {
                var details = (IEntityDetails)entity._ref;
                var factoryName = details.Installer is ILoadableAsset asset
                    ? asset.AssetLoadKey : string.Empty;
                var components = PooledList<EntityComponentSaveData>.GetPooled();
                var elements = details.GetElements();
                foreach (var element in elements)
                    if (element.Instance is ISerializableComponent comp)
                    {
                        var serializers = comp.GetSerializers();
                        var serializer = serializers.Last();
                        var data = serializer.Serialize(comp);
                        components.Add(new()
                        {
                            ComponentName = comp.GetType().Name,
                            SerializerIndex = serializers.Length - 1,
                            SerializedData = data
                        });
                    }

                return new()
                {
                    EntityPath = serializedPath,
                    FactoryName = factoryName,
                    SaveDatas = components,
                };
            }
            void AddToSerialization(Entity entity, string path)
            {
                if (!entity)
                    return;
                if (string.IsNullOrWhiteSpace(entity._ref.SerializationName))
                    return;

                var newPath = Path.Join(path, entity._ref.SerializationName);

                var datas = GetDataList(entity);
                var data = GetEntitySaveData(entity, newPath);
                datas.Add(data);

                foreach (var child in entity._ref.Children)
                    AddToSerialization(child.GetToken(), newPath);
            }
            List<EntitySaveData> GetDataList(Entity entity)
            {
                List<EntitySaveData> datas;

                var scene = _sceneManager.GetSceneData(entity);
                if (scene is not { } s || s.DoNotDestroyOnLoad)
                    datas = noSceneDatas;
                else if (!scenes.TryGetValue(s.Scene, out datas))
                {
                    datas = new List<EntitySaveData>();
                    scenes.Add(s.Scene, datas);
                }

                return datas;
            }
        }
        public async UniTask LoadGame(LoadGameContext e)
        {
            var path = GetSavePath(e.FilePath);

            var saveData = await _fileSystem.Read<GameSaveData>(new()
            {
                Path = path,
            });

            if (!_assets.HasAsset(saveData.GameInstaller, out BaseGameInstallerAsset gameInstaller))
            {
                LogError($"No Game Installer found with key {saveData.GameInstaller}. Abandoning load game.");
                return;
            }

            var scenes = new List<GameScene>();

            foreach (var sceneSaveData in saveData.SceneSaveDatas)
            {
                if (string.IsNullOrEmpty(sceneSaveData.SceneName))
                    continue;

                var sceneData = _sceneManager.GetSceneData(sceneSaveData.SceneName);
                if (sceneData?.Scene is not { } sceneAsset)
                {
                    LogError($"No Scene asset found with key {sceneSaveData.SceneName}");
                    continue;
                }

                scenes.Add(sceneAsset);
            }

            if (scenes.Count == 0)
            {
                LogError($"No scenes found that can be loaded. Abandoning load game.");
                return;
            }

            await _gameManager.EndGame(new()
            {
                ClearGameInstaller = true
            });

            await _gameManager.StartGame(new()
            {
                GameInstaller = gameInstaller,
                Scenes = scenes,
                AfterSceneLoad = ApplySaveDataToAll,
                SkipStartGameEvents = true
            });

            void ApplySaveDataToAll()
            {
                _beforeGameLoad.Publish();
                var entities = GetSerializableEntities(Core);
                foreach (var sceneData in saveData.SceneSaveDatas)
                {
                    foreach (var data in sceneData.EntitySaveDatas)
                    {
                        if (!entities.TryGetValue(e => e.Path == data.EntityPath, out var entity))
                        {
                            LogError($"No entity found with path '{data.EntityPath}'");
                            continue;
                        }

                        ApplySaveData(entity.Entity, data);
                    }
                }
                _afterGameLoad.Publish();
            }
        }
        PooledList<SerializableEntity> GetSerializableEntities(Entity root)
        {
            var result = PooledList<SerializableEntity>.GetPooled();
            AddSelfAndChildren(root, null);
            return result;

            void AddSelfAndChildren(Entity entity, string path)
            {
                if (!entity)
                    return;
                if (string.IsNullOrWhiteSpace(entity._ref.SerializationName))
                    return;

                var newPath = string.Join('/', path, entity._ref.SerializationName);
                var scene = entity.Has(out Root root) ? root.Transform.gameObject.scene : default;
                result.Add(new SerializableEntity
                {
                    Entity = entity,
                    Scene = scene,
                    Path = newPath
                });

                foreach (var child in entity._ref.Children)
                    AddSelfAndChildren(child.GetToken(), newPath);
            }

        }
        void ApplySaveData(Entity entity, EntitySaveData saveData)
        {
            if (saveData.SaveDatas.IsNullOrEmpty())
                return;

            if (entity._ref is not IEntityDetails details)
                return;

            var components = new Dictionary<string, object>();
            foreach (var comp in details.GetElements())
            {
                if (comp.Instance is not ISerializableComponent)
                    continue;

                var type = comp.Instance.GetType();
                if (components.ContainsKey(type.Name))
                {
                    LogError($"Entity {entity} contains multiple components with name {type.Name}");
                    continue;
                }
                components.Add(type.Name, comp.Instance);
            }

            var serializableDatas = new List<LoadGameData>();

            foreach (var data in saveData.SaveDatas)
            {
                if (!components.TryGetValue(data.ComponentName, out var comp))
                {
                    LogError($"Could not find a component with name {data.ComponentName}");
                    continue;
                }

                if (comp is not ISerializableComponent serializableComp)
                {
                    LogError($"Component {data.ComponentName} is not serializable");
                    continue;
                }

                var serializers = serializableComp.GetSerializers();
                if (serializers?.Length is not > 0)
                {
                    LogError($"Component {data.ComponentName} does not have any serializers");
                    continue;
                }

                if (serializers.Length <= data.SerializerIndex)
                {
                    LogError($"Component {data.ComponentName} has {serializers.Length} serializers, " +
                        $"while serialized index is {data.SerializerIndex}");
                    continue;
                }

                var serializer = serializers[data.SerializerIndex];
                serializableDatas.Add(new()
                {
                    Component = serializableComp,
                    Serializer = serializer,
                    SerializedData = data.SerializedData
                });
            }

            foreach (var data in serializableDatas)
            {
                data.Serializer.ApplySpawn(new()
                {
                    Entity = entity,
                    Component = data.Component,
                    SerializedData = data.SerializedData
                });
            }

            foreach (var data in serializableDatas)
            {
                data.Serializer.ApplyAfterSpawn(new()
                {
                    Entity = entity,
                    Component = data.Component,
                    SerializedData = data.SerializedData
                });
            }
        }
        //IEntityComponentSerializer GetComponentSerializer(string name)
        //{
        //    if (_serializers.TryGetValue(name, out var componentSerializer))
        //        return componentSerializer;

        //    Log.Error($"{name} serializer type not found. " +
        //            $"Skipping component.");
        //    return null;
        //}
        //void InitSerializers()
        //{
        //    if (_serializers is not null)
        //        return;

        //    _serializers = new();
        //    foreach (var type in GetType().Assembly.GetTypes())
        //    {
        //        if (!typeof(IEntityComponentSerializer).IsAssignableFrom(type))
        //            continue;

        //        if (type.IsAbstract)
        //            continue;

        //        if (!type.HasDefaultConstructor())
        //        {
        //            Log.Error($"{type.Name} serializer does not have a default constructor. " +
        //                $"Skipping component.");
        //            continue;
        //        }

        //        var serializer = (IEntityComponentSerializer)Activator.CreateInstance(type);
        //        _serializers.Add(type.Name, serializer);
        //    }
        //}
        //Dictionary<string, IEntityComponentSerializer> _serializers;
        string GetSavePath(string path) => $"Saves/{path}.txt";

        public string GetFullPath(string fileName)
            => _fileSystem.GetFullPath(GetSavePath(fileName));

        Entity Core => WorldBootstrap.World.Core.Entity.GetToken();
        Entity Game => WorldBootstrap.World.Game.Entity.GetToken();
        readonly struct TempEntitySaveData
        {
            public Scene Scene { get; init; }
            public IEntity Entity { get; init; }
        }
    }

    public sealed class GameSaveData
    {
        public int Version { get; init; }
        public string GameInstaller { get; init; }
        public List<SceneSaveData> SceneSaveDatas { get; init; }
    }
    public sealed class SceneSaveData
    {
        public string SceneName { get; init; }
        public List<EntitySaveData> EntitySaveDatas { get; init; }
    }
    public sealed class EntitySaveData
    {
        public string EntityPath { get; init; }
        public string FactoryName { get; init; }
        public List<EntityComponentSaveData> SaveDatas { get; init; }
    }
    public sealed class EntityComponentSaveData
    {
        public string ComponentName { get; init; }
        public int SerializerIndex { get; init; }
        public object SerializedData { get; init; }
    }
    public interface IEntityComponentSerializer
    {
        object Serialize(object target);
        void ApplySpawn(in DeserializationContext context);
        void ApplyAfterSpawn(in DeserializationContext context);
    }
    public readonly struct DeserializationContext
    {
        public Entity Entity { get; init; }
        public ISerializableComponent Component { get; init; }
        public object SerializedData { get; init; }
    }
    public interface ISerializableComponent
    {
        IEntityComponentSerializer[] GetSerializers();
    }

}