using System;
using System.Collections.Generic;
namespace BB.Di
{
	public sealed class EntityGameObject
		: BaseBehaviour,
		IEntityBehaviour,
		IEntityInstaller,
		IDespawnable,
		IDisposable
	{
		public Entity Entity => _entity is null ? default : _entity.GetToken();
		[Inject]
		IEntity _entity;
		public IEntityPool _pool;
		public string Name => name;
		public void Install(IDiContainer container)
		{
			container.System<Root>(transform);
			container.Instance(this).Inject().BindEvents();
			var installers = GetComponents<IEntityInstaller>();
			foreach (var installer in installers)
				if (installer != (IEntityInstaller)this)
					installer.Install(container);
		}
		public void Despawn()
		{
			Entity.Despawn();
			_pool?.Return(Entity._ref);
		}
		[OnPostSpawn]
		void Spawn()
		{
			foreach (var child in _children)
				child.Spawn();
		}
		public readonly List<EntityGameObject> _children = new();
		public EntityGameObject Parent { get; set; }
		public void Dispose()
		{
			if (gameObject)
				Destroy(gameObject);
		}
		private void OnDestroy()
		{
			Entity._ref.State = EntityState.Disposed;
		}
	}
}