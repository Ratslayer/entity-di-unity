using System;
using UnityEngine;
using BB.Di;
namespace BB
{
	public static class GameObjectSpawnUtils
	{
		public static Entity SpawnEntity(
			this GameObject prefab,
			TransformOperation args,
			Entity parent = default)
		{
			if (!parent)
				parent = World.Entity;
			if (parent._ref is not IEntityUnity eu)
				throw new Exception($"Entity implementation does not support creating gameobject children." +
					$"Check that you're using the original {nameof(EntityImpl)} class.");
			var entity = eu.CreateChild(prefab, true);
			args.Apply(entity.Require<Root>());
			entity.State = EntityState.Enabled;
			return entity.GetToken();
		}
		public static GameObject SpawnInstance(this GameObject prefab, TransformOperation args, Entity parent = default)
			=> prefab.SpawnEntity(args, parent).Require<Root>().Transform.gameObject;
		public static Entity SpawnEntity(this Component comp, TransformOperation args, Entity parent = default)
			=> SpawnEntity(comp.gameObject, args, parent);

		public static T SpawnInstance<T>(this T prefab, TransformOperation args, Entity entity = default)
			where T : Component
			=> prefab.gameObject.SpawnInstance(args, entity).GetComponent<T>();
		public static void Despawn(this GameObject go)
		{
			if (!go)
				return;
			if (go.TryGetComponent(out IDespawnable despawnable))
				despawnable.Despawn();
			else go.Destroy();
		}
		public static void Despawn(this Component comp)
		{
			if (!comp)
				return;
			comp.gameObject.Despawn();
		}
		public static void DespawnChildren(this GameObject go)
		{
			if (!go)
				return;
			foreach (var i in -go.transform.childCount)
			{
				var c = go.transform.GetChild(i);
				foreach (var eb in c.GetComponentsInChildren<EntityGameObject>())
					eb.Despawn();
			}
			go.transform.DestroyAllChildren();
		}
		public static void DespawnChildren(this Component comp)
		{
			if (comp)
				comp.gameObject.DespawnChildren();
		}
		public static void SpawnChildren<T>(
			this Transform parent,
			T prefab,
			int numChildren,
			Action<T, int> init,
			Entity parentEntity = default)
			where T : Component
		{
			parent.SpawnChildren(prefab.gameObject, numChildren, Init, parentEntity);
			void Init(GameObject go, int index)
			{
				var comp = go.GetComponent<T>();
				init(comp, index);
			}
		}
		public static void SpawnChildren(
			this Transform parent,
			GameObject prefab,
			int numChildren,
			Action<GameObject, int> init,
			Entity parentEntity = default)
		{
			//set num children
			var c = parent.childCount;
			if (numChildren > c)
			{
				for (var i = c; i < numChildren; i++)
					prefab.SpawnInstance(new(parent), parentEntity);
			}
			else if (numChildren < c)
			{
				for (var i = c - 1; i >= numChildren; i--)
					parent.GetChild(i).Despawn();
			}
			//init children
			foreach (var i in numChildren)
				init(parent.GetChild(i).gameObject, i);
		}
	}
}