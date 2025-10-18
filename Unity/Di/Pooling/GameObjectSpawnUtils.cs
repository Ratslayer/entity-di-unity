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
        public static Entity SpawnEntity(
            this GameObject prefab,
            in TransformOperation args,
            Entity? parent = null)
        {
            var entity = SpawnGameObjectEntity(prefab, parent);

            var root = entity.Require<Root>();
            //reset to prefab scale in case it has been changed
            root.Transform.localScale = prefab.transform.localScale;
            args.Apply(root);

            return EnableEntity(entity);
        }

        public static Entity SpawnEntity(
            this RectTransform prefab,
            in TransformOperation2D args,
            Entity? parent = null)
        {
            var entity = SpawnGameObjectEntity(prefab.gameObject, parent);

            var root = entity.Require<Root>();
            args.Apply(root);

            return EnableEntity(entity);
        }

        private static IEntity SpawnGameObjectEntity(GameObject prefab, Entity? parent)
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
        private static Entity EnableEntity(IEntity entity)
        {
            entity.State = EntityState.Enabled;
            return entity.GetToken();
        }
        public static GameObject SpawnInstance(this GameObject prefab, TransformOperation args, Entity? parent = null)
            => prefab.SpawnEntity(args, parent).Require<Root>().Transform.gameObject;
        public static Entity SpawnEntity(this Component comp, TransformOperation args, Entity? parent = null)
            => SpawnEntity(comp.gameObject, args, parent);
        public static T SpawnInstance<T>(this T prefab, TransformOperation args, Entity? entity = null)
            where T : Component
            => prefab.gameObject.SpawnInstance(args, entity).GetComponent<T>();
        public static GameObject SpawnInstance(this BasePrefabAsset asset, TransformOperation args, Entity? entity = null)
            => asset._prefab.SpawnInstance(args, entity);
        public static T SpawnInstance<T>(this BasePrefabAsset<T> prefab, TransformOperation args, Entity? entity = null)
           where T : Component
           => prefab._prefab.SpawnInstance(args, entity);

        public static T SpawnInstance2D<T>(this T prefab, in TransformOperation2D args, Entity? entity = null)
            where T : EntityBehaviour2D
            => prefab.Rt.SpawnEntity(args, entity).Require<Root>().Transform.GetComponent<T>();
        public static RectTransform SpawnInstance2D(this BasePrefabAsset2D prefab, TransformOperation2D args, Entity? entity = default)
            => prefab._prefab.SpawnEntity(args, entity).Require<Root>().Transform.GetComponent<RectTransform>();

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
                    prefab.SpawnInstance(new() { Parent = parent }, parentEntity);
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