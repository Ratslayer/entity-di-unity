using BB.Di;
namespace BB
{
	public abstract class InstallerAsset : BaseScriptableObject, IEntityInstaller
	{
		public string Name => name;
		public virtual void Install(IDiContainer container)
		{
			container.Instance(GetType(), this);
		}
	}
	public abstract class EntityComponentAsset : BaseScriptableObject, IEntityStateOnEnter, IEntityStateOnExit
	{
		public string Name => name;
		public void ApplyOnSpawn(Entity entity)
			=> Apply(entity, null);

		public void EnterState(IStateData data)
		{
			Apply(data.Entity, data);
		}

		public void ExitState(IStateData data)
		{
			Unapply(data.Entity, data);
		}

		public virtual void Apply(Entity target, IStateData data)
		{
		}
		public virtual void Unapply(Entity target, IStateData data)
		{
		}
	}
}