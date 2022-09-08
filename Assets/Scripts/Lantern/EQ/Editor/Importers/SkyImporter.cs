using System.Collections.Generic;
using System.IO;
using Lantern.Editor.Importers;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Environment;
using Lantern.EQ.Helpers;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public static class SkyImporter
    {
        [MenuItem("EQ/Import/Sky", false, 50)]
        public static void ImportSky()
        {
            if (!EditorUtility.DisplayDialog("Import Sky",
                "Are you sure you want to import the sky?", "Yes", "No"))
            {
                return;
            }

            var startTime = EditorApplication.timeSinceStartup;

            var meshesToCreate = new List<string>
            {
                "layer11",
                "layer13",
                "layer21",
                "layer23",
                "layer31",
                "layer33",
                "layer41",
                "layer43",
                "layer51",
                "layer53",
                "moon",
                "moon31",
                "moon32",
                "moon33",
                "moon34",
                "moon35",
                "sun",
            };

            foreach (var mtc in meshesToCreate)
            {
                ActorStaticImporter.Import("sky", mtc, AssetImportType.Sky);
            }

            // Assembly
            var root = new GameObject("Sky");
            var group1 = GameObjectHelper.CreateNewGameObjectAsChild("Group1", root);
            var group2 = GameObjectHelper.CreateNewGameObjectAsChild("Group2", root);
            var group1Objects = GameObjectHelper.CreateNewGameObjectAsChild("Objects", group1);
            var group2Objects = GameObjectHelper.CreateNewGameObjectAsChild("Objects", group2);
            var sky1 = GameObjectHelper.CreateNewGameObjectAsChild("Sky1", group1);
            var sky2 = GameObjectHelper.CreateNewGameObjectAsChild("Sky2", group1);
            var sky3 = GameObjectHelper.CreateNewGameObjectAsChild("Sky3", group2);
            var sky4 = GameObjectHelper.CreateNewGameObjectAsChild("Sky4", group1);
            var sky5 = GameObjectHelper.CreateNewGameObjectAsChild("Sky5", group1);
            var layer12 = GameObjectHelper.CreateNewGameObjectAsChild("root", group1Objects);
            var layer32 = GameObjectHelper.CreateNewGameObjectAsChild("root", group2Objects);
            InstantiateSkyPrefabAsChild("moon", layer12);
            InstantiateSkyPrefabAsChild("sun", layer12);
            InstantiateSkyPrefabAsChild("moon35", layer32);
            InstantiateSkyPrefabAsChild("moon33", layer32);
            InstantiateSkyPrefabAsChild("moon31", layer32);
            InstantiateSkyPrefabAsChild("moon32", layer32);
            InstantiateSkyPrefabAsChild("moon34", layer32);
            var layer11 = InstantiateSkyPrefabAsChild("layer11", sky1);
            var layer13 = InstantiateSkyPrefabAsChild("layer13", sky1);
            var layer21 = InstantiateSkyPrefabAsChild("layer21", sky2);
            var layer23 = InstantiateSkyPrefabAsChild("layer23", sky2);
            var layer31 = InstantiateSkyPrefabAsChild("layer31", sky3);
            var layer33 = InstantiateSkyPrefabAsChild("layer33", sky3);
            var layer41 = InstantiateSkyPrefabAsChild("layer41", sky4);
            var layer43 = InstantiateSkyPrefabAsChild("layer43", sky4);
            var layer51 = InstantiateSkyPrefabAsChild("layer51", sky5);
            var layer53 = InstantiateSkyPrefabAsChild("layer53", sky5);

            // Animations
            var group1Anim = group1Objects.AddComponent<UnityEngine.Animation>();
            var group2Anim = group2Objects.AddComponent<UnityEngine.Animation>();
            var path = Path.Combine(PathHelper.GetEqAssetPath(), "Sky/Animations/");
            var clip1 = AnimationImporter.CreateDefaultAnimations("layer12", Path.Combine(path, $"layer12_pos.txt"),
                AssetImportType.Sky, "sky", false);
            var clip2 = AnimationImporter.CreateDefaultAnimations("layer32", Path.Combine(path, $"layer32_pos.txt"),
                AssetImportType.Sky, "sky", false);
            group1Anim.clip = clip1;
            group2Anim.clip = clip2;
            group1Anim.wrapMode = group2Anim.wrapMode = WrapMode.Loop;
            group1Anim.playAutomatically = group2Anim.playAutomatically = false;

            // Controller script
            var sc = root.AddComponent<SkyController>();
            sc.AddSkyGroup(new List<GameObject>(), null);
            sc.AddSkyGroup(new List<GameObject> { sky1, group1 }, group1Anim);
            sc.AddSkyGroup(new List<GameObject> { sky2, group1 }, group1Anim);
            sc.AddSkyGroup(new List<GameObject> { sky3, group2 }, group2Anim);
            sc.AddSkyGroup(new List<GameObject> { sky4, group1 }, group1Anim);
            sc.AddSkyGroup(new List<GameObject> { sky5, group1 }, group1Anim);

            // Layer movement
            layer11.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.008333333f, 0f), SkyLayer.SkyLayerType.SkyLayer);
            layer13.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.05f, 0f), SkyLayer.SkyLayerType.CloudLayer);
            layer21.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.008333333f, 0f), SkyLayer.SkyLayerType.SkyLayer);
            layer23.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.025f, 0f), SkyLayer.SkyLayerType.CloudLayer);
            layer31.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.025f, 0f), SkyLayer.SkyLayerType.SkyLayer);
            layer33.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.05f, 0f), SkyLayer.SkyLayerType.CloudLayer);
            layer41.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.008333333f, 0f), SkyLayer.SkyLayerType.SkyLayer);
            layer43.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.025f, 0f), SkyLayer.SkyLayerType.CloudLayer);
            layer51.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.025f, 0f), SkyLayer.SkyLayerType.SkyLayer);
            layer53.AddComponent<SkyLayer>().SetSkyLayerData(new Vector2(-0.05f, 0f), SkyLayer.SkyLayerType.CloudLayer);

            var savePath = Path.Combine(PathHelper.GetAssetBundleContentPath(), "Sky/Sky.prefab");
            PrefabUtility.SaveAsPrefabAsset(root, savePath);
            AssetDatabase.Refresh();
            Object.DestroyImmediate(root);

            ImportHelper.TagAllAssetsForBundles(PathHelper.GetAssetBundleContentPath()+ "Sky", "sky");
            EditorUtility.DisplayDialog("SkyImport",
                $"Sky import finished in {(int)(EditorApplication.timeSinceStartup - startTime)} seconds", "OK");
        }

        private static GameObject InstantiateSkyPrefabAsChild(string prefabName, GameObject parent)
        {
            var prefabPath = Path.Combine(PathHelper.GetAssetBundleContentPath(), $"Sky/{prefabName}.prefab");
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (asset == null)
            {
                Debug.LogError("Unable to load prefab at path: " + prefabPath);
                return null;
            }

            return PrefabUtility.InstantiatePrefab(asset, parent.transform) as GameObject;
        }
    }
}
