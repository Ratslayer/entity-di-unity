using BB.Di;

public abstract class InstallerAsset : BaseInstallerAsset
{
	public override void Install(IDiContainer container)
	{
		container.InjectedInstance(GetType(), this);
	}
}