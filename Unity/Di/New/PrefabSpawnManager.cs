using System.Collections.Generic;
using UnityEngine;

namespace BB
{
    public sealed class PrefabSpawnManager : IPrefabSpawnManager
    {
        readonly Dictionary<GameObject, PrefabPool> _pools = new();
        GameObject _poolsParent;
        public GameObject GetDisabledInstance(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                var parent = new GameObject(prefab.name);
                parent.transform.SetParent(_poolsParent.transform);
                parent.transform.ResetData();

                pool = new()
                {
                    Prefab = prefab,
                    Parent = parent
                };
            }

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
        [OnEvent]
        void Init(EntityCreatedEvent _)
        {
            _poolsParent = new GameObject("Prefabs");
            Object.DontDestroyOnLoad(_poolsParent);
        }
        sealed class PrefabPool : IPrefabPool
        {
            public GameObject Prefab { get; init; }
            public GameObject Parent { get; init; }
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