using Sirenix.OdinInspector;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;


#if UNITY_EDITOR
using BB.Actions;
using UnityEditor;
#endif

namespace BB
{
    public static class LoadableAssetsUtils
    {
        public static void SetNameToInit(LoadableScriptableObject asset)
        {
            var tokens = asset.name.SplitByWords();
            var prefix = asset.DefaultNamePrefix;
            // var prefix = asset switch
            // {
            //     BaseBoardKey => "board_key",
            //     BaseItemAsset => "item",
            //     PlayerActionAsset => "player_action",
            //     _ => "asset"
            // };

            var suffix = string.Join('_', tokens);
            var name = string.Join('_', prefix, suffix);

            Log.Info($"{asset.name} will have its key set to {name}");

            asset.AssetLoadKey = name;
            asset.Dirty();
        }
    }

    public sealed class LoadableAssets : BaseScriptableObject, ILoadableAssets
    {
        [SerializeField, ReadOnly] LoadableAsset[] _assets = { };

        [Serializable]
        struct LoadableAsset
        {
            [HideInInspector] public string _name;
            [HideInInspector] public LazyLoadReference<BaseScriptableObject> _asset;
            [ShowInInspector, LabelText("$_name")] public BaseScriptableObject Asset => _asset.asset;
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
            AddAssets(Filter);

            LoadableScriptableObject Filter(LoadableScriptableObject asset)
            {
                if (string.IsNullOrWhiteSpace(asset.AssetLoadKey))
                {
                    Log.Error($"{asset.name} is loadable but has no load key.");
                    return null;
                }

                return asset;
            }
        }

        [Button]
        void AddAndInitAllAssets()
        {
            AddAssets(Init);

            LoadableScriptableObject Init(LoadableScriptableObject asset)
            {
                if (string.IsNullOrWhiteSpace(asset.AssetLoadKey))
                    LoadableAssetsUtils.SetNameToInit(asset);

                return asset;
            }
        }

        void AddAssets(Func<LoadableScriptableObject, LoadableScriptableObject> filter)
        {
            var assetIds = AssetDatabase.FindAssets($"t:{nameof(LoadableScriptableObject)}");
            var assets = new List<LoadableAsset>();
            foreach (var id in assetIds)
            {
                var path = AssetDatabase.GUIDToAssetPath(id);
                var asset = AssetDatabase.LoadAssetAtPath<LoadableScriptableObject>(path);
                asset = filter(asset);
                if (asset is null)
                    continue;
                assets.Add(new()
                {
                    _name = asset.AssetLoadKey,
                    _asset = asset
                });
            }

            _assets = assets.ToArray();
        }
#endif
    }
}