namespace BB
{
    public interface ILoadableAssets
    {
        bool HasAssetKey(object asset, out string key);
        bool HasAsset<T>(string key, out T asset)
            where T : BaseScriptableObject, ILoadableAsset;
    }
}