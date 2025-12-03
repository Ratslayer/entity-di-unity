//namespace BB.Di
//{
//	public interface IUnityFromPrefabSpawner2D
//    {
//        Entity Spawn(in Context context);
//        public readonly struct Context : ISpawnContext
//        {
//            public IEntityInstaller Installer => Prefab;
//            public BaseEntityGameObject Prefab { get; init; }
//            public Entity? Parent { get; init; }
//            public TransformOperation2D Operation { get; init; }
//        }
//    }
//    public sealed class UnityEntitySpawnManager2D
//          : BaseEntitySpawnManager<IUnityFromPrefabSpawner2D.Context>, IUnityFromPrefabSpawner2D
//    {
//        public Entity Spawn(in IUnityFromPrefabSpawner2D.Context context)
//        {
//            var entity = GetUnspawnedEntity(context);

//            var root = entity.Require<Root2D>();
//            context.Operation.Apply(root);

//            entity.SetState(EntityState.Enabled);
//            return entity.GetToken();
//        }

//        protected override EntitySpawnData CreateNewData(in IUnityFromPrefabSpawner2D.Context context)
//        {
//            var pool = new EntityPool();
//            var injector = CreateInjector(context);
//            var factory = new EntityFromPrefabFactory(pool, context.Prefab.gameObject);
//            return new EntitySpawnData
//            {
//                Pool = pool,
//                Injector = injector,
//                Installer = context.Installer,
//                Factory = factory
//            };

//        }
//    }
//}