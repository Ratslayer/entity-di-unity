//using System;
//using System.Collections.Generic;
//using UnityEngine;

//public sealed record GameObjectEntityPoolSystem
//	: EntitySystem, IGameObjectEntityPool, IDisposable
//{
//	public sealed class Pool
//	{
//		public Transform _parent;
//		public List<PooledObject> _objects = new();
//	}
//	readonly Dictionary<GameObject, Pool> _pools = new();
//	Transform _poolsParent;
//	[OnCreate]
//	void OnCreate()
//	{
//		var parent = new GameObject("Entities");
//		parent.transform.ResetData();
//		UnityEngine.Object.DontDestroyOnLoad(parent);
//		_poolsParent = parent.transform;
//	}
//	public GameObject Spawn(GameObject prefab, TransformArgs args, Entity parentEntity)
//	{
//		if (!prefab)
//			return default;
//#if UNITY_EDITOR
//		if (!Application.isPlaying)
//		{
//			var instance = UnityEngine.Object.Instantiate(prefab);
//			args.Apply(instance.transform);
//			return instance;
//		}
//#endif
//		//get or create pool
//		if (!_pools.TryGetValue(prefab, out var pool))
//		{
//			pool = new();
//			_pools.Add(prefab, pool);
//			var parent = new GameObject($"{prefab.name} {_pools.Count}").transform;
//			parent.SetParent(_poolsParent);
//			parent.ResetData();
//			pool._parent = parent;
//		}
//		//get or create pooled object
//		if (!pool._objects.Contains(out var po, p => !p.Spawned))
//		{
//			prefab.SetActive(false);
//			var instance = UnityEngine.Object.Instantiate(prefab);
//			prefab.SetActive(true);
//			po = instance.AddComponent<PooledObject>();
//			po._pool = pool;
//			pool._objects.Add(po);
//			//install entity game objects
//			po._gameObject = po.gameObject.AddComponent<EntityGameObject>();
//			var ep = parentEntity ? parentEntity : World.Entity;
//			po._gameObject.CreateEntities(ep._ref);
//		}
//		args.Apply(po.transform);
//		po.Spawn();
//		return po.gameObject;
//	}

//	public void Dispose()
//	{
//		foreach (var pool in _pools)
//			foreach (var po in pool.Value._objects)
//				po.GetEntityRef().Dispose();
//		_pools.Clear();
//		_poolsParent.DestroyGameObject();
//		_poolsParent = null;
//	}
//}
