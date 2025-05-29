using BB.Di;
namespace BB
{
	public abstract class ChildEntityComponentAsset : EntityComponentAsset, IEntityInstaller
	{
		public virtual void Install(IDiContainer container)
		{
			container.Instance(GetType(), this);
		}

		public override void Apply(Entity target, IStateMachine machine)
		{
			var entity = this.Spawn(target);
			machine?.AddStateEntity(entity);
		}
	}
}