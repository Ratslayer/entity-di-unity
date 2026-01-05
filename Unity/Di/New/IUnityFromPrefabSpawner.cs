namespace BB.Di
{
	public interface IUnityFromPrefabSpawner
    {
        Entity Spawn(in Context3D context);
        Entity Spawn(in Context2D context);
        public readonly struct Context3D
        {
            public EntityGameObject3D Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation Operation { get; init; }
            public bool DoNotInstantiate { get; init; }
        }
        public readonly struct Context2D
        {
            public EntityGameObject2D Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation2D Operation { get; init; }
            public bool DoNotInstantiate { get; init; }

        }
        public readonly struct CommonContext : ISpawnContext
        {
            public IEntityInstaller Installer => Prefab;
            public BaseEntityInstallerGameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
            public bool DoNotInstantiate { get; init; }
        }
    }
}