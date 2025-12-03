using BB.Di;
using System;
using UnityEngine;
namespace BB
{

    public readonly struct EntitySpawnContext3D
    {
        public InstallerAdapter3D Installer { get; init; }
        public TransformOperation? Transform { get; init; }
        public Entity? Parent { get; init; }
        public string SerializationName { get; init; }
        public bool DoNotDestroyOnLoad { get; init; }

        public Entity Spawn()
        {

            if (Installer.PrefabInstaller)
            {

                var entity = GameObjectSpawnUtils.CreateDespawnedGameObjectEntity(
                    Installer.PrefabInstaller,
                    Parent);

                ApplyTransform(entity);

                var token = GameObjectSpawnUtils.EnableEntity(entity);
                if (!string.IsNullOrWhiteSpace(SerializationName))
                    EntitySerializationUtils.RegisterAsSerializedEntity(token, SerializationName);
                return token;
            }
            if (Installer.Installer)
            {
                var entity = new EntitySpawnContext
                {
                    Installer = Installer.Installer,
                    Parent = Parent,
                    SerializationName = SerializationName,
                }.Spawn();

                ApplyTransform(entity._ref);

                return entity;
            }

            throw new ArgumentException($"Could not create an entity with this context.");
        }
        public T Spawn<T>() where T : class
        {
            var entity = Spawn();
            return GameObjectSpawnUtils.GameObjectEntityRequire<T>(entity);
        }

        void ApplyTransform(IEntity entity)
        {
            var root = entity.Require<Root>();

            Transform?.Apply(root);
            if (DoNotDestroyOnLoad)
                GameObject.DontDestroyOnLoad(root.Transform);
        }
    }
}