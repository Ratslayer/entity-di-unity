using BB.Di;
public abstract class BaseInstallerAsset : BaseScriptableObject, IEntityInstaller
{
	public string Name => name;
	public abstract void Install(IDiContainer container);
}