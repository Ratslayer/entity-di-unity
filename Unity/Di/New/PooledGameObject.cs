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
            _pool.ReturnInstance(this);
        }
    }
}