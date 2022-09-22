using System.Collections.Generic;
using System.IO;
using Lantern.EQ.Editor.Helpers;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.EqAssetCopy
{
    public static class EqAssetCopier
    {
        [MenuItem("EQ/Copy EQ Assets", false, 30)]
        public static void Copy()
        {
            if (Application.isPlaying)
            {
                Debug.LogError("Cannot copy assets while playing.");
                return;
            }

            var startTime = EditorApplication.timeSinceStartup;

            var sourceRootFolder = PathHelper.GetSystemPathFromUnity(PathHelper.GetEqAssetPath());
            var destRootFolder = PathHelper.GetSystemPathFromUnity(PathHelper.GetAssetBundleContentPath());

            var failedToCopy = new List<string>();

            foreach (var atc in AssetList.AssetsToCopy)
            {
                var sourcePath = Path.Combine(sourceRootFolder, atc.EqFolder);
                var destPath = Path.Combine(destRootFolder, atc.AssetBundle.ToString());
                foreach (var file in atc.AssetsToCopy)
                {
                    if (!CopyAssetToBundle(sourcePath, destPath, file, atc.AssetImportType))
                    {
                        failedToCopy.Add(file);
                    }
                }
            }

            AssetDatabase.Refresh();

            foreach (var ats in AssetList.SpriteSheetsToSplice)
            {
                var destFolder = Path.Combine(destRootFolder, ats.AssetBundle.ToString());
                foreach (var file in ats.AssetToSplice)
                {
                    var destPath = PathHelper.GetUnityPathFromSystem(Path.Combine(destFolder, file));
                    SpriteSheetCreator.CreateSpriteSheet(destPath, ats.CellSize.x, ats.CellSize.y, ats.KeepEmptyRects,
                        ats.CellsToRemove);
                }
            }

            foreach (var ss in AssetList.SpriteSheetsToCreate)
            {
                var destFolder = Path.Combine(destRootFolder, ss.AssetBundle.ToString());
                var destPath = Path.Combine(destFolder, ss.SheetFileName);

                if (ss.AssetIndices != null)
                {
                    ss.AssetsToPack ??= new List<string>();
                    foreach(var i in ss.AssetIndices)
                    {
                        ss.AssetsToPack.Add($"{ss.AssetBase}{i:00}.png");
                    }
                }

                SpriteSheetCreator.CreateSpriteSheetAtlas(ss.EqFolder, ss.AssetsToPack, ss.Padding, destPath);
            }

            if (failedToCopy.Count > 0)
            {
                Debug.LogError($"Failed to copy {failedToCopy.Count} assets");
                foreach (var f in failedToCopy)
                {
                    Debug.LogError(f);
                }
            }

            AssetDatabase.Refresh();
            ImportHelper.TagAllAssetsForBundles(PathHelper.GetAssetBundleContentPath()+ "Sprites", "sprites");
            ImportHelper.TagAllAssetsForBundles(PathHelper.GetAssetBundleContentPath()+ "Startup", "startup");
            ImportHelper.TagAllAssetsForBundles(PathHelper.GetAssetBundleContentPath()+ "CharacterSelect_Classic", "characterselect_classic");
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("EQAssetsCopy",
                $"EQ asset copy finished in {(int) (EditorApplication.timeSinceStartup - startTime)} seconds", "OK");
        }

        private static bool CopyAssetToBundle(string sourcePath, string destPath, string file,
            AssetImportType assetImportType)
        {
            if (!Directory.Exists(sourcePath))
            {
                return false;
            }

            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            var sourceFile = Path.Combine(sourcePath, file);
            var destinationFile = Path.Combine(destPath, file);

            if (!File.Exists(sourceFile))
            {
                Debug.LogError("Source file does not exist: " + sourceFile);
                return false;
            }

            if (File.Exists(destinationFile))
            {
                return true;
            }

            File.Copy(sourceFile, destinationFile);
            var unityPath = PathHelper.GetUnityPathFromSystem(destPath);

            switch (assetImportType)
            {
                case AssetImportType.Texture2d:
                {
                    if (!string.IsNullOrEmpty(unityPath))
                    {
                        string assetPath = unityPath + "/" + file;
                        AssetDatabase.Refresh();
                        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                        if (importer != null)
                        {
                            importer.alphaSource = TextureImporterAlphaSource.None;
                            importer.mipmapEnabled = false;
                            importer.isReadable = true;
                            importer.SaveAndReimport();
                        }
                    }

                    break;
                }
                case AssetImportType.Sprite:
                case AssetImportType.SpriteSheet:
                {
                    if (!string.IsNullOrEmpty(unityPath))
                    {
                        string assetPath = unityPath + "/" + file;
                        AssetDatabase.Refresh();
                        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                        if (importer != null)
                        {
                            importer.textureType = TextureImporterType.Sprite;
                            importer.alphaSource = TextureImporterAlphaSource.FromInput;
                            importer.textureCompression = TextureImporterCompression.Uncompressed;
                            importer.isReadable = true;
                            importer.spriteImportMode = assetImportType == AssetImportType.SpriteSheet
                                ? SpriteImportMode.Multiple
                                : SpriteImportMode.Single;
                            importer.SaveAndReimport();
                        }
                    }

                    break;
                }
            }

            return true;
        }
    }
}
