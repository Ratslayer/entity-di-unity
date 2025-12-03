using BB.Di;

namespace BB
{
	public abstract class InstallerAsset2D : InstallerAsset
	{
		public override void Install(IDiContainer container)
		{
			base.Install(container);
			container.System<Root2D>();
		}
	}
}