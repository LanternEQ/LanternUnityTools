using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lantern.Editor.Helpers;
using Lantern.EQ.Animation;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class MaterialImporter
    {
        public static void CreateMaterials(string shortname, AssetImportType importType,
            TextureAnimation materialSetter, List<AnimatedMaterial> animatedMaterials = null)
        {
            string rootPath = PathHelper.GetSystemPathFromUnity(PathHelper.GetSavePath(shortname, importType));

            Directory.CreateDirectory(rootPath);
            Directory.CreateDirectory(rootPath + "Materials/");
            Directory.CreateDirectory(rootPath + "Textures/");

            // Create the materials
            string materialList = string.Empty;
            string materialsPath = string.Empty;

            if (importType == AssetImportType.Sky)
            {
                materialsPath = "Assets/ZoneAssets/Classic/sky/sky_materials_sky.txt";
            }
            else
            {
                materialsPath = "Assets/ZoneAssets/Classic/" + shortname + "/materials_" +
                                importType.ToString().ToLower() + ".txt";
            }

            if (!ImportHelper.LoadTextAsset(materialsPath, out materialList))
            {
                Debug.LogError("Unable to load materials list: " + materialsPath);
                return;
            }

            var lines = TextParser.ParseTextByDelimitedLines(materialList, ',', '#');

            foreach (var line in lines)
            {
                Material newMaterial = MaterialHelper.CreateMaterial(line[0]);

                int frameCount = Convert.ToInt32(line[1]);

                if (frameCount <= 1)
                {
                    if (line.Count > 2)
                    {
                        string textureName = line[2];

                        // Find the main texture
                        Texture mainTexture = TextureHelper.GetTexture(shortname, importType, textureName,
                            line[0].StartsWith("tm_"));

                        if (mainTexture == null)
                        {
                            Debug.LogError("Unable to load png: " + textureName);
                            continue;
                        }

                        newMaterial.SetTexture("_BaseMap", mainTexture);
                    }
                }
                else
                {
                    List<string> textureNames = TextParser.ParseStringToList(line[3]);

                    List<Texture> textures = new List<Texture>();

                    foreach (var texture in textureNames)
                    {
                        textures.Add(
                            TextureHelper.GetTexture(shortname, importType, texture, line[0].StartsWith("tm_")));
                    }

                    Texture mainTexture = textures[0];

                    if (mainTexture == null)
                    {
                        Debug.LogError("Unable to load png: " + textureNames[0]);
                        continue;
                    }

                    newMaterial.SetTexture("_BaseMap", mainTexture);

                    int delay = Convert.ToInt32(line[2]);

                    /*if (materialSetter != null)
                    {
                        materialSetter.AddInstance(new AnimatedMaterial
                        {
                            Delay = delay,
                            Material = newMaterial,
                            
                        }newMaterial, textures, delay);
                    }*/

                    animatedMaterials?.Add(new AnimatedMaterial
                    {
                        Material = newMaterial,
                        Delay = delay,
                        Textures = textures,
                    });
                }

                string savePath = PathHelper.GetSavePath(shortname, importType) + "Materials/" + line[0] + ".mat";
                AssetDatabase.CreateAsset(newMaterial, savePath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static List<Material[]> LoadAllMaterialsForList(string zoneName, string assetMaterialPath,
            string savePath, AssetImportType importType, List<AnimatedMaterial> textureAnimations = null)
        {
            if (!ImportHelper.LoadTextAsset(assetMaterialPath, out var zoneMaterialsAsset))
            {
                return default;
            }

            var newMaterials = new List<Material[]>();
            var materialLines = TextParser.ParseTextByDelimitedLines(zoneMaterialsAsset, ',');
            int materialCount = materialLines.Count;

            newMaterials.Add(new Material[materialCount]);
            var baseMaterials = newMaterials[0];

            // Get the materials
            foreach (var line in materialLines)
            {
                int index = Convert.ToInt32(line[0]);

                List<string> allMaterials = line[1].Split(';').ToList();

                for (int i = 0; i < allMaterials.Count; ++i)
                {
                    if (string.IsNullOrEmpty(allMaterials[i]))
                    {
                        continue;
                    }

                    List<string> materialComponents = allMaterials[i].Split(':').ToList();

                    string materialName = materialComponents[0];
                    List<string> textureNames = new List<string>();

                    if (materialComponents.Count >= 2)
                    {
                        for (int j = 1; j < materialComponents.Count; ++j)
                        {
                            textureNames.Add(materialComponents[j]);
                        }
                    }

                    Material material = null;

                    // Load the material if it exists
                    if (materialName != string.Empty)
                    {
                        string materialPath = savePath + "Materials/" + materialName + ".mat";
                        material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                        if (material == null)
                        {
                            material = CreateMaterial(zoneName, materialName, textureNames.FirstOrDefault(),
                                importType);
                        }

                        if (material != null && textureNames.Count > 1 && textureAnimations != null || materialName == "tau_fire1")
                        {
                            int delay = Convert.ToInt32(line[2]);
                            
                            // Fix for fire bug
                            if (materialName == "tau_fire1" && textureNames.Count != 4)
                            {
                                textureNames.Clear();
                                textureNames.Add("fire1");
                                textureNames.Add("fire2");
                                textureNames.Add("fire3");
                                textureNames.Add("fire4");
                                delay = 100;
                            }
                            
                            var am = new AnimatedMaterial
                            {
                                Material = material,
                                Delay = delay,
                                TextureIndex = index,
                                Textures = new List<Texture>()
                            };

                            foreach (var texture in textureNames)
                            {
                                am.Textures.Add(TextureHelper.GetTexture(zoneName, importType, texture, false));
                            }
                            
                            textureAnimations.Add(am);
                        }
                    }

                    // Fallback to base material
                    if (material == null)
                    {
                        material = baseMaterials[index];

                        if (material == null)
                        {
                            // Load up default material
                            string materialPath = "Assets/Content/Materials/d_default.mat";
                            material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                        }
                    }

                    if (i >= newMaterials.Count)
                    {
                        newMaterials.Add(new Material[materialCount]);
                    }

                    if (index >= newMaterials[i].Length)
                    {
                        continue;
                    }

                    newMaterials[i][index] = material;
                }
            }

            return newMaterials;
        }

        private static Material CreateMaterial(string shortname, string materialName, string mainTextureName,
            AssetImportType importType)
        {
            Material newMaterial = MaterialHelper.CreateMaterial(materialName);
            if (newMaterial == null)
            {
                return null;
            }
            
            if (!string.IsNullOrEmpty(mainTextureName))
            {
                // Find the main texture
                Texture mainTexture = TextureHelper.GetTexture(shortname, importType, mainTextureName,
                    mainTextureName.StartsWith("tm_"));

                if (mainTexture == null)
                {
                    Debug.LogError("Unable to load png: " + mainTextureName);
                    return null;
                }

                newMaterial.SetTexture("_BaseMap", mainTexture);
                
                // Even though URP/our shaders don't use _MainTex, it needs to be serialized
                // Without this, the FBX exports won't have textures
                newMaterial.SetTexture("_MainTex", mainTexture);
            }

            string fullExportPath = PathHelper.GetSystemPathFromUnity(PathHelper.GetSavePath(shortname, importType)) +
                                    "Materials/";
            Directory.CreateDirectory(fullExportPath);
            string savePath = PathHelper.GetSavePath(shortname, importType) + "Materials/" + materialName + ".mat";
            AssetDatabase.CreateAsset(newMaterial, savePath);
            return newMaterial;
        }
    }
}