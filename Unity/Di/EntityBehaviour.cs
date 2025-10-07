using System;
using System.Reflection;
using UnityEngine;
namespace BB.Di
{
    public abstract class EntityBehaviour<T> : EntityBehaviour
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<T>();
        }
    }
    [RequireComponent(typeof(RectTransform))]
    public abstract class EntityBehaviour2D : EntityBehaviour
    {
        public RectTransform Rt => GetComponent<RectTransform>();
    }
    public abstract class EntityBehaviour : BaseBehaviour, IEntityInstaller, IEntityBehaviour
    {
        public string Name => name;
        EntityGameObject _entity;
        public Entity Entity => _entity ? _entity.Entity : default;
        public void RaiseEvent<T>(T msg) => Entity.Publish(msg);
        public void RaiseEvent<T>() => Entity.Publish<T>();
        public bool Has<T>(out T elem) => Entity.Has(out elem);
        protected virtual void Awake()
        {
            InitUnspawned();
        }
        bool _getAttributeRead = false;
        void ReadGetAttribute()
        {
            if (_getAttributeRead)
                return;
            _getAttributeRead = true;
            foreach (var info in ReflectionUtils.GetAllMembersWithAttribute<GetAttribute>(GetType()))
                switch (info)
                {
                    case PropertyInfo prop:
                        prop.SetValue(this, GetComponent(prop.PropertyType));
                        break;
                    case FieldInfo field:
                        field.SetValue(this, GetComponent(field.FieldType));
                        break;
                }
        }
        void InitUnspawned()
        {
            if (_entity)
                return;
            ReadGetAttribute();
            if (!TryGetComponent(out _entity))
            {
                var go = EntityGameObjectUtils.GetRootEntityGameObject(this);
                if (World.EntityRef is not IEntityUnity eu)
                    return;
                var entity = eu.SpawnChildGameObjectEntity(go, false);
                entity.State = EntityState.Enabled;
            }
        }
        public virtual void Install(IDiContainer container)
        {
            ReadGetAttribute();
            _entity = gameObject.GetComponent<EntityGameObject>();
            container.Instance(GetType(), this)
                .Inject()
                .BindEvents();
        }

        public static implicit operator Entity(EntityBehaviour b) => b ? b.Entity : default;

        protected void Get<TComponent>(IDiContainer container)
        {
            container.Instance(GetComponent<TComponent>());
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class GetAttribute : Attribute { }
    public static class EntityGameObjectUtils
    {
        public static GameObject GetRootEntityGameObject(EntityBehaviour eb)
        {
            var root = eb.gameObject;
            while (root.transform.parent)
            {
                if (root.transform.parent.TryGetComponentInParent(out EntityBehaviour parentEb)
                    && !parentEb.GetComponent<EntityGameObject>())
                    root = parentEb.gameObject;
                else break;
            }
            return root;
        }
    }
}