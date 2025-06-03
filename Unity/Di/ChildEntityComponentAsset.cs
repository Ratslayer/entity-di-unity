using BB.Di;
namespace BB
{
	public abstract class ChildEntityComponentAsset : EntityComponentAsset, IEntityInstaller
	{
		public virtual void Install(IDiContainer container)
		{
			container.Instance(GetType(), this);
		}

		public override void Apply(Entity target, IStateData data)
		{
			var entity = this.Spawn(target);
			data?.AddEntity(entity);
		}
	}
}