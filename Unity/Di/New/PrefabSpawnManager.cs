using System.Collections.Generic;
using UnityEngine;

namespace BB
{
	public sealed class PrefabSpawnManager : IPrefabSpawnManager
    {
        readonly Dictionary<GameObject, PrefabPool> _pools = new();
        public GameObject GetDisabledInstance(GameObject prefab)
        {
            var pool = _pools.GetOrCreate(prefab);
            pool.Prefab = prefab;

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
            public GameObject Prefab { get; set; }
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