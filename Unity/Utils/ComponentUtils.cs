using System.Collections.Generic;
using BB;
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

    public static PooledList<T> GetFirstComponentsNoAlloc<T>(this GameObject obj) where T : Component
    {
        var list = PooledList<T>.GetPooled();
        GetComponents(obj);
        return list;

        void GetComponents(GameObject go)
        {
            if (go.TryGetComponent(out T component))
            {
                list.Add(component);
                return;
            }

            foreach (var child in go.transform.GetChildren())
                GetComponents(child.gameObject);
        }
    }

    public static T InstantiateGameObject<T>(this T component) where T : Component
    {
        var copy = Object.Instantiate(component.gameObject);
        copy.transform.SetPositionAndRotation(component.transform.position, component.transform.rotation);
        return copy.GetComponent<T>();
    }
}