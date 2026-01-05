using BB.Di;
using UnityEngine;

namespace BB
{

    public readonly struct SpawnEntityFromInstaller3DContext
    {
        public IEntityInstaller3D Installer { get; init; }
        public GameObject Prefab { get; init; }
        public TransformOperation? Transform { get; init; }
        public Entity? Parent { get; init; }
        public string SerializationName { get; init; }
    }
    public readonly struct SpawnEntityFromInstaller2DContext
    {
        public IEntityInstaller2D Installer { get; init; }
        public RectTransform Prefab { get; init; }
        public TransformOperation2D? Transform { get; init; }
        public Entity? Parent { get; init; }
        public string SerializationName { get; init; }
    }
    public readonly struct SpawnEntityFromPrefab3DContext
    {
        public EntityGameObject3DAdapter Prefab { get; init; }
        public TransformOperation? Transform { get; init; }
        public Entity? Parent { get; init; }
        public string SerializationName { get; init; }
        public bool DoNotInstantiate { get; init; }
    }
    public readonly struct SpawnEntityFromPrefab2DContext
    {
        public EntityGameObject2DAdapter Prefab { get; init; }
        public TransformOperation2D? Transform { get; init; }
        public Entity? Parent { get; init; }
        public string SerializationName { get; init; }
        public bool DoNotInstantiate { get; init; }
    }

    public readonly struct SpawnPrefab3DContext
    {
        public GameObject Prefab { get; init; }
        public TransformOperation? Transform { get; init; }
    }
    public readonly struct SpawnPrefab3DContext<T>
        where T : BaseBehaviour
    {
        public T Prefab { get; init; }
        public TransformOperation? Transform { get; init; }
    }
    public readonly struct SpawnPrefab2DContext
    {
        public RectTransform Prefab { get; init; }
        public TransformOperation2D? Transform { get; init; }
    }
    public readonly struct SpawnPrefab2DContext<T>
        where T : BaseBehaviour2D
    {
        public T Prefab { get; init; }
        public TransformOperation2D? Transform { get; init; }
    }

    public readonly struct EntityGameObject3DAdapter
    {
        public EntityGameObject3D Object { get; init; }
        public static implicit operator EntityGameObject3DAdapter(EntityGameObject3D go)
            => new() { Object = go };
        public static implicit operator EntityGameObject3DAdapter(EntityComponent3D go)
            => new() { Object = go.GetComponent<EntityGameObject3D>() };
    }
    public readonly struct EntityGameObject2DAdapter
    {
        public EntityGameObject2D Object { get; init; }
        public static implicit operator EntityGameObject2DAdapter(EntityGameObject2D go)
            => new() { Object = go };
        public static implicit operator EntityGameObject2DAdapter(EntityComponent2D go)
            => new() { Object = go.GetComponent<EntityGameObject2D>() };
    }
    //public Entity Spawn()
    //{

    //    if (Installer.PrefabInstaller)
    //    {

    //        var entity = GameObjectSpawnUtils.CreateDespawnedGameObjectEntity(
    //            Installer.PrefabInstaller,
    //            Parent);

    //        ApplyTransform(entity);

    //        var token = GameObjectSpawnUtils.EnableEntity(entity);
    //        if (!string.IsNullOrWhiteSpace(SerializationName))
    //            EntitySerializationUtils.RegisterAsSerializedEntity(token, SerializationName);
    //        return token;
    //    }
    //    if (Installer.Installer)
    //    {
    //        var entity = new EntitySpawnContext
    //        {
    //            Installer = Installer.Installer,
    //            Parent = Parent,
    //            SerializationName = SerializationName,
    //        }.Spawn();

    //        ApplyTransform(entity._ref);

    //        return entity;
    //    }

    //    throw new ArgumentException($"Could not create an entity with this context.");
    //}
    //public T Spawn<T>() where T : class
    //{
    //    var entity = Spawn();
    //    return GameObjectSpawnUtils.GameObjectEntityRequire<T>(entity);
    //}

    //void ApplyTransform(IEntity entity)
    //{
    //    var root = entity.Require<Root>();

    //    Transform?.Apply(root);
    //    if (DoNotDestroyOnLoad)
    //        GameObject.DontDestroyOnLoad(root.Transform);
    //}
    //}
}