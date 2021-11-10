#if UNITY_EDITOR
using System;
using System.IO;
using Lantern.Editor.Importers;
using Lantern.Global.AssetBundles;
using Lantern.Services;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using File = System.IO.File;

namespace Infrastructure.Editor
{
    public static class CreateBuilds
    {
        [MenuItem("Lantern/General/Build AssetBundles")]
        public static void BuildAllAssetBundles()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Cannot build bundles while in playmode!");
                return;
            }

            var outputPath = PathHelper.GetSystemPathFromUnity("Assets/StreamingAssets/AssetBundles/");

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                AssetDatabase.Refresh();
            }

            double startTime = EditorApplication.timeSinceStartup;
            RestoreOriginalAssetBundleNames();
            AssetDatabase.Refresh();
            if (BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.None,
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
            EditorUtility.DisplayDialog("AssetBundles",
                $"Asset bundle build finished in {(int) (endTime - startTime)} seconds", "OK");
        }

        private static void RestoreOriginalAssetBundleNames()
        {
            string path = "Assets/StreamingAssets/AssetBundles";

            string[] pathList = new string[1];
            pathList[0] = path;
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

                GlobalAssetBundleId? id = AssetBundleVersions.GetBundleIdFromName(baseType);

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
                if (fileName.EndsWith(version.ToString().Replace('.', '_')))
                {
                    // Rename it to the original so that it will not be regenerated
                    var assetsPathWithoutAssets = assetPathString.Remove(0, 6);
                    string oldPath = Application.dataPath + assetsPathWithoutAssets;

                    string bundleName = string.Empty;

                    if (id.Value == GlobalAssetBundleId.Zones)
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
            var values = Enum.GetValues(typeof(GlobalAssetBundleId));
            foreach (GlobalAssetBundleId value in values)
            {
                if (value == GlobalAssetBundleId.Zones)
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

        private static BuildPlayerOptions GetBuildOptions(BuildTarget target, bool isDebug = false)
        {
            string folderName = GetPlatformFolderName(target);

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] {"Assets/Scenes/ZoneViewerRoot.unity"},
                locationPathName = $"Builds/{folderName}/LZV/LZV",
                target = target,
                options = isDebug ? BuildOptions.Development : BuildOptions.None
            };

            if (target == BuildTarget.StandaloneWindows64)
            {
                buildPlayerOptions.locationPathName += ".exe";
            }

            return buildPlayerOptions;
        }

        private static string GetPlatformFolderName(BuildTarget target)
        {
            switch (target)
            {
                case BuildTarget.Android:
                {
                    return "Android";
                }
                case BuildTarget.StandaloneLinux64:
                {
                    return "Linux";
                }
                case BuildTarget.StandaloneOSX:
                {
                    return "macOS";
                }
                case BuildTarget.StandaloneWindows64:
                {
                    return "Windows";
                }
            }

            return string.Empty;
        }

        [MenuItem("Lantern/Build OSX")]
        public static void BuildOSX()
        {
            bool wasSuccessful = CreateBuild(BuildTarget.StandaloneOSX);
            EditorUtility.DisplayDialog("Build finished", GetBuildMessage(wasSuccessful), "OK");
        }

        private static string GetBuildMessage(bool wasSuccessful)
        {
            return wasSuccessful ? "Build completed successfully" : "Build failed. See console for log";
        }

        [MenuItem("Lantern/Build Linux")]
        public static void BuildLinux()
        {
            bool wasSuccessful = CreateBuild(BuildTarget.StandaloneLinux64);
            EditorUtility.DisplayDialog("Build finished", GetBuildMessage(wasSuccessful), "OK");
        }

        [MenuItem("Lantern/Build Windows")]
        public static void BuildWindows()
        {
            bool wasSuccessful = CreateBuild(BuildTarget.StandaloneWindows64);
            EditorUtility.DisplayDialog("Build finished", GetBuildMessage(wasSuccessful), "OK");
        }

        [MenuItem("Lantern/Build All")]
        public static void BuildAll()
        {
            bool wasSuccessfulWindows = CreateBuild(BuildTarget.StandaloneWindows64);
            bool wasSuccessfulLinux = CreateBuild(BuildTarget.StandaloneLinux64);
            bool wasSuccessfulOsx = CreateBuild(BuildTarget.StandaloneOSX);
            EditorUtility.DisplayDialog("Builds finished",
                GetBuildMessage(wasSuccessfulWindows && wasSuccessfulLinux && wasSuccessfulOsx), "OK");
        }

        public static bool CreateBuild(BuildTarget targetPlatform)
        {
            BuildPipeline.BuildAssetBundles("Assets/StreamingAssets/AssetBundles", BuildAssetBundleOptions.None,
                targetPlatform);

            var buildOptions = GetBuildOptions(targetPlatform);

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                return true;
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
                return false;
            }

            return false;
        }
    }
}
#endif