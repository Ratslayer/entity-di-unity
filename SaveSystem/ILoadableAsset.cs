using UnityEngine;

namespace BB
{
	public interface ILoadableAsset
    {
        string AssetLoadKey { get; }
    }
    public abstract class LoadableScriptableObject : BaseScriptableObject, ILoadableAsset
    {
        [SerializeField] string _assetLoadKey;
        public string AssetLoadKey => _assetLoadKey;
    }
}