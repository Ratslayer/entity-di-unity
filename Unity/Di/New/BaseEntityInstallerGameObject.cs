using BB.Di;

namespace BB
{
	public abstract class BaseEntityInstallerGameObject
        : BaseEntityGameObject,
        IEntityBehaviour,
        IEntityInstaller
    {
        public string Name => name;
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
            _entityRef = SpawnEntity();
        }
        protected abstract IEntity SpawnEntity();
    }
}