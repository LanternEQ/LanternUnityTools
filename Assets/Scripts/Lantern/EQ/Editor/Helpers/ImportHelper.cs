using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Helpers
{
    public static class ImportHelper
    {
        public static bool LoadTextAsset(string assetPath, out string text)
        {
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

            if (textAsset == null)
            {
                text = string.Empty;
                return false;
            }

            text = textAsset.text;
            return true;
        }

        public static GameObject FixModelParent(GameObject createdObject, Transform newParent)
        {
            if (createdObject == null)
            {
                return null;
            }

            if (createdObject.transform.childCount == 0)
            {
                createdObject.transform.SetParent(newParent, true);
                return createdObject;
            }

            string name = createdObject.name;

            GameObject child = createdObject.transform.GetChild(0).gameObject;

            child.transform.SetParent(newParent, true);
            child.name = name;

            Object.DestroyImmediate(createdObject);

            return child;
        }

        public static void CopyDirectory(string sourceDir, string targetDir)
        {
            CopyAll(sourceDir, targetDir);
        }

        private static void CopyAll(string sourceDir, string targetDir, bool ignoreMetaFiles = true)
        {
            if (!Directory.Exists(sourceDir))
            {
                Debug.LogError($"ImportHelper: Unable to copy all textures. Source folder does not exist {sourceDir}");
                return;
            }

            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                if(ignoreMetaFiles && file.EndsWith(".meta"))
                {
                    continue;
                }

                string destination = Path.Combine(targetDir, Path.GetFileName(file));

                if (File.Exists(destination))
                {
                    continue;
                }

                File.Copy(file, destination);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
                CopyAll(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

        public static void TagAllAssetsForBundles(string folderPath, string bundleTag)
        {
            AssetImporter importer = AssetImporter.GetAtPath(folderPath);

            if (importer == null)
            {
                return;
            }

            importer.SetAssetBundleNameAndVariant(bundleTag, string.Empty);
            importer.SaveAndReimport();
        }
    }
}
