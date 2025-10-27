using BB.Di;
using Sirenix.Serialization;
using System;
using UnityEngine;
namespace BB
{
    public readonly struct InstallerAdapter
    {
        public IEntityInstaller Installer { get; init; }
        public GameObject Prefab { get; init; }
        public static implicit operator InstallerAdapter(InstallerAsset asset)
            => new() { Installer = asset };
        public static implicit operator InstallerAdapter(GameObject prefab)
            => new() { Prefab = prefab };
        public static implicit operator InstallerAdapter(Component prefab)
            => new() { Prefab = prefab.gameObject };
    }
    public static class EntitySerializationUtils
    {
        public static void RegisterAsSerializedEntity(Entity entity, string serializedName)
        {
            var serializedEntities = World.Require<ISerializedEntities>();
            serializedEntities.Add(entity, serializedName);
        }
    }
    public readonly struct EntitySpawnContext
    {
        public InstallerAdapter Installer { get; init; }
        public Entity? Parent { get; init; }
        public TransformOperation? Transform { get; init; }
        public TransformOperation2D? Transform2D { get; init; }
        public string SerializationName { get; init; }
        public bool DoNotDestroyOnLoad { get; init; }

        public Entity Spawn()
        {
            var entity = SpawnEntity();
            if (DoNotDestroyOnLoad && entity.Has(out Root root))
                GameObject.DontDestroyOnLoad(root.Transform);

            if (string.IsNullOrWhiteSpace(SerializationName))
                return entity;

            EntitySerializationUtils.RegisterAsSerializedEntity(entity, SerializationName);

            return entity;
        }
        public T Spawn<T>()
        {
            var entity = Spawn();
            return entity.Get<T>();
        }
        Entity SpawnEntity()
        {
            if (Installer.Prefab)
            {
                if (Transform2D is { } t2d && Installer.Prefab.TryGetComponent(out RectTransform rt))
                    return GameObjectSpawnUtils.SpawnEntity(rt, t2d, Parent);
                return GameObjectSpawnUtils.SpawnEntity(Installer.Prefab, Transform ?? default, Parent);
            }
            if (Installer.Installer is not null)
                return Installer.Installer.Spawn(Parent);

            throw new ArgumentException($"Could not create an entity with this context.");
        }
    }
}