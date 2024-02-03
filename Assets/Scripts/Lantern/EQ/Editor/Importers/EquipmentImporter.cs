using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lantern.EQ.Animation;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Equipment;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public class EquipmentImporter : LanternEditorWindow
    {
        private static Dictionary<string, AnimationClip> _animations;

        private static readonly List<string> Text1 = new()
        {
            "This process creates equipment prefabs from intermediate EverQuest data.",
            "Importing all equipment takes around ten minutes.",
        };

        private static readonly List<string> Text2 = new()
        {
            "EverQuest equipment data must be located in:",
            "\fAssets/EQAssets/equipment/",
        };

        private static readonly List<string> Text3 = new()
        {
            "Equipment prefabs will be output to:",
            "\fAssets/Content/AssetBundleContent/Equipment/"
        };

        /// <summary>
        /// Unity relative paths to animation text files
        /// </summary>
        private static List<string> _animationPaths;

        [MenuItem("EQ/Assets/Import Equipment &e", false, 3)]
        public static void ShowImportDialog()
        {
            GetWindow<EquipmentImporter>("Import Equipment", typeof(EditorWindow));
        }

        private void OnGUI()
        {
            DrawInfoBox(Text1, "d_console.infoicon");
            DrawInfoBox(Text2, "d_Collab.FolderConflict");
            DrawInfoBox(Text3, "d_Collab.FolderMoved");
            DrawHorizontalLine();

            if (DrawButton("Import"))
            {
                ImportEquipment();
            }
        }

        private static void ImportEquipment()
        {
            _animations = new Dictionary<string, AnimationClip>();
            _animationPaths = AnimationImporter.LoadAnimationPaths("equipment", AssetImportType.Equipment);

            var path = PathHelper.GetRootLoadPath("equipment");
            if (!Directory.Exists(PathHelper.GetSystemPathFromUnity(path)))
            {
                Debug.LogError($"EquipmentImporter: No folder at path: {path}");
                return;
            }

            var startTime = EditorApplication.timeSinceStartup;
            ActorStaticImporter.ImportList("equipment", AssetImportType.Equipment, PostProcessStatic);
            ActorSkeletalImporter.ImportList("equipment", AssetImportType.Equipment, PostProcessSkeletal);
            ImportHelper.TagAllAssetsForBundles(PathHelper.GetAssetBundleContentPath()+ "Equipment", "equipment");
            EditorUtility.DisplayDialog("EquipmentImport",
                $"Equipment import finished in {(int) (EditorApplication.timeSinceStartup - startTime)} seconds", "OK");

            _animations.Clear();
            _animationPaths.Clear();
        }

        private static void PostProcessStatic(GameObject go)
        {
            if (go == null)
            {
                return;
            }

            AddModelScript(go);
            CreateHdVariant(go);
        }

        private static void AddModelScript(GameObject go)
        {
            var ea = go.AddComponent<EquipmentAnimation>();
            ea.InitializeImport();

            var em = go.AddComponent<EquipmentModel>();
            em.SetReferences(ea);
        }

        private static void CreateHdVariant(GameObject go)
        {
            string hdAssetName = go.name + "_HD";
            var hdPath = $"Assets/Content/HD/Equipment/{hdAssetName}.fbx";
            var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(hdPath);

            if (mesh == null)
            {
                // No HD model available
                return;
            }

            var newMesh = new Mesh
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

            var oldMeshRenderer = go.GetComponent<MeshRenderer>();
            if (oldMeshRenderer == null)
            {
                Debug.LogError($"EquipmentHD: No mesh renderer found on {go.name}");
                return;
            }

            var wholeObject = AssetDatabase.LoadAssetAtPath<GameObject>(hdPath);
            if (wholeObject == null)
            {
                Debug.LogError($"EquipmentHD: No game object found at path {hdPath}");
                return;
            }

            var mr = wholeObject.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                Debug.LogError($"EquipmentHD: No mesh renderer found on loaded game object {wholeObject.name}");
                return;
            }

            Dictionary<int, int> remap = new Dictionary<int, int>();

            for (var i = 0; i < oldMeshRenderer.sharedMaterials.Length; i++)
            {
                var material = oldMeshRenderer.sharedMaterials[i];
                for (var j = 0; j < mr.sharedMaterials.Length;j++)
                {
                    var secondMaterial = mr.sharedMaterials[j];
                    if (material.name.Replace('-', '_') == secondMaterial.name.Split('.')[0])
                    {
                        remap[j] = i;
                    }
                }
            }

            if (remap.Count != oldMeshRenderer.sharedMaterials.Length)
            {
                Debug.LogError($"EquipmentHD: Cannot remap materials for {go.name}. Invalid material count");
                return;
            }

            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                newMesh.SetIndices(mesh.GetIndices(i), mesh.GetTopology(i), remap[i]);
            }

            var savePath = PathHelper.GetSavePath("equipment", AssetImportType.Equipment) + $"Meshes/{hdAssetName}.asset";
            AssetDatabase.CreateAsset(newMesh, savePath);

            GameObject newPrefab = Object.Instantiate(go);
            newPrefab.name += "_HD";
            var mf = newPrefab.GetComponent<MeshFilter>();
            mf.sharedMesh = newMesh;
            savePath = PathHelper.GetSavePath("equipment", AssetImportType.Equipment) + $"{hdAssetName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(newPrefab, savePath);
            DestroyImmediate(newPrefab);
        }

        private static void PostProcessSkeletal(GameObject go)
        {
            string modelAsset = go.name;

            var animation = go.GetComponent<UnityEngine.Animation>();

            if (animation == null)
            {
                return;
            }

            // Find animations
            if (animation != null)
            {
                LoadEquipmentAnimations(animation, modelAsset, AssetImportType.Equipment);
            }

            AddModelScript(go);
        }

        private static void LoadEquipmentAnimations(UnityEngine.Animation animation, string animationBase, AssetImportType type)
        {
            string prefix = animationBase + "_";

            foreach (var path in _animationPaths)
            {
                string fileName = Path.GetFileNameWithoutExtension(path);

                if (!fileName.StartsWith(prefix))
                {
                    continue;
                }

                var animClip = AnimationImporter.GetAnimationClip("equipment", fileName, path, animationBase, type,
                    ref _animations);

                if (animClip == null)
                {
                    Debug.LogError("Unable to import animation: " + fileName);
                }

                animation.AddClip(animClip, animClip.name);

                if (type == AssetImportType.Characters)
                {
                    if (!AnimationHelper.TrySplitAnimationName(fileName, out _, out var at))
                    {
                        continue;
                    }

                    if (!AnimationHelper.IsReverseAnimationNeeded(at))
                    {
                        continue;
                    }

                    animClip = AnimationImporter.GetAnimationClip("equipment", fileName, path, animationBase, type,
                        ref _animations, true);

                    if (animClip != null)
                    {
                        animation.AddClip(animClip, animClip.name);
                    }
                }
            }
        }
    }
}
