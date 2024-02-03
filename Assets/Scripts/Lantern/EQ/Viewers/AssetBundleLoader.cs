using System.Collections.Generic;
using System.IO;
using Lantern.EQ.AssetBundles;
using UnityEngine;

namespace Lantern.EQ.Viewers
{
    /// <summary>
    /// A dirt simple asset bundle loader
    /// Assumes asset bundles are located in Assets/StreamingAssets/AssetBundles
    /// This classes uses the LanternEQ bundle names
    /// Asset bundles can be loaded both by their ID and by raw name (e.g. zone bundles)
    /// </summary>
    public class AssetBundleLoader
    {
        private string _assetBundlePath = Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        private Dictionary<string, AssetBundle> _loadedBundles = new();

        public bool LoadAssetBundle(string assetBundleName)
        {
            var path = Path.Combine(_assetBundlePath, assetBundleName);

            if (_loadedBundles.ContainsKey(path))
            {
                return true;
            }

            var bundle = AssetBundle.LoadFromFile(path);

            if (bundle == null)
            {
                Debug.LogError($"Unable to load asset bundle at path: {path}");
                return false;
            }

            _loadedBundles[assetBundleName] = bundle;
            return true;
        }

        public T LoadAsset<T>(LanternAssetBundleId assetBundleId, string assetName) where T : Object
        {
            var bundleName = AssetBundleHelper.GetAssetBundleName(assetBundleId);
            if (_loadedBundles.TryGetValue(bundleName, out var bundle))
            {
                return bundle.LoadAsset<T>(assetName);
            }

            return !LoadAssetBundle(bundleName) ? default : _loadedBundles[bundleName].LoadAsset<T>(assetName);
        }

        public T LoadAsset<T>(string assetBundleName, LanternAssetBundleId bundleType, string assetName) where T : Object
        {
            var path = AssetBundleHelper.GetAssetBundleName(bundleType, assetBundleName);
            if (_loadedBundles.TryGetValue(path, out var bundle))
            {
                return bundle.LoadAsset<T>(assetName);
            }

            return !LoadAssetBundle(path) ? default : _loadedBundles[path].LoadAsset<T>(assetName);
        }
    }
}
