using System;
using System.IO;
using Lantern.EQ.AssetBundles;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Lantern;
using Lantern.EQ.Zone;
using UnityEditor;
using UnityEngine;
using File = System.IO.File;

namespace Lantern.EQ.Editor.AssetBundles
{
    /// <summary>
    /// Creates asset bundles for the LanternEQ client
    /// </summary>
    public static class BuildAssetBundles
    {
#if LANTERN_CLIENT
        [MenuItem("Lantern/General/Build Asset Bundles", false, 0)]
#else
        [MenuItem("EQ/Build Asset Bundles", false, 100)]
#endif
        public static void BuildAllAssetBundles()
        {
            BuildAllAssetBundles(true);
        }

        public static void BuildAllAssetBundles(bool showCompleteMessage)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Cannot build bundles while in playmode!");
                return;
            }

            var outputPath = PathHelper.GetSystemPathFromUnity(LanternConstants.AssetBundlePath);

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                AssetDatabase.Refresh();
            }

            double startTime = EditorApplication.timeSinceStartup;
            RestoreOriginalAssetBundleNames();
            AssetDatabase.Refresh();
            if (BuildPipeline.BuildAssetBundles(LanternConstants.AssetBundlePath, BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget) == null)
            {
                EditorUtility.DisplayDialog("AssetBundles",
                    $"Building asset bundles failed. This usually happens when there are compiler errors from the UnityEngine.Editor namespace being included in non editor scripts.",
                    "OK");
                return;
            }

            AssetDatabase.Refresh();
            SetAssetBundleVersions();
            AssetDatabase.Refresh();
            double endTime = EditorApplication.timeSinceStartup;

            if (showCompleteMessage)
            {
                EditorUtility.DisplayDialog("AssetBundles",
                    $"Asset bundle build finished in {(int) (endTime - startTime)} seconds", "OK");
            }
        }

        private static void RestoreOriginalAssetBundleNames()
        {
            string[] pathList = new string[1];
            pathList[0] = LanternConstants.AssetBundlePath;
            var assets = AssetDatabase.FindAssets("", pathList);

            foreach (var assetPath in assets)
            {
                var assetPathString = AssetDatabase.GUIDToAssetPath(assetPath);

                if (assetPathString.EndsWith(".meta"))
                {
                    continue;
                }

                var fileName = Path.GetFileNameWithoutExtension(assetPathString);

                if (string.IsNullOrEmpty(fileName))
                {
                    continue;
                }

                if (fileName == "AssetBundles")
                {
                    continue;
                }

                // Get the base type
                string baseType = fileName.Split('-')[0];

                LanternAssetBundleId? id = AssetBundleVersions.GetBundleIdFromName(baseType);

                if (!id.HasValue)
                {
                    continue;
                }

                var version = AssetBundleVersions.GetVersion(id.Value);

                if (version == null)
                {
                    continue;
                }

                // Create the version string we want
                if (fileName.EndsWith(version.ToString().Replace('.', '_'))
                    && !fileName.StartsWith("shaders"))
                {
                    // Rename it to the original so that it will not be regenerated
                    var assetsPathWithoutAssets = assetPathString.Remove(0, 6);
                    string oldPath = Application.dataPath + assetsPathWithoutAssets;

                    string bundleName = string.Empty;

                    if (id.Value == LanternAssetBundleId.Zones)
                    {
                        bundleName = baseType;
                    }
                    else
                    {
                        bundleName = id.Value.ToString().ToLower();
                    }

                    string newPath = Application.dataPath + Path.GetDirectoryName(assetsPathWithoutAssets) + "/" +
                                     bundleName +
                                     Path.GetExtension(assetsPathWithoutAssets);

                    if (File.Exists(newPath))
                    {
                        File.Delete(newPath);
                        AssetDatabase.Refresh();
                    }

                    // New path
                    File.Move(oldPath, newPath);
                }
                else
                {
                    // This is a file that is not a bundle and or an old bundle
                    var assetsPathWithoutAssets2 = assetPathString.Remove(0, 6);
                    string oldPath2 = Application.dataPath + assetsPathWithoutAssets2;
                    File.Delete(oldPath2);
                }
            }
        }

        private static void SetAssetBundleVersions()
        {
            var values = Enum.GetValues(typeof(LanternAssetBundleId));
            foreach (LanternAssetBundleId value in values)
            {
                if (value == LanternAssetBundleId.Zones)
                {
                    continue;
                }

                SetVersion(value.ToString());
            }

            foreach (var zone in ZoneHelper.ShortnameDict)
            {
                SetVersion(zone.Key);
            }
        }

        private static void SetVersion(string name)
        {
            string lowerName = name.ToLower();
            var bundlePath = Application.streamingAssetsPath + "/AssetBundles/" + lowerName;
            var manifestPath = Application.streamingAssetsPath + "/AssetBundles/" + lowerName + ".manifest";

            var id = AssetBundleVersions.GetBundleIdFromName(name);

            if (!id.HasValue)
            {
                return;
            }


            if (!AssetBundleVersions.Versions.ContainsKey(id.Value))
            {
                Debug.LogError(
                    "AssetBundleBuilder: Error building asset bundles. Version does not contain bundle id: " +
                    id.Value);
                return;
            }

            string version = AssetBundleVersions.Versions[id.Value].ToString().Replace(".", "_");
            var bundleWithVersion = bundlePath + "-" + version;
            var manifestWithVersion = bundlePath + "-" + version + ".manifest";

            if (File.Exists(bundlePath) && !File.Exists(bundleWithVersion))
            {
                File.Move(bundlePath, bundleWithVersion);
            }

            if (File.Exists(manifestPath) && !File.Exists(manifestWithVersion))
            {
                File.Move(manifestPath, manifestWithVersion);
            }
        }
    }
}
