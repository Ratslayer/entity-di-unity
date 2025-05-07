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
	public abstract class EntityComponentAsset : BaseScriptableObject
	{
		public string Name => name;
		public abstract void Apply(Entity target);
	}
}