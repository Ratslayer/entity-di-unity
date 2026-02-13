using Sirenix.OdinInspector;
using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BB
{
    public sealed class LoadableAssets : BaseScriptableObject, ILoadableAssets
    {
        [SerializeField, ReadOnly]
        LoadableAsset[] _assets = { };

        [Serializable]
        struct LoadableAsset
        {
            [HideInInspector]
            public string _name;
            [HideInInspector]
            public LazyLoadReference<BaseScriptableObject> _asset;
            [ShowInInspector, LabelText("$_name")]
            public BaseScriptableObject Asset => _asset.asset;
        }
        public bool HasAsset<T>(string key, out T asset) 
            where T : BaseScriptableObject, ILoadableAsset
        {
            foreach (var a in _assets)
            {
                if (a._name != key)
                    continue;
                if (a.Asset is not T t)
                    continue;
                asset = t;
                return true;
            }
            asset = null;
            return false;
        }
        public bool HasAssetKey(object obj, out string key)
        {
            key = null;

            if (obj is not BaseScriptableObject asset)
                return false;

            if (obj is not ILoadableAsset loadableAsset)
                return false;

            if (!_assets.Contains(v => v._name == loadableAsset.AssetLoadKey))
                return false;

            key = loadableAsset.AssetLoadKey;

            return true;
        }
#if UNITY_EDITOR
        [Button]
        void AddAllAssets()
        {
            var assetIds = AssetDatabase.FindAssets($"t:{nameof(BaseScriptableObject)}");
            var assets = new List<LoadableAsset>();
            foreach (var id in assetIds)
            {
                var path = AssetDatabase.GUIDToAssetPath(id);
                var asset = AssetDatabase.LoadAssetAtPath<BaseScriptableObject>(path);
                if (asset is not ILoadableAsset la)
                    continue;
                if (string.IsNullOrWhiteSpace(la.AssetLoadKey))
                {
                    Log.Error($"{asset.name} is loadable but has no load key.");
                    continue;
                }
                assets.Add(new()
                {
                    _name = la.AssetLoadKey,
                    _asset = asset
                });
            }
            _assets = assets.ToArray();
        }
#endif
    }
}