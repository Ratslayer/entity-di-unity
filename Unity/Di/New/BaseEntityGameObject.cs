using System;
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

        private void Awake()
        {
            var objects = GetComponents<BaseEntityGameObject>();
            if (objects.Length > 1)
                Debug.LogError("More than 1 EntityGameObject attached to a single GameObject", this);
        }

        public abstract void Despawn();
        public void ClearEntity() => _entityRef = null;
    }
}