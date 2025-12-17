using System;
using System.Reflection;
using UnityEngine;
namespace BB.Di
{
    public sealed record EntityFactoryFromPrefab(
        IEntityPool Pool,
        IEntityInjector Injector,
        IEntityInstaller Installer,
        GameObject Prefab,
        bool DoNotInstantiate)
        : EntityFactory(Pool, Injector, Installer)
    {
        public override IEntity Create(in CreateEntityContext context)
        {
            var entity = base.Create(context);
            GameObject instance;
            if (DoNotInstantiate)
                instance = Prefab;
            else
            {
                Prefab.SetActive(false);
                instance = UnityEngine.Object.Instantiate(Prefab);
                Prefab.SetActive(true);
            }

            var gameObjectWrapper = ((IFullEntity)entity).GetComponentData(new()
            {
                ContractType = typeof(GameObjectWrapper),
                RequestingType = GetType(),
                Init = true
            });
            ((GameObjectWrapper)gameObjectWrapper.Instance).GameObject = instance;

            if (instance.TryGetComponent(out BaseEntityGameObject gameObject))
                gameObject.Init(entity);

            return entity;
        }
    }
    public sealed class GameObjectWrapper
    {
        public GameObject GameObject { get; set; }
    }
    public sealed class ComponentDiComponent : BaseDiComponent
    {
        public ComponentDiComponent(in DiComponentContext context) : base(context)
        {
        }

        public override object Create(IEntity entity)
        {
            var goWrapper = entity.Require<GameObjectWrapper>();
            var component = goWrapper.GameObject.GetComponent(ContractType);
            return component;
        }

        public override bool Validate(IEntityInstaller installer) => true;
    }
    public interface IEntityInstaller3D : IEntityInstaller { }
    public interface IEntityInstaller2D : IEntityInstaller { }
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
    public abstract class BaseEntityInstallerGameObject
        : BaseEntityGameObject,
        IEntityBehaviour,
        IEntityInstaller
    {
        public string Name => name;
        public virtual void Install(IDiContainer container)
        {
            container.System<GameObjectWrapper>();
        }
        public override void Init(IEntity entity)
        {
            base.Init(entity);
            foreach (var comp in GetComponents<BaseEntityComponent>())
                comp.Init();
        }
        public override void Despawn() => _entityRef.SetState(EntityState.Despawned);
        private void Awake()
        {
            if (_entityRef is not null)
                return;
            _entityRef = SpawnEntity();
        }
        protected abstract IEntity SpawnEntity();
    }
    public sealed class EntityGameObject3D : BaseEntityInstallerGameObject
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<Root>();
            foreach (var comp in GetComponents<EntityBehaviour>())
                comp.Install(container);
        }
        protected override IEntity SpawnEntity()
            => Entity.Spawn(new SpawnEntityFromPrefab3DContext
            {
                Prefab = this,
                DoNotInstantiate = true
            })._ref;
    }
    public static class UnityDiExtensions
    {
        public static void Component<T>(this IDiContainer container)
            => container.Component(typeof(T));
        public static void Component(this IDiContainer container, Type type)
        {
            container.AddComponent(new ComponentDiComponent(new()
            {
                ContractType = type,
                InstanceType = type,
            }));
        }
    }
    public abstract class BaseEntityComponent : BaseBehaviour, IEntityProvider, IEntityBehaviour
    {
        public virtual void Install(IDiContainer container)
        {
            container.Component(GetType());
        }
        bool _getAttributeRead = false;
        [Get]
        BaseEntityGameObject _gameObject;
        public Entity Entity => _gameObject.Entity;
        public void Init()
        {
            if (_getAttributeRead)
                return;
            _getAttributeRead = true;
            var members = ReflectionUtils.GetAllMembersWithAttribute<GetAttribute>(GetType());
            foreach (var info in members)
                switch (info)
                {
                    case PropertyInfo prop:
                        if (prop.CanWrite)
                            prop.SetValue(this, GetComponent(prop.PropertyType));
                        break;
                    case FieldInfo field:
                        field.SetValue(this, GetComponent(field.FieldType));
                        break;
                }
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class GetAttribute : Attribute { }

    [RequireComponent(typeof(EntityGameObject3D))]
    public abstract class EntityBehaviour : BaseEntityComponent
    {
    }
    [RequireComponent(typeof(EntityGameObject2D))]
    public abstract class EntityBehaviour2D : BaseEntityComponent
    {
        [Get]
        public RectTransform Rt { get; private set; }
    }
    //public sealed record UnityEntity<TPrefab>(
    //    string Name,
    //    IEntityPool Pool,
    //    IEntityInstaller Installer,
    //    TPrefab Instance) : BaseEntity(Name, Pool, Installer)
    //    where TPrefab : MonoBehaviour, IEntityInstaller;
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