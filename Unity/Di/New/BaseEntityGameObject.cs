using BB.Di;
using UnityEngine;

namespace BB
{
	public abstract class BaseEntityGameObject : BaseComponent
    {
        protected IEntity _entityRef;
        public Entity Entity => _entityRef?.GetToken() ?? default;
        public abstract GameObject Prefab { get; }
        public virtual void Init(IEntity entity)
        {
            _entityRef = entity;
        }
        public abstract void Despawn();
    }
}