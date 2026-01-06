using BB.Di;
using UnityEngine;
namespace BB
{
    public abstract class BaseRoot
    {
        [Inject] GameObjectWrapper _gameObject;
        public virtual void Init() { }
        public virtual void Clear() { }
        public GameObject GameObject => _gameObject.GameObject;
        public T GetComponent<T>() => GameObject.GetComponent<T>();

        #region Events
        [OnEvent]
        void OnEnable(EntityEnabledEvent _)
        {
            if (GameObject)
                GameObject.SetActive(true);
        }
        [OnEvent]
        void OnDisable(EntityDisabledEvent _)
        {
            if (GameObject)
                GameObject.SetActive(false);
        }
        [OnEvent]
        void OnDespawn(EntityDespawnedEvent _)
        {
            if (!GameObject.TryGetComponent(out PooledGameObject pgo))
                return;
            Clear();
            pgo.Despawn();
        }

        #endregion
    }
}