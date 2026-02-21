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
    public abstract class LoadableInstallerAsset : InstallerAsset, ILoadableAsset
    {
		public string _assetLoadKey;
        public string AssetLoadKey
		{
			get => _assetLoadKey;
			set => _assetLoadKey = value;
		}
    }
}