namespace BB.Di
{
	public sealed class UnityFromPrefabSpawner
        : BaseEntitySpawnManager<IUnityFromPrefabSpawner.CommonContext>, IUnityFromPrefabSpawner
    {
        public Entity Spawn(in IUnityFromPrefabSpawner.Context3D context)
        {
            var entity = GetUnspawnedEntity(new()
            {
                Parent = context.Parent,
                Prefab = context.Prefab,
                DoNotInstantiate = context.DoNotInstantiate
            });

            var root = entity.Require<Root>();
            root.Init();
            context.Operation.Apply(root);

            entity.SetState(EntityState.Enabled);
            return entity.GetToken();
        }

        public Entity Spawn(in IUnityFromPrefabSpawner.Context2D context)
        {
            var entity = GetUnspawnedEntity(new()
            {
                Parent = context.Parent,
                Prefab = context.Prefab,
                DoNotInstantiate = context.DoNotInstantiate

            });

            var root = entity.Require<Root2D>();
            root.Init();
            context.Operation.Apply(root);

            entity.SetState(EntityState.Enabled);
            return entity.GetToken();
        }
        protected override IEntityFactory CreateFactory(in IUnityFromPrefabSpawner.CommonContext context, IEntityPool pool, IEntityInjector injector)
            => new EntityFactoryFromPrefab(
                pool,
                injector,
                context.Installer,
                context.Prefab.gameObject,
                context.DoNotInstantiate);
    }
}