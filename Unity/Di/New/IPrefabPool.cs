using UnityEngine;

namespace BB
{
    public interface IPrefabPool
    {
        GameObject Prefab { get; }
        PooledGameObject GetDisabledInstance();
        void ReturnInstance(PooledGameObject instance);
    }
}