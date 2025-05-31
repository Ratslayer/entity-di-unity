using BB.Di;
namespace BB
{
	public abstract class AttachedEntityComponentAsset : EntityComponentAsset, IEntityInstaller
	{
		public override void Apply(Entity target, IStateMachine machine)
		{
			var entity = this.SpawnAndAttachTo(target);
			if (machine is not null)
				Log.Info($"State {name} added {entity} to {target}");
			machine?.AddStateEntity(entity);
		}

		public virtual void Install(IDiContainer container)
		{
			container.Instance(GetType(), this);
		}
	}
	public abstract class AttachedEntityComponentAsset<T> : AttachedEntityComponentAsset
	{
		public override void Install(IDiContainer container)
		{
			base.Install(container);
			container.System<T>();
		}
	}
}