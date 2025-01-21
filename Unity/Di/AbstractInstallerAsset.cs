using BB.Di;
public abstract class AbstractInstallerAsset : BaseScriptableObject, IEntityInstaller
{
	public string Name => name;
	public abstract void Install(IDiContainer container);
}
public abstract class InstallerAsset : AbstractInstallerAsset
{
	public override void Install(IDiContainer container)
	{
		container.Instance(GetType(), this);
	}
}