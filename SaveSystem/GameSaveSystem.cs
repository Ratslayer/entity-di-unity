using BB.Di;
using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
        public IEntity Entity { get; init; }
        public string SerializedName { get; init; }
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
    }
    public sealed record GameSaveSystem(
        LoadableAssets Assets,
        IFileSystem FileSystem,
        ISerializedEntities SerializedEntities) : IGameSaveSystem
    {
        public async UniTask SaveGame(SaveGameContext e)
        {
            var worldEntity = World.GetWorldEntity();
            var world = GetEntitySaveData(worldEntity);

            var gameEntity = World.GetGameEntity();
            var game = GetEntitySaveData(gameEntity);

            var scenes = PooledDictionary<Scene, List<EntitySaveData>>.GetPooled();

            foreach (var se in SerializedEntities.GetAll())
            {
                var entity = se.Entity.GetToken();
                var path = se.SerializedName;
                var data = GetEntitySaveData(entity, path);
                var scene = entity.Has(out Root root) ? root.Transform.gameObject.scene : default;
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
                World = world,
                Game = game,
                SceneSaveDatas = sceneDatas
            };

            var filePath = GetSavePath(e.FilePath);

            await FileSystem.Write(new()
            {
                Path = filePath,
                Data = gameSaveData
            });


            scenes.DisposeAndClear();
            sceneDatas.DisposeAndClear();
            EntitySaveData GetEntitySaveData(Entity entity, string serializedPath = null)
            {
                var details = (IEntityDetails)entity._ref;
                var factoryName = details.Installer is ILoadableAsset asset
                    ? asset.AssetLoadKey : string.Empty;
                var components = PooledList<EntityComponentSaveData>.GetPooled();
                foreach (var (type, comp) in details.GetElements())
                    if (comp is ISerializableComponent serializedComp)
                    {
                        var serializer = serializedComp.GetSerializer();
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
        }

        public async UniTask LoadGame(LoadGameContext e)
        {
            InitSerializers();
            var path = GetSavePath(e.FilePath);

            var saveData = await FileSystem.Read<GameSaveData>(new()
            {
                Path = path,
            });

            ApplySaveData(World.GetWorldEntity(), saveData.World);
            ApplySaveData(World.GetGameEntity(), saveData.Game);
            foreach (var sceneData in saveData.SceneSaveDatas)
            {
                var sceneIsLoaded = sceneData.SceneName == "DontDestroyOnLoad";
                if (!sceneIsLoaded)
                {
                    var scene = SceneManager.GetSceneByName(sceneData.SceneName);
                    sceneIsLoaded = scene.isLoaded;
                }

                if (!sceneIsLoaded)
                    continue;

                foreach (var data in sceneData.EntitySaveDatas)
                {
                    if (!SerializedEntities.Has(data.EntityPath, out var entity))
                        return;
                    ApplySaveData(entity, data);
                }
            }
        }
        void ApplySaveData(Entity entity, EntitySaveData saveData)
        {
            if (saveData.SaveDatas.IsNullOrEmpty())
                return;

            foreach (var compData in saveData.SaveDatas)
            {
                var serializer = GetComponentSerializer(compData.SerializerName);
                if(serializer is null)
                    continue;

                serializer.Apply(entity, compData.SerializedData);
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
            foreach(var type in GetType().Assembly.GetTypes())
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
        string GetSavePath(string path) => $"Saves/{path}";
        readonly struct TempEntitySaveData
        {
            public Scene Scene { get; init; }
            public IEntity Entity { get; init; }
        }
    }
    public sealed class GameSaveData
    {
        public int Version { get; init; }
        public EntitySaveData World { get; init; }
        public EntitySaveData Game { get; init; }
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
        void Apply(Entity entity, object serializedData);
    }
    public interface ISerializableComponent
    {
        IEntityComponentSerializer GetSerializer();
    }

}