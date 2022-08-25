using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class EquipmentImporter
    {
        private static Dictionary<string, AnimationClip> _animations;
        
        [MenuItem("EQ/Import/Equipment", false, 50)]
        public static void ImportEquipment()
        {
            if (!EditorUtility.DisplayDialog("Import Equipment",
                "Are you sure you want to import equipment?", "Yes", "No"))
            {
                return;
            }

            var path = PathHelper.GetRootLoadPath("equipment");
            if (!Directory.Exists(PathHelper.GetSystemPathFromUnity(path)))
            {
                Debug.LogError($"EquipmentImporter: No folder at path: {path}");
                return;
            }
            
            var startTime = EditorApplication.timeSinceStartup;
            ActorStaticImporter.ImportList("equipment", AssetImportType.Equipment, PostProcessStatic);
            ActorSkeletalImporter.ImportList("equipment", AssetImportType.Equipment, PostProcess);
            ZoneImporter.TagAllAssetsForBundles("Assets/Content/AssetsToBundle/Equipment", "equipment");
            EditorUtility.DisplayDialog("EquipmentImport",
                $"Equipment import finished in {(int) (EditorApplication.timeSinceStartup - startTime)} seconds", "OK");
        }

        private static void PostProcessStatic(GameObject obj)
        {
            if (obj == null)
            {
                return;
            }
            
            string hdAssetName = obj.name + "_HD";
            var hdPath = $"Assets/Content/HD/Equipment/{hdAssetName}.fbx";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(hdPath);

            if (mesh == null)
            {
                return;
            }
            
            Mesh newMesh = new Mesh
            {
                vertices = mesh.vertices,
                triangles = mesh.triangles,
                uv = mesh.uv,
                normals = mesh.normals,
                colors = mesh.colors.ToArray(),
                tangents = mesh.tangents,
                subMeshCount = mesh.subMeshCount,
                indexFormat = mesh.indexFormat,
            };

            var oldmr = obj.GetComponent<MeshRenderer>();
            if (oldmr == null)
            {
                return;
            }
            
            var wholeObject = AssetDatabase.LoadAssetAtPath<GameObject>(hdPath);
            if (wholeObject == null)
            {
                return;
            }
            
            var mr = wholeObject.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                return;
            }
            
            Dictionary<int, int> remap = new Dictionary<int, int>();

            for (var i = 0; i < oldmr.sharedMaterials.Length; i++)
            {
                var material = oldmr.sharedMaterials[i];
                for (var j = 0; j < mr.sharedMaterials.Length;j++)
                {
                    var secondMaterial = mr.sharedMaterials[j];
                    if (material.name.Replace('-', '_') == secondMaterial.name.Split('.')[0])
                    {
                        remap[j] = i;
                    }
                }
            }

            if (remap.Count != oldmr.sharedMaterials.Length)
            {
                Debug.LogError("Cannot remap materials for: " + obj.name);
                return;
            }

            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                newMesh.SetIndices(mesh.GetIndices(i), mesh.GetTopology(i), remap[i]);
            }
            
            var savePath = PathHelper.GetSavePath("equipment", AssetImportType.Equipment) + $"Meshes/{hdAssetName}.asset";
            AssetDatabase.CreateAsset(newMesh, savePath);

            GameObject newPrefab = GameObject.Instantiate(obj);
            newPrefab.name += "_HD";
            var mf = newPrefab.GetComponent<MeshFilter>();
            mf.sharedMesh = newMesh;
            savePath = PathHelper.GetSavePath("equipment", AssetImportType.Equipment) + $"{hdAssetName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath);
            GameObject.DestroyImmediate(newPrefab);
        }

        private static void PostProcess(GameObject obj)
        {
            string modelAsset = obj.name;

            var animation = obj.GetComponent<Animation>();

            if (animation == null)
            {
                return;
            }
            
            // Find animations
            string rootName = modelAsset;
            if (animation != null)
            {
                CharacterImporter.GetAdditionalAnimations(animation, rootName, AssetImportType.Equipment);
            }
        }
    }
}