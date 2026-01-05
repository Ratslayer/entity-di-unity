using BB.Di;

namespace BB
{
	public abstract class BaseEntityGameObject : BaseBehaviour
    {
        protected IEntity _entityRef;
        public Entity Entity => _entityRef?.GetToken() ?? default;
        public virtual void Init(IEntity entity)
        {
            _entityRef = entity;
        }
        public abstract void Despawn();
    }
}