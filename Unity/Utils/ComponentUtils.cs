using UnityEngine;

public static class ComponentUtils
{
	public static bool TryGetComponentInParent<T>(this Component component, out T result)
	{
		result = component.GetComponentInParent<T>();
		return result is not null;
	}
	public static bool TryGetComponentInChildren<T>(this Component component, out T result)
	{
		result = component.GetComponentInChildren<T>();
		return result is not null;
	}
	public static bool IsComponentOrGameObject<T>(this Object obj, out T component)
		where T : Component
	{
		if (obj is T comp)
		{
			component = comp;
			return true;
		}
		if (obj is GameObject go)
			return go.TryGetComponent(out component);

		component = null;
		return false;
	}
}
