using System.Collections.Generic;
using UnityEngine;

namespace BB.Di
{
    public abstract class BaseEntityGameObject : BaseBehaviour
    {
        protected IEntity _entityRef;
        public Entity Entity => _entityRef?.GetToken() ?? default;
        public virtual void Init(IEntity entity)
        {
            _entityRef = entity;
        }
        public abstract void Despawn();
    }
    public sealed class PooledGameObject : BaseEntityGameObject
    {
        public IPrefabPool _pool;
        public override void Despawn()
        {
            _entityRef = null;
            gameObject.SetActive(false);
            _pool.ReturnInstance(this);
        }
    }
    public interface IPrefabPool
    {
        PooledGameObject GetDisabledInstance();
        void ReturnInstance(PooledGameObject instance);
    }
    public interface IPrefabSpawnManager
    {
        GameObject GetDisabledInstance(GameObject prefab);
    }
    public sealed class PrefabSpawnManager : IPrefabSpawnManager
    {
        readonly Dictionary<GameObject, PrefabPool> _pools = new();
        public GameObject GetDisabledInstance(GameObject prefab)
        {
            var pool = _pools.GetOrCreate(prefab);
            var instance = pool.GetDisabledInstance();
            if (!instance)
            {
                var instanceGameObject = Object.Instantiate(prefab);
                instanceGameObject.name = $"{prefab.name} {pool.GetNextId}";
                instance = instanceGameObject.AddComponent<PooledGameObject>();
                instance._pool = pool;
            }
            return instance.gameObject;
        }
        sealed class PrefabPool : IPrefabPool
        {
            readonly List<PooledGameObject> _disabledInstances = new();
            ulong _instanceCount;
            public ulong GetNextId => ++_instanceCount;
            public PooledGameObject GetDisabledInstance()
                => _disabledInstances.RemoveLastOrDefault();

            public void ReturnInstance(PooledGameObject instance)
                => _disabledInstances.Add(instance);
        }

    }
}