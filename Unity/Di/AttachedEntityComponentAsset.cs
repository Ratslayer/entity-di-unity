using BB.Di;
namespace BB
{
	public abstract class AttachedEntityComponentAsset : EntityComponentAsset, IEntityInstaller
	{
		public override void Apply(Entity parent)
			=> this.SpawnAndAttachTo(parent);

		public virtual void Install(IDiContainer container)
		{
			container.Instance(GetType(), this);
		}
	}
}