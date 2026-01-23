using UnityEngine;

namespace BB
{
    public sealed class PooledGameObject : BaseEntityGameObject
    {
        public IPrefabPool _pool;
		public override GameObject Prefab => _pool.Prefab;
        public override void Despawn()
        {
            _entityRef = null;
            gameObject.SetActive(false);
            gameObject.transform.SetParent(_pool.Parent.transform);
            gameObject.transform.ResetData();
            _pool.ReturnInstance(this);
        }
    }
}