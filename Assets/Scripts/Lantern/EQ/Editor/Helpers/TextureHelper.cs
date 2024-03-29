﻿using System.IO;
using System.Linq;
using System.Text;
using Lantern.Editor.Importers;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Helpers
{
    public static class TextureHelper
    {
        public static void CopyTextures(string shortname, AssetImportType assetImportType)
        {
            string source =
                PathHelper.GetSystemPathFromUnity(PathHelper.GetLoadPath(shortname, assetImportType) + "Textures/");
            string destination =
                PathHelper.GetSystemPathFromUnity(PathHelper.GetSavePath(shortname, assetImportType) + "Textures/");
            ImportHelper.CopyDirectory(source, destination);
            AssetDatabase.Refresh();
        }

        // Tries to load the texture. If it doesn't exist, it copies it from the zone assets
        public static Texture GetTexture(string shortname, AssetImportType importType, string textureName,
            bool isMasked, bool outputFailure = true)
        {
            string fromPath = PathHelper.GetLoadPath(shortname, importType) + "Textures/";
            string toPath = PathHelper.GetSavePath(shortname, importType) + "Textures/";
            string pngPath = toPath + textureName + ".png";

            Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(pngPath);

            if (texture != null)
            {
                return texture;
            }

            // Texture does not exist - copy it
            string textureToCopyPath = PathHelper.GetSystemPathFromUnity(fromPath + textureName + ".png");
            string unityDestinationPath = toPath + textureName + ".png";
            string destination = PathHelper.GetSystemPathFromUnity(unityDestinationPath);

            if (!File.Exists(destination))
            {
                if (File.Exists(textureToCopyPath))
                {
                    CopyAndRefresh(destination, textureToCopyPath, unityDestinationPath, isMasked);
                }
                else
                {
                    // Check in global texture folder
                    string globalPath = Path.Combine(PathHelper.GetEqAssetPath(), "equipment/Textures/") + textureName + ".png";
                    globalPath = PathHelper.GetSystemPathFromUnity(globalPath);
                    
                    if (File.Exists(globalPath))
                    {
                        CopyAndRefresh(destination, globalPath, unityDestinationPath, isMasked);
                    }
                    else
                    {
                        if (outputFailure)
                        {
                            Debug.LogError("Unable to copy texture. Doesn't exist at: " + textureToCopyPath);
                        }
                    }
                }
            }

            texture = AssetDatabase.LoadAssetAtPath<Texture>(pngPath);

            return texture;
        }

        private static void CopyAndRefresh(string destination, string textureToCopyPath, string unityDestinationPath, bool isMasked)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination));
            File.Copy(textureToCopyPath, destination);

            AssetDatabase.Refresh();

            if (isMasked)
            {
                TextureImporter importer = AssetImporter.GetAtPath(unityDestinationPath) as TextureImporter;

                if (importer != null)
                {
                    importer.alphaIsTransparency = true;
                    importer.SaveAndReimport();
                }
            }
        }

        public static Texture FindFaceVariant(string modelName, Texture texture, int index)
        {
            if (texture == null)
            {
                return null;
            }

            string textureName = texture.name;

            // Edge case: Iksar females use the Iksar Citizen Female base texture.
            // Looking for additional textures with the ICF prefix won't find the
            // additional faces. Instead, we must look for the IKF prefix.
            if (modelName == "ikf" && texture.name == "icfhe0001")
            {
                textureName = "ikfhe0001";
            }

            StringBuilder variantName = new StringBuilder(textureName);
            variantName[variantName.Length - 2] = index.ToString().FirstOrDefault();
            var faceTexture = GetTexture("characters", AssetImportType.Characters, variantName.ToString(), false, false);
            return faceTexture;
        }

        public static Texture FindEquipmentVariant(Texture texture, int index, string requiredString)
        {
            if (texture == null)
            {
                // TODO: Why is this being called with a null texture?
                return null;
            }

            if (texture.name.StartsWith("clkerf") || texture.name.StartsWith("clkerm"))
            {
                if (index == 0)
                {
                    return texture;
                }

                if (!string.IsNullOrEmpty(requiredString) && !texture.name.Contains(requiredString))
                {
                    return null;
                }

                return GetTexture("all", AssetImportType.Characters, "clk" + index.ToString("00") + "06", false, false);
            }

            string textureName = texture.name;
            StringBuilder variantName = new StringBuilder(textureName);
            string index10 = index.ToString("00");
            variantName[variantName.Length - 4] = index10.FirstOrDefault();
            variantName[variantName.Length - 3] = index10.LastOrDefault();

            if (!string.IsNullOrEmpty(requiredString) && !variantName.ToString().Contains(requiredString))
            {
                return null;
            }

            return GetTexture("all", AssetImportType.Characters, variantName.ToString(), false, false);
        }
    }
}
