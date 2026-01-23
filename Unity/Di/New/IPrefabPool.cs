using UnityEngine;

namespace BB
{
    public interface IPrefabPool
    {
        GameObject Prefab { get; }
        GameObject Parent { get; }
        PooledGameObject GetDisabledInstance();
        void ReturnInstance(PooledGameObject instance);
    }
}