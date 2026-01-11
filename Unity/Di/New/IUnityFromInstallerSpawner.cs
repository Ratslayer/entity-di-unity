using UnityEngine;

namespace BB.Di
{
    
    public interface IUnityFromInstallerSpawner
    {
        Entity Spawn(in Context3D context);
        Entity Spawn(in Context2D context);
        public readonly struct Context3D : ISpawnContext
        {
            public IEntityInstaller3D Installer { get; init; }
            public GameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation Operation { get; init; }

            IEntityInstaller ISpawnContext.Installer => Installer;
        }
        public readonly struct Context2D : ISpawnContext
        {
            public IEntityInstaller2D Installer { get; init; }
            public GameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation2D Operation { get; init; }

            IEntityInstaller ISpawnContext.Installer => Installer;
        }
        public readonly struct CommonContext : ISpawnContext
        {
            public IEntityInstaller Installer { get; init; }
            public GameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
        }
    }
}