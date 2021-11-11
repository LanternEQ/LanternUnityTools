using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class SkyImporter
    {
        private static GameObject _skyObject;

        private static Transform _sky1Root;
        private static Transform _sky2Root;
        
        private static Transform _sky1ObjectRoot;
        private static Transform _sky2ObjectRoot;

        private static Animation _sky1Animation;
        private static Animation _sky2Animation;

        [MenuItem("EQ/Import/Sky", false, 50)]
        public static void ImportSky()
        {
            if (!EditorUtility.DisplayDialog("Import Sky",
                "Are you sure you want to import the sky?", "Yes", "No"))
            {
                return;
            }
            
            CreateSkyMaterials();
            CreateSkyMeshes();
            CreateSkySkeletonAndAnimation("layer12");
            CreateSkySkeletonAndAnimation("layer32");
            CreateHierarchy();
        }

        private static void CreateHierarchy()
        {
            /*GameObject root = new GameObject("ZoneEnvironment");
            //root.transform.localScale = new Vector3(100000f, 100000f, 100000f);
            var skyController = root.AddComponent<SkyController>();
            root.AddComponent<FogController>();
            root.AddComponent<WorldLightController>();

            var skyGroup1Root = InstantiateSkyPrefab("layer12", root);
            skyGroup1Root.name = "Root1";

            // Sky 1
            var sky1Parent = new GameObject("Sky1");
            sky1Parent.transform.parent = skyGroup1Root.transform;
            var sky1SkyLayer = InstantiateSkyPrefab("layer11", sky1Parent);
            var sky1CloudLayer = InstantiateSkyPrefab("layer13", sky1Parent);

            // Sky 2
            var sky2Parent = new GameObject("Sky2");
            sky2Parent.transform.parent = skyGroup1Root.transform;
            var sky2SkyLayer = InstantiateSkyPrefab("layer21", sky2Parent);
            var sky2CloudLayer = InstantiateSkyPrefab("layer23", sky2Parent);

            // Sky 4
            var sky4Parent = new GameObject("Sky4");
            sky4Parent.transform.parent = skyGroup1Root.transform;
            var sky4SkyLayer = InstantiateSkyPrefab("layer41", sky4Parent);
            var sky4CloudLayer = InstantiateSkyPrefab("layer43", sky4Parent);
            
            // Sky 5
            var sky5Parent = new GameObject("Sky5");
            sky5Parent.transform.parent = skyGroup1Root.transform;
            var sky5SkyLayer = InstantiateSkyPrefab("layer51", sky5Parent);
            var sky5CloudLayer = InstantiateSkyPrefab("layer53", sky5Parent);
            
            var skyGroup2Root = InstantiateSkyPrefab("layer32", root);
            skyGroup2Root.name = "Root2";
            
            // Sky 3
            var sky3Parent = new GameObject("Sky3");
            sky3Parent.transform.parent = skyGroup2Root.transform;
            var sky3SkyLayer = InstantiateSkyPrefab("layer31", sky3Parent);
            var sky3CloudLayer = InstantiateSkyPrefab("layer33", sky3Parent);
            
            skyController.AddSkyRoot(skyGroup1Root.transform);
            skyController.AddSkyRoot(skyGroup2Root.transform);

            var root1Objects = skyGroup1Root.transform.GetChild(0).gameObject;
            var root2Objects = skyGroup2Root.transform.GetChild(0).gameObject;

            var root1Animation = root1Objects.GetComponent<Animation>();
            var root2Animation = root2Objects.GetComponent<Animation>();
            root1Animation.playAutomatically = false;
            root2Animation.playAutomatically = false;
            
            root1Objects.name = root2Objects.name = "Objects";
            
            skyController.SetObjectPool(new List<GameObject>
            {
                sky1Parent, sky2Parent, sky3Parent, sky4Parent, sky5Parent, root1Objects, root2Objects
            });

            skyController.AddSkyGroup(new List<GameObject>(),null);
            skyController.AddSkyGroup(new List<GameObject> {root1Objects, sky1Parent},
                root1Animation);
            skyController.AddSkyGroup(new List<GameObject> {root1Objects, sky2Parent},
                root1Animation);
            skyController.AddSkyGroup(new List<GameObject> {root2Objects, sky3Parent},
                root2Animation);
            skyController.AddSkyGroup(new List<GameObject> {root1Objects, sky4Parent},
                root1Animation);
            skyController.AddSkyGroup(new List<GameObject> {root1Objects, sky5Parent},
                root1Animation);

            SetupLayerPanningLogic(new List<GameObject>
            {
                sky1CloudLayer,
                sky1SkyLayer,
                sky2CloudLayer,
                sky2SkyLayer,
                sky3CloudLayer,
                sky3SkyLayer,
                sky4CloudLayer,
                sky4SkyLayer,
                sky5CloudLayer,
                sky5SkyLayer,
            });
            
            root.transform.localScale = new Vector3(100f, 100f, 100f);*/
        }

        private static GameObject InstantiateSkyPrefab(string layer12, GameObject root)
        {
            var skeleton1 =
                AssetDatabase.LoadAssetAtPath<GameObject>(PathHelper.GetSavePath("sky", AssetImportType.Sky) +
                                                          layer12 + ".prefab");
            
            return PrefabUtility.InstantiatePrefab(skeleton1, root.transform) as GameObject;
        }

        private static void CreateSkySkeletonAndAnimation(string skeletonName)
        {
            var pathToSkeleton = $"Assets/ZoneAssets/Classic/sky/Animations/{skeletonName}_POS.txt";
            var pathToAnimation = $"Assets/ZoneAssets/Classic/sky/Skeletons/{skeletonName}.txt";

            var defaultAnim = AnimationImporter.CreateDefaultAnimations(skeletonName, pathToSkeleton, AssetImportType.Sky, "sky", false);

            if (defaultAnim == null)
            {
                return;
            }
            
            List<string> skeletalMeshes = new List<string>();

            /*GameObject skeleton =
                SkeletonImporter.Import(skeletonName, "sky",AssetImportType.Sky);
            
            PrefabUtility.SaveAsPrefabAsset(skeleton, PathHelper.GetSavePath("sky", 
                AssetImportType.Sky) + skeletonName + ".prefab");

            // Destroy
            GameObject.DestroyImmediate(skeleton);*/
        }

        private static void CreateSkyMaterials()
        {
            MaterialImporter.CreateMaterials("sky", AssetImportType.Sky, null);
        }

        private static void CreateSkyMeshes()
        {
            var meshListPath = PathHelper.GetLoadPath("sky", AssetImportType.Sky) + "meshes.txt";

            string meshListAsset = string.Empty;
            if(!ImportHelper.LoadTextAsset(meshListPath, out meshListAsset))
            {
                Debug.LogError("Unable to load sky mesh list at path:  " + meshListPath);
                return;
            }

            var meshLines = TextParser.ParseTextByNewline(meshListAsset);

            foreach (var meshName in meshLines)
            {
                List<Material[]> moose = null;
            }
        }

        private static void SetupLayerPanningLogic(List<GameObject> objects)
        {
           /* foreach(GameObject child in objects)
            {
                SkydomeLayer layer = child.gameObject.AddComponent<SkydomeLayer>();
                float xPan = 0.0f;
                float yPan = 0.0f;

                if (child.name == "layer11" || child.name == "layer21" || child.name == "layer41")
                {
                    xPan = -0.008333333f;
                }
                if (child.name == "layer13" || child.name == "layer53" || child.name == "layer33")
                {
                    xPan = -0.05f;
                }
                if (child.name == "layer23" || child.name == "layer43" || child.name == "layer51" || child.name == "layer31")
                {
                    xPan = -0.025f;
                }
                
                layer.SetUvPanSpeed(new Vector2(xPan, yPan));
            }*/
        }
    }
}