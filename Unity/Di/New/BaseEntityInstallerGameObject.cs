using BB.Di;
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
	}
}