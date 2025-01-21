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
	//public static bool HasEntity(this GameObject go, out EntityOld entity)
	//{
	//	var entityBehaviour = go.GetComponentInParent<EntityBehaviourOld>();
	//	if (entityBehaviour)
	//	{
	//		entity = entityBehaviour.Entity;
	//		return true;
	//	}
	//	entity = default;
	//	return false;
	//}
	//public static bool HasEntityOld(this Component component, out EntityOld entity)
	//{
	//	if (component && component.gameObject.HasEntity(out entity))
	//		return true;
	//	entity = default;
	//	return false;
	//}
}
