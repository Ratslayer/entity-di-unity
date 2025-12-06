using UnityEngine;

namespace BB.Di
{
    public interface IUnityFromInstallerSpawner
    {
        Entity Spawn(in Context3D context);
        Entity Spawn(in Context2D context);
        public readonly struct Context3D : ISpawnContext
        {
            public IEntityInstaller Installer { get; init; }
            public GameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation Operation { get; init; }
        }
        public readonly struct Context2D : ISpawnContext
        {
            public IEntityInstaller Installer { get; init; }
            public GameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation2D Operation { get; init; }
        }
        public readonly struct CommonContext : ISpawnContext
        {
            public IEntityInstaller Installer { get; init; }
            public GameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
        }
    }
    public sealed class UnityFromInstallerSpawner
          : BaseEntitySpawnManager<IUnityFromInstallerSpawner.CommonContext>, IUnityFromInstallerSpawner
    {
        [Inject] IPrefabSpawnManager _prefabSpawnManager;
        public Entity Spawn(in IUnityFromInstallerSpawner.Context3D context)
        {
            var entity = GetUnspawnedEntity(new()
            {
                Installer = context.Installer,
                Parent = context.Parent,
            });
            var root = entity.Require<Root>();

            var instance = _prefabSpawnManager.GetDisabledInstance(context.Prefab);
            root.SetGameObject(instance);

            context.Operation.Apply(root);
            instance.SetActive(true);

            entity.SetState(EntityState.Enabled);
            return entity.GetToken();
        }

        public Entity Spawn(in IUnityFromInstallerSpawner.Context2D context)
        {
            var entity = GetUnspawnedEntity(new()
            {
                Installer = context.Installer,
                Parent = context.Parent,
            });
            var root = entity.Require<Root2D>();

            var instance = _prefabSpawnManager.GetDisabledInstance(context.Prefab);
            root.SetGameObject(instance);

            context.Operation.Apply(root);
            instance.SetActive(true);

            entity.SetState(EntityState.Enabled);
            return entity.GetToken();
        }

        protected override EntitySpawnData CreateNewData(in IUnityFromInstallerSpawner.CommonContext context)
        {
            var pool = new EntityPool();
            var injector = CreateInjector(context);
            var factory = new EntityFactory(pool, injector);
            return new EntitySpawnData
            {
                Pool = pool,
                Injector = injector,
                Installer = context.Installer,
                Factory = factory
            };

        }
    }
}