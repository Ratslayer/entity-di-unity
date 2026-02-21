using BB.Di;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BB
{
    public abstract class BaseEntityInstallerGameObject
        : BaseEntityGameObject,
        IEntityBehaviour,
        IEntityInstaller
    {
        public string _serializationName;
        public string Name => name;
        public override GameObject Prefab => null;
        bool _selfSpawned;
        public virtual void Install(IDiContainer container)
        {
            container.System<GameObjectWrapper>();
        }
        public override void Init(IEntity entity)
        {
            base.Init(entity);
            foreach (var comp in GetComponents<BaseEntityComponent>())
                comp.Init();
        }
        public override void Despawn() => _entityRef.SetState(EntityState.Despawned);
        private void Awake()
        {
            if (_entityRef is not null)
                return;
            _selfSpawned = true;
            _entityRef = SpawnEntity();
            if (_entityRef is IEntityDetails details)
                details.OneShot = true;
        }
        protected abstract IEntity SpawnEntity();
        private void OnDestroy()
        {
            if (_selfSpawned)
                _entityRef.SetState(EntityState.Destroyed);
            else _entityRef.SetState(EntityState.Despawned);
        }

#if UNITY_EDITOR
        [Button, HorizontalGroup]
        void SetKey()
        {
            var objects = GetObjects();
            var (invalidKeyObjects, keys) = GetInvalidObjectsAndKeys(objects);
            if (invalidKeyObjects.Contains(this))
                InitKey(this, keys);
        }
        [Button, HorizontalGroup]
        void SetAllKeys()
        {
            var objects = GetObjects();
            var (invalidKeyObjects, keys) = GetInvalidObjectsAndKeys(objects);

            foreach (var obj in invalidKeyObjects)
                InitKey(obj, keys);
        }
        BaseEntityInstallerGameObject[] GetObjects()
            => FindObjectsByType<BaseEntityInstallerGameObject>(FindObjectsSortMode.InstanceID);
        (List<BaseEntityInstallerGameObject>, HashSet<string>) GetInvalidObjectsAndKeys(BaseEntityInstallerGameObject[] objects)
        {
            var invalidKeyObjects = new List<BaseEntityInstallerGameObject>();
            var keys = new HashSet<string>();
            foreach (var obj in objects)
            {
                if (string.IsNullOrEmpty(obj._serializationName))
                {
                    invalidKeyObjects.Add(obj);
                    continue;
                }

                if (!keys.Add(obj._serializationName))
                    invalidKeyObjects.Add(obj);
            }

            return (invalidKeyObjects, keys);
        }
        void InitKey(BaseEntityInstallerGameObject obj, HashSet<string> keys)
        {
            var tokens = obj.name.SplitByWords();

            string prefix;
            int id;
            if (int.TryParse(tokens[^1], out var tokenId))
            {
                prefix = string.Join('_', tokens.SkipLast(1));
                id = tokenId;
            }
            else
            {
                prefix = string.Join('_', tokens);
                id = 1;
            }

            string key;
            while (true)
            {
                key = $"{prefix}_{id}";
                if (keys.Add(key))
                    break;
                id++;
            }

            obj._serializationName = key;
            obj.SetDirty();
        }
#endif
    }
}