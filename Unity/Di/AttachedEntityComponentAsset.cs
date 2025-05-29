using BB.Di;
namespace BB
{
	public abstract class AttachedEntityComponentAsset : EntityComponentAsset, IEntityInstaller
	{
		public override void Apply(Entity target, IStateMachine machine)
		{
			var entity = this.SpawnAndAttachTo(target);
			machine?.AddStateEntity(entity);
        }

		public virtual void Install(IDiContainer container)
		{
			container.Instance(GetType(), this);
		}
	}
}