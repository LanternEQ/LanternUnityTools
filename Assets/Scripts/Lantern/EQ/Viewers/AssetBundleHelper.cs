using System;
using System.IO;
using Lantern.EQ.AssetBundles;
using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public static class AssetBundleHelper
    {
        /// <summary>
        /// Takes an asset bundle ID and returns the versioned asset bundle name
        /// </summary>
        /// <param name="assetBundleId"></param>
        /// <returns></returns>
        public static string GetAssetBundleName(LanternAssetBundleId assetBundleId)
        {
            var bundleName = assetBundleId.ToString().ToLower();
            var version = AssetBundleVersions.GetVersion(assetBundleId);
            string versionString = version.ToString();
            bundleName += "-" + versionString.Replace(".", "_");
            return bundleName;
        }

        /// <summary>
        /// Takes an asset bundle name and a category and returns the versioned asset bundle name
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="bundleCategory"></param>
        /// <returns></returns>
        public static string GetAssetBundleName(LanternAssetBundleId bundleCategory, string bundleName)
        {
            var version = AssetBundleVersions.GetVersion(bundleCategory);
            string versionString = version.ToString();
            bundleName += "-" + versionString.Replace(".", "_");
            return bundleName;
        }

        public static string GetAssetBundlePath()
        {
            return Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        }

        public static bool DoesGlobalBundleExist(LanternAssetBundleId assetBundleId)
        {
            var bundleName = GetAssetBundleName(assetBundleId);
            string path = GetAssetBundlePath();
            return File.Exists(Path.Combine(path, bundleName));
        }

        public static bool DoesZoneBundleExist(string shortname)
        {
            shortname = shortname.ToLower();
            var bundleName = GetAssetBundleName(LanternAssetBundleId.Zones, shortname);
            string path = GetAssetBundlePath();
            return File.Exists(Path.Combine(path, bundleName));
        }

        public static bool IsGlobalBundle(string fileName)
        {
            foreach (var bundleId in Enum.GetNames(typeof(LanternAssetBundleId)))
            {
                if (fileName.ToLower().StartsWith(bundleId.ToLower()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
