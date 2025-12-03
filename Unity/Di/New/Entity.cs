using BB.Di;
using System;
namespace BB
{
    public readonly partial struct Entity
    {
        public static Entity Spawn(in EntitySpawnContext3D context)
        {
            if (context.Installer.Installer)
            {
                var spawner = World.Require<IUnityFromInstallerSpawner>();
                var entity = spawner.Spawn(new IUnityFromInstallerSpawner.Context3D()
                {
                    Installer = context.Installer.Installer,
                    Prefab = context.Installer.Prefab,
                    Parent = context.Parent,
                    Operation = context.Transform ?? default
                });

                return entity;

            }
            else if (context.Installer.PrefabInstaller)
            {

                var spawner = World.Require<IUnityFromPrefabSpawner>();
                var entity = spawner.Spawn(new IUnityFromPrefabSpawner.Context3D()
                {
                    Prefab = context.Installer.PrefabInstaller,
                    Parent = context.Parent,
                    Operation = context.Transform ?? default
                });

                return entity;
            }
            else throw new ArgumentException(
                $"Invalid {typeof(EntitySpawnContext3D).Name}. " +
                $"No Installer or Prefab.");
        }
        public static Entity Spawn(in EntitySpawnContext2D context)
        {
            if (context.Installer.Installer)
            {
                var spawner = World.Require<IUnityFromInstallerSpawner>();
                var entity = spawner.Spawn(new IUnityFromInstallerSpawner.Context2D()
                {
                    Installer = context.Installer.Installer,
                    Prefab = context.Installer.Prefab.gameObject,
                    Parent = context.Parent,
                    Operation = context.Transform ?? default
                });

                return entity;

            }
            else if (context.Installer.PrefabInstaller)
            {

                var spawner = World.Require<IUnityFromPrefabSpawner>();
                var entity = spawner.Spawn(new IUnityFromPrefabSpawner.Context2D()
                {
                    Prefab = context.Installer.PrefabInstaller,
                    Parent = context.Parent,
                    Operation = context.Transform ?? default
                });

                return entity;
            }
            else throw new ArgumentException(
                $"Invalid {typeof(EntitySpawnContext3D).Name}. " +
                $"No Installer or Prefab.");
        }
    }
}
