using UnityEngine;

namespace BB.Di
{
    public sealed class ComponentDiComponent : BaseDiComponent
    {
        public ComponentDiComponent(in DiComponentContext context) : base(context)
        {
        }

        public override object Create(in DiComponentCreateContext context)
        {
            var entity = (UnityEntity)context.Entity;
            var component = entity.Object.GetComponent(InstanceType);
            return component;
        }

        public override bool Validate(IEntityInstaller installer) => true;
    }
    public interface IEntityInstaller3D : IEntityInstaller { }
    public interface IUnityFromPrefabSpawner
    {
        Entity Spawn(in Context3D context);
        Entity Spawn(in Context2D context);
        public readonly struct Context3D
        {
            public IEntityInstaller Installer => Prefab;
            public EntityGameObject3D Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation Operation { get; init; }
        }
        public readonly struct Context2D
        {
            public EntityGameObject2D Prefab { get; init; }
            public Entity? Parent { get; init; }
            public TransformOperation2D Operation { get; init; }
        }
        public readonly struct CommonContext : ISpawnContext
        {
            public IEntityInstaller Installer => Prefab;
            public BaseEntityGameObject Prefab { get; init; }
            public Entity? Parent { get; init; }
        }
    }

    public sealed class UnityFromPrefabSpawner
        : BaseEntitySpawnManager<IUnityFromPrefabSpawner.CommonContext>, IUnityFromPrefabSpawner
    {
        public Entity Spawn(in IUnityFromPrefabSpawner.Context3D context)
        {
            var entity = GetUnspawnedEntity(new()
            {
                Parent = context.Parent,
                Prefab = context.Prefab,
            });

            var root = entity.Require<Root>();
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
            });

            var root = entity.Require<Root2D>();
            context.Operation.Apply(root);

            entity.SetState(EntityState.Enabled);
            return entity.GetToken();
        }

        protected override EntitySpawnData CreateNewData(in IUnityFromPrefabSpawner.CommonContext context)
        {
            var pool = new EntityPool();
            var injector = CreateInjector(context);
            var factory = new EntityFromPrefabFactory(pool, context.Prefab.gameObject);
            return new EntitySpawnData
            {
                Pool = pool,
                Injector = injector,
                Installer = context.Installer,
                Factory = factory
            };
        }
    }

    public sealed record EntityFromPrefabFactory(
        IEntityPool Pool,
        GameObject Prefab
        ) : IEntityFactory
    {
        public IEntity Create(in CreateEntityContext context)
        {
            Prefab.SetActive(false);
            var instance = UnityEngine.Object.Instantiate(Prefab);
            Prefab.SetActive(true);

            var entity = new UnityEntity(context.Name, Pool, instance);
            var root = entity.Require<Root>();
            root.Transform = instance.transform;
            return entity;
        }
    }
    public sealed class UnityEntity : BaseEntity
    {
        public GameObject Object { get; private set; }
        public UnityEntity(
            string name,
            IEntityPool pool,
            GameObject gameObject) : base(name, pool)
        {
            Object = gameObject;
        }
    }
    public abstract class BaseEntityGameObject
        : BaseBehaviour,
        IEntityBehaviour,
        IEntityInstaller
    {
        public string Name => name;

        public Entity Entity { get; set; }

        public abstract void Install(IDiContainer container);
    }
    public sealed class EntityGameObject3D : BaseEntityGameObject
    {
        public override void Install(IDiContainer container)
        {
            container.System<Root>();
        }
    }
    public sealed class EntityGameObject2D : BaseEntityGameObject
    {
        public override void Install(IDiContainer container)
        {
            container.System<Root2D>();
        }
    }
    public abstract class BaseEntityComponent : BaseBehaviour
    {
        public virtual void Install(IDiContainer container)
        {
            container.BindStrategy(new ComponentDiComponent(new()
            {
                ContractType = GetType(),
                InstanceType = GetType(),
            }));
        }
    }
    [RequireComponent(typeof(EntityGameObject3D))]
    public abstract class EntityComponent3D : BaseEntityComponent
    {
    }
    [RequireComponent(typeof(EntityGameObject2D))]
    public abstract class EntityComponent2D : BaseEntityComponent
    {
    }
    public sealed class UnityEntity<TPrefab> : BaseEntity
        where TPrefab : MonoBehaviour, IEntityInstaller
    {
        public readonly TPrefab _instance;
        public UnityEntity(string name, IEntityPool pool, TPrefab instance)
            : base(name, pool)
        {
            _instance = instance;
        }
    }
    public interface IUnityEntityPools3D
    {
        IUnityEntityPool3D GetPool(EntityGameObject3D prefab);
    }
    public interface IUnityEntityPool3D : IEntityPool { }
    //public sealed class UnityEntityFactory<TPrefab> : BaseEntityFactory
    //    where TPrefab : MonoBehaviour, IEntityInstaller
    //{
    //    readonly TPrefab _prefab;
    //    public UnityEntityFactory(IEntityPool pool, TPrefab prefab) : base(pool)
    //    {
    //        _prefab = prefab;
    //    }
    //    protected override BaseEntity CreateEntity(in CreateEntityContext context)
    //    {
    //        _prefab.enabled = false;
    //        var instance = UnityEngine.Object.Instantiate(_prefab);
    //        _prefab.enabled = true;

    //        return new UnityEntity<TPrefab>(context.Name, _pool, instance);
    //    }
    //}
    //public sealed class UnityEntityPool<TPrefab> : IUnityEntityPool3D
    //    where TPrefab : MonoBehaviour, IEntityInstaller
    //{
    //    readonly TPrefab _prefab;
    //    readonly UnityEntityFactory<TPrefab> _entityFactory;
    //    readonly List<TPrefab> _allInstances = new(), _availableInstances = new();
    //    public UnityEntityPool(TPrefab prefab)
    //    {
    //        _prefab = prefab;
    //        _entityFactory = new(this, prefab);
    //    }
    //    public IEntityInstaller Installer => _prefab;

    //    public IEntity GetUnspawnedEntity(Entity? parent = null)
    //    {
    //        $"{_prefab.name} {_allInstances.Count}"
    //    }

    //    public void ReturnEntity(IEntity entity)
    //    {
    //        throw new System.NotImplementedException();
    //    }
    //}
}