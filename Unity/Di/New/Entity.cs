using BB.Di;
using UnityEngine;
namespace BB
{
    public readonly partial struct Entity
    {
        public static GameObject SpawnPrefab3D(in SpawnPrefab3DContext context)
        {
            var spawner = World.Require<IPrefabSpawnManager>();
            var instance = spawner.GetDisabledInstance(context.Prefab);
            context.Transform?.Apply(instance);
            instance.SetActive(true);
            return instance;
        }
        public static GameObject SpawnPrefab2D(in SpawnPrefab2DContext context)
        {
            var spawner = World.Require<IPrefabSpawnManager>();
            var instance = spawner.GetDisabledInstance(context.Prefab.gameObject);
            context.Transform?.Apply(instance);
            instance.SetActive(true);
            return instance;
        }
        public static T SpawnPrefab3D<T>(in SpawnPrefab3DContext<T> context)
            where T : BaseBehaviour
            => SpawnPrefab3D(new()
            {
                Prefab = context.Prefab.gameObject,
                Transform = context.Transform,
            }).GetComponent<T>();
        public static T SpawnPrefab2D<T>(in SpawnPrefab2DContext<T> context)
            where T : BaseBehaviour2D
            => SpawnPrefab2D(new()
            {
                Prefab = context.Prefab.RT,
                Transform = context.Transform,
            }).GetComponent<T>();
        public static Entity Spawn(in SpawnEntityFromInstaller3DContext context)
        {
            var spawner = World.Require<IUnityFromInstallerSpawner>();
            var entity = spawner.Spawn(new IUnityFromInstallerSpawner.Context3D()
            {
                Installer = context.Installer,
                Prefab = context.Prefab,
                Parent = context.Parent,
                Operation = context.Transform ?? default
            });

            RegisterIfSerializable(entity, context.SerializationName);

            return entity;
        }

        public static Entity Spawn(in SpawnEntityFromInstaller2DContext context)
        {
            var spawner = World.Require<IUnityFromInstallerSpawner>();
            var entity = spawner.Spawn(new IUnityFromInstallerSpawner.Context2D()
            {
                Installer = context.Installer,
                Prefab = context.Prefab.gameObject,
                Parent = context.Parent,
                Operation = context.Transform ?? default
            });

            RegisterIfSerializable(entity, context.SerializationName);

            return entity;
        }
        public static Entity Spawn(in SpawnEntityFromPrefab3DContext context)
        {
            var spawner = World.Require<IUnityFromPrefabSpawner>();
            var entity = spawner.Spawn(new IUnityFromPrefabSpawner.Context3D()
            {
                Prefab = context.Prefab.Object,
                Parent = context.Parent,
                Operation = context.Transform ?? default,
                DoNotInstantiate = context.DoNotInstantiate
            });

            RegisterIfSerializable(entity, context.SerializationName);

            return entity;
        }
        public static Entity Spawn(in SpawnEntityFromPrefab2DContext context)
        {
            var spawner = World.Require<IUnityFromPrefabSpawner>();
            var entity = spawner.Spawn(new IUnityFromPrefabSpawner.Context2D()
            {
                Prefab = context.Prefab.Object,
                Parent = context.Parent,
                Operation = context.Transform ?? default,
                DoNotInstantiate = context.DoNotInstantiate
            });

            RegisterIfSerializable(entity, context.SerializationName);

            return entity;
        }

        private static void RegisterIfSerializable(Entity entity, string serializationName)
        {
            if (!string.IsNullOrWhiteSpace(serializationName))
                EntitySerializationUtils.RegisterAsSerializedEntity(entity, serializationName);
        }
    }
}
