namespace BB
{
	public interface ILoadableAssets
    {
        bool HasAsset(string key, out BaseScriptableObject asset);
    }
}