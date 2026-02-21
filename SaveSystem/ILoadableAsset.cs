using UnityEngine;

namespace BB
{
    public interface ILoadableAsset
    {
        string AssetLoadKey { get; set; }
    }
    public abstract class LoadableScriptableObject : BaseScriptableObject, ILoadableAsset
    {
        [SerializeField] string _assetLoadKey;
        public string AssetLoadKey
        {
            get => _assetLoadKey;
            set => _assetLoadKey = value;
        }
    }
}