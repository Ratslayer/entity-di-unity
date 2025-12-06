using UnityEngine;

namespace BB.Di
{
	public sealed class PooledGameObject : BaseBehaviour
    {
        public IPrefabPool _pool;
        public void ReturnToPool()
        {

        }
    }
    public interface IPrefabPool
    {
        GameObject GetDisabledInstance();
        void ReturnInstance(GameObject instance);
    }
    public interface IPrefabSpawnManager
    {
        GameObject GetDisabledInstance(GameObject prefab);
    }
    public sealed class PrefabSpawnManager : IPrefabSpawnManager
    {

    }
}