using System;
using System.Collections.Generic;
using UnityEngine;
namespace BB.Di
{
	public interface IEntityUnity
	{
		IEntity SpawnChildGameObjectEntity(GameObject prefab, bool usePooling);
	}
	public partial class EntityImpl : IEntityUnity
	{
#if UNITY_EDITOR
		public Guid DebugGuid { get; private set; } = Guid.NewGuid();
#endif
		sealed class UnityPool : IEntityPool
		{
			static GameObject _poolsParent;
			readonly Transform _parent;
			public readonly List<EntityImpl> _entities = new();
			public uint NumCreatedItems = 0;
			public UnityPool(string name)
			{
				if (!_poolsParent)
				{
					_poolsParent = new GameObject("Entity Pools");
					UnityEngine.Object.DontDestroyOnLoad(_poolsParent);
					_poolsParent.transform.ResetData();
				}
				_parent = new GameObject(name).transform;
				_parent.SetParent(_poolsParent.transform);
				_parent.ResetData();
			}
			public void Return(IEntity entity)
			{
				_entities.Add(entity as EntityImpl);
				var root = entity.Require<Root>();
				root.Transform.SetParent(_parent);
				root.Transform.ResetData(false);
			}
			public void Remove(IEntity entity) => _entities.Remove(entity as EntityImpl);
		}
		Dictionary<GameObject, UnityPool> _childUnityPools;
		GameObject _gameObject;
		partial void UnityDestroy()
		{
			_childUnityPools?.Clear();
		}
		partial void UnityDespawn()
		{
			foreach (var ego in _gameObject.GetComponentsInChildren<EntityGameObject>(true))
				if (ego.gameObject != _gameObject)
					ego.Despawn();
		}
		public IEntity SpawnChildGameObjectEntity(GameObject prefab, bool usePooling)
		{
			if (!usePooling)
				return CreateGameObjectEntity(prefab, null, this, null, true);

			_childUnityPools ??= new();
			if (!_childUnityPools.TryGetValue(prefab, out var pool))
			{
				pool = new UnityPool(prefab.name);
				_childUnityPools.Add(prefab, pool);
			}

			if (!pool._entities.TryRemoveLast(out var entity))
			{
				prefab.SetActive(false);
				var instance = UnityEngine.Object.Instantiate(prefab);
				instance.name = $"{prefab.name} {++pool.NumCreatedItems}";
				prefab.SetActive(true);
				entity = CreateGameObjectEntity(instance, null, this, pool, true);
				var ego = entity.Require<EntityGameObject>();

				ego._pool = pool;

				InvokeCreate(ego);
				static void InvokeCreate(EntityGameObject ego)
				{
					((EntityImpl)ego.Entity._ref).PublishCreateEvent();
					foreach (var child in ego._children)
						InvokeCreate(child);
				}
			}
			return entity;
		}
		EntityImpl CreateGameObjectEntity(
			GameObject go,
			EntityGameObject parentEgo,
			EntityImpl parent,
			IEntityPool pool,
			bool isRoot)
		{
			var ego = go.AddComponent<EntityGameObject>();
			if (parentEgo)
				parentEgo._children.Add(ego);
			var entity = CreateEntity(
				go.name,
				parent,
				ego.Install,
				pool,
				isRoot);

			entity._gameObject = go;
			CreateChildEntities(ego.transform, ego, entity);
			return entity;
		}
		void CreateChildEntities(Transform t, EntityGameObject parentEgo, EntityImpl parent)
		{
			foreach (var i in t.childCount)
			{
				var c = t.GetChild(i);
				if (c.GetComponent<EntityBehaviour>())
				{
					var entity = CreateGameObjectEntity(c.gameObject, parentEgo, parent, null, false);
					entity.InitState(EntityState.Enabled);
				}
				else CreateChildEntities(c, parentEgo, parent);
			}
		}
	}
}