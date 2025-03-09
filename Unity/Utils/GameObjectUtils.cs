using System;
using UnityEngine;
namespace BB
{
	public static class GameObjectUtils
	{
		public static GameObject ToGameObject(this UnityEngine.Object obj)
		{
			if (!obj)
				throw new ArgumentNullException($"Attempting to convert null obj to GameObject");
			var type = obj.GetType();
			if (type == typeof(GameObject))
				return obj as GameObject;
			if (typeof(Component).IsAssignableFrom(type))
				return (obj as Component).gameObject;
			throw new ArgumentException($"{type.Name} can not be converted to GameObject");
		}
		public static void Destroy(this GameObject go)
		{
			if (!go)
				return;
			if (Application.isPlaying)
				UnityEngine.Object.Destroy(go);
			else UnityEngine.Object.DestroyImmediate(go);
		}
		public static T GetOrAdd<T>(this GameObject go)
			where T : Component
		{
			var result = go.GetComponent<T>();
			if (!result)
				result = go.AddComponent<T>();
			return result;
		}
		public static void DestroyGameObject(this Component component)
		{
			if (component)
				component.gameObject.Destroy();
		}
		public static void SetStaticRecursive(this GameObject go, bool value)
		{
			if (!go)
				return;
			go.isStatic = value;
			foreach (var child in go.transform.GetChildren())
				SetStaticRecursive(child.gameObject, value);
		}
		public static bool TryGetComponentInChildren<T>(this GameObject go, out T comp)
		{
			comp = go.GetComponentInChildren<T>();
			return comp is not null;
		}
		public static bool TryGetComponentInParent<T>(this GameObject go, out T comp)
		{
			comp = go.GetComponentInParent<T>();
			return comp is not null;
		}
	}
}