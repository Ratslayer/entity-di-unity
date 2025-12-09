using BB.Di;

namespace BB
{
    public abstract class InstallerAsset2D : InstallerAsset, IEntityInstaller2D
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<Root2D>();
        }
    }
}