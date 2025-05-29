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
	public abstract class EntityComponentAsset : BaseScriptableObject, IStateOnEnter, IStateOnExit
	{
		public string Name => name;
		public void ApplyOnSpawn(Entity entity)
			=> Apply(entity, null);

		public void EnterState(IStateMachine machine)
		{
			Apply(machine.Entity, machine);
		}

		public void ExitState(IStateMachine machine)
		{
			Unapply(machine.Entity, machine);
		}

		public virtual void Apply(Entity target, IStateMachine machine)
		{
		}
		public virtual void Unapply(Entity target, IStateMachine machine)
		{
		}
	}
}