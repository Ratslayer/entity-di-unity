//public sealed class PooledObject : BaseBehaviour, IDespawnable
//{
//	public bool Spawned { get; private set; }
//	public GameObjectEntityPoolSystem.Pool _pool;
//	public EntityGameObject _gameObject;
//	public void Spawn()
//	{
//		Spawned = true;
//		gameObject.SetActive(true);
//		_gameObject.Spawn();
//	}
//	public void Despawn()
//	{
//		Spawned = false;
//		gameObject.SetActive(false);
//		transform.SetParent(_pool._parent);
//		_gameObject.Despawn();
//	}
//	private void OnDestroy()
//	{
//		_pool._objects.Remove(this);
//	}
//}