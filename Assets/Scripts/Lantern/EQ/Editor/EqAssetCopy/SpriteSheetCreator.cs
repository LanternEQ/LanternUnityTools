using System.Collections.Generic;
using System.IO;
using Lantern.EQ.Editor.Helpers;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Lantern.EQ.Editor.EqAssetCopy
{
    public class SpriteSheetCreator : MonoBehaviour
    {
        public static void CreateSpriteSheet(string filePath, int width, int height, bool keepEmptyRects, List<int> cellsToRemove)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(filePath);
            var importer = AssetImporter.GetAtPath(filePath) as TextureImporter;

            if (texture == null)
            {
                Debug.LogError("Unable to load texture for sprite sheet creation: " + filePath);
                return;
            }

            if (importer == null)
            {
                Debug.LogError("Cannot load texture importer.");
                return;
            }

            var newData = new List<SpriteMetaData>();
            var rects = InternalSpriteUtility.GenerateGridSpriteRectangles(texture, Vector2.zero,
                new Vector2(width, height), Vector2.zero, keepEmptyRects);

            for (int i = 0; i < rects.Length; i++)
            {
                var smd = new SpriteMetaData
                {
                    rect = rects[i],
                    pivot = new Vector2(0.5f, 0.5f),
                    alignment = (int)SpriteAlignment.Center,
                    name = texture.name + "_" + i
                };

                newData.Add(smd);
            }

            if (cellsToRemove != null && cellsToRemove.Count != 0)
            {
                for (int i = cellsToRemove.Count - 1; i >= 0; i--)
                {
                    int toRemove = cellsToRemove[i];
                    if (toRemove < 0 || toRemove >= newData.Count)
                    {
                        continue;
                    }
                    newData.RemoveAt(cellsToRemove[i]);
                }
            }

            // Wrote this before knowing about InternalSpriteUtility
            // Worked well, will keep here for reference
            /*
            int sliceWidth = width;
            int sliceHeight = height;
            int rows = texture.height / sliceHeight;
            int columns = texture.width / sliceWidth;
            int runningCount = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = j * sliceWidth;
                    int y  = texture.height - (i + 1) * sliceHeight;
                    SpriteMetaData smd = new SpriteMetaData
                    {
                        pivot = new Vector2(0.5f, 0.5f),
                        alignment = (int)SpriteAlignment.Center,
                        name = texture.name + "_" + runningCount,
                        rect = new Rect(x, y, sliceWidth, sliceHeight)
                    };

                    newData.Add(smd);
                    runningCount++;
                }
            }*/

            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritesheet = newData.ToArray();
            importer.SaveAndReimport();
            AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
        }

        public static void CreateSpriteSheetAtlas(string sourceFolder, List<string> assets, int padding, string savePath)
        {
            var textures = new List<Texture2D>();
            var sourceRootFolder = Path.Combine(PathHelper.GetEqAssetPath(), sourceFolder);

            foreach (var a in assets)
            {
                var sourcePath = Path.Combine(sourceRootFolder, a);
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(sourcePath);
                if (texture != null)
                {
                    textures.Add(texture);
                }
                else
                {
                    Debug.LogError($"Unable to load all required textures for sprite sheet creation. Missing {sourcePath}");
                    return;
                }

                if (!texture.isReadable)
                {
                    var i = AssetImporter.GetAtPath(sourcePath) as TextureImporter;
                    if (i != null)
                    {
                        i.isReadable = true;
                        i.SaveAndReimport();
                    }
                }
            }

            var atlas = new Texture2D(512, 512);
            var rects = atlas.PackTextures(textures.ToArray(), padding, 512);
            var bytes = atlas.EncodeToPNG();
            var unityPath = PathHelper.GetUnityPathFromSystem(savePath);
            File.WriteAllBytes(savePath, bytes);
            AssetDatabase.ImportAsset(unityPath, ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();

            var importer = AssetImporter.GetAtPath(unityPath) as TextureImporter;

            if (importer == null)
            {
                Debug.LogError($"Cannot load importer for sprite sheet at: {unityPath}");
                return;
            }

            var smd = new List<SpriteMetaData>();
            for (int i = 0; i < textures.Count; ++i)
            {
                var sourceRect = rects[i];
                smd.Add(new SpriteMetaData
                {
                    name = textures[i].name,
                    rect = new Rect(sourceRect.x * atlas.width,
                        sourceRect.y * atlas.height,
                        sourceRect.width * atlas.width,
                        sourceRect.height * atlas.height),
                    pivot = new Vector2(0.5f, 0.5f)
                });
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritesheet = smd.ToArray();
            importer.SaveAndReimport();
        }
    }
}
