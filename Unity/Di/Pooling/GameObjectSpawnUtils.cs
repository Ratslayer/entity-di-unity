using System;
using UnityEngine;
using BB.Di;
namespace BB
{
    public abstract class BasePrefabAsset : BaseScriptableObject
    {
        public GameObject _prefab;
    }
    public abstract class BasePrefabAsset<T> : BaseScriptableObject
        where T : Component
    {
        public T _prefab;
    }
    public abstract class BasePrefabAsset2D : BaseScriptableObject
    {
        public RectTransform _prefab;
    }
    public static class GameObjectSpawnUtils
    {

        public static IEntity CreateDespawnedGameObjectEntity(
            GameObject prefab,
            Entity? parent)
        {
            parent ??= World.Entity;
            if (parent?._ref is null)
                throw new Exception("Trying to create an instance with no parent and a null World");
            if (parent?._ref is not IEntityUnity eu)
                throw new Exception(
                    $"Entity implementation does not support creating gameobject children." +
                    $"Check that you're using the original {nameof(EntityImpl)} class.");

            var entity = eu.SpawnChildGameObjectEntity(prefab, true);
            return entity;
        }
        public static Entity EnableEntity(IEntity entity)
        {
            entity.State = EntityState.Enabled;
            return entity.GetToken();
        }
        public static T GameObjectEntityRequire<T>(in Entity entity)
            where T : class
        {
            T result = null;
            if (typeof(Component).IsAssignableFrom(typeof(T))
                && entity.Require<Root>().Transform.TryGetComponent(typeof(T), out var comp))
                result = comp as T;
            result ??= entity.Require<T>();
            return result;
        }
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
    }
}