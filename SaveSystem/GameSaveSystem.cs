using BB.Di;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;

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
        public async UniTask SaveGame(SaveGameContext e)
        {
            var scenes = PooledDictionary<Scene, List<EntitySaveData>>.GetPooled();

            var entities = GetSerializableEntities(Core);
            foreach (var ed in entities)
            {
                var data = GetEntitySaveData(ed.Entity, ed.Path);
                var scene = ed.Entity.Has(out Root root) ? root.Transform.gameObject.scene : default;
                if (!scenes.TryGetValue(scene, out var datas))
                {
                    datas = new List<EntitySaveData>();
                    scenes.Add(scene, datas);
                }
                datas.Add(data);
            }

            await UniTask.NextFrame();

            var sceneDatas = new List<SceneSaveData>();

            foreach (var kvp in scenes)
            {
                var sceneName = kvp.Key.name;
                sceneDatas.Add(new()
                {
                    SceneName = sceneName,
                    EntitySaveDatas = kvp.Value
                });
            }


            var gameSaveData = new GameSaveData
            {
                Version = 1,
                SceneSaveDatas = sceneDatas
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
                        var serializer = comp.GetSerializer();
                        var data = serializer.Serialize(comp);
                        components.Add(new()
                        {
                            SerializerName = serializer.GetType().Name,
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

                var data = GetEntitySaveData(entity, newPath);
                var scene = entity.Has(out Root root) ? root.Transform.gameObject.scene : default;
                if (!scenes.TryGetValue(scene, out var datas))
                {
                    datas = new List<EntitySaveData>();
                    scenes.Add(scene, datas);
                }
                datas.Add(data);

                foreach (var child in entity._ref.Children)
                    AddToSerialization(child.GetToken(), newPath);
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
        public async UniTask LoadGame(LoadGameContext e)
        {
            InitSerializers();
            var path = GetSavePath(e.FilePath);

            var saveData = await _fileSystem.Read<GameSaveData>(new()
            {
                Path = path,
            });
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
                //var sceneIsLoaded = sceneData.SceneName == "DontDestroyOnLoad";
                //if (!sceneIsLoaded)
                //{
                //    var scene = SceneManager.GetSceneByName(sceneData.SceneName);
                //    sceneIsLoaded = scene.isLoaded;
                //}

                //if (!sceneIsLoaded)
                //    continue;

                //foreach (var data in sceneData.EntitySaveDatas)
                //{
                //    //if (!existingEntities.TryGetValue(data.EntityPath, out var entity))
                //    //    return;
                //    //ApplySaveData(entity, data);
                //}
            }
        }
        void ApplySaveData(Entity entity, EntitySaveData saveData)
        {
            if (saveData.SaveDatas.IsNullOrEmpty())
                return;

            var serializers = saveData.SaveDatas
                .Select(data => (GetComponentSerializer(data.SerializerName), data.SerializedData))
                .ToList();

            foreach (var (serializer, data) in serializers)
            {
                serializer.ApplySpawn(new()
                {
                    Entity = entity,
                    SerializedData = data,
                });
            }
            foreach (var (serializer, data) in serializers)
            {
                serializer.ApplyAfterSpawn(new()
                {
                    Entity = entity,
                    SerializedData = data,
                });
            }
        }
        IEntityComponentSerializer GetComponentSerializer(string name)
        {
            if (_serializers.TryGetValue(name, out var componentSerializer))
                return componentSerializer;

            Log.Error($"{name} serializer type not found. " +
                    $"Skipping component.");
            return null;
        }
        void InitSerializers()
        {
            if (_serializers is not null)
                return;

            _serializers = new();
            foreach (var type in GetType().Assembly.GetTypes())
            {
                if (!typeof(IEntityComponentSerializer).IsAssignableFrom(type))
                    continue;

                if (type.IsAbstract)
                    continue;

                if (!type.HasDefaultConstructor())
                {
                    Log.Error($"{type.Name} serializer does not have a default constructor. " +
                        $"Skipping component.");
                    continue;
                }

                var serializer = (IEntityComponentSerializer)Activator.CreateInstance(type);
                _serializers.Add(type.Name, serializer);
            }
        }
        Dictionary<string, IEntityComponentSerializer> _serializers;
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
        public string SerializerName { get; init; }
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
        public object SerializedData { get; init; }
    }
    public interface ISerializableComponent
    {
        IEntityComponentSerializer GetSerializer();
    }

}