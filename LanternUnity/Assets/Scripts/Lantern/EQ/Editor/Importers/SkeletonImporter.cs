using System;
using System.Collections.Generic;
using Lantern.EQ;
using Lantern.EQ.Animation;
using Lantern.Helpers;
using Lantern.Logic;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class SkeletonImporter
    {
        public static void Import(string assetName, string shortName, AssetImportType importType,
            Action<GameObject> postProcess = null)
        {
            var filePath = PathHelper.GetLoadPath(shortName, importType) + "Skeletons/" + assetName + ".txt";
            TextAsset skeletonAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);

            if (skeletonAsset == null)
            {
                Debug.LogError($"SkeletonImporter: Cannot load skeleton text asset at: {filePath}");
                return;
            }

            List<List<string>> skeletonLines = TextParser.ParseTextByDelimitedLines(skeletonAsset.text, ',');
            List<string> meshesToCreate = new List<string>();

            if (skeletonLines.Count == 0 || skeletonLines[0].Count == 0)
            {
                Debug.LogError($"SkeletonImporter: Invalid skeleton text asset for: {assetName}");
                return;
            }
            
            if (skeletonLines[0][0] == "meshes")
            {
                for (int i = 1; i < skeletonLines[0].Count; ++i)
                {
                    meshesToCreate.Add(skeletonLines[0][i]);
                }

                skeletonLines.RemoveAt(0);
            }

            var skeletonRoot = new GameObject(assetName);
            var baseAnimation = skeletonRoot.AddComponent<Animation>();
            baseAnimation.cullingType = AnimationCullingType.BasedOnRenderers;

            var bones = new Transform[skeletonLines.Count];
            var poses = new Matrix4x4[skeletonLines.Count];

            // Create skeleton hierarchy
            CreateSkeletonBone(0, skeletonLines, skeletonRoot.transform, bones, poses, skeletonRoot.transform,
                shortName, importType);

            var path = PathHelper.GetLoadPath(shortName, importType) + $"Animations/{assetName}_pos.txt";
            AnimationClip animClip =
                AnimationImporter.CreateDefaultAnimations(assetName, path, importType, shortName, false);

            string clipName = animClip.name;
            baseAnimation.AddClip(animClip, animClip.name);

            // Set skeleton into default pose
            baseAnimation[clipName].enabled = true;
            baseAnimation[clipName].weight = 1.0f;
            baseAnimation.Sample();
            baseAnimation[clipName].enabled = false;

            // LANTERN ONLY START
            if (importType == AssetImportType.Objects)
            {
                var vertexColorDebug = skeletonRoot.AddComponent<VertexColorSetterNew>();
                vertexColorDebug.FindMeshFilters();
                skeletonRoot.AddComponent<AnimatedObject>().AddAnimationClip(animClip);
            }

            VariantHandler handler = null;

            if (importType == AssetImportType.Characters)
            {
                if (RaceHelper.IsPlayableRace(assetName))
                {
                    handler = skeletonRoot.AddComponent<Equipment2dHandler>();
                }
                else
                {
                    handler = skeletonRoot.AddComponent<NonPlayableVariantHandler>();
                }
            }
            // LANTERN ONLY END

            if (assetName == "ivm")
            {
                meshesToCreate.Add("elm");
                meshesToCreate.Add("elmhe00");
            }
            
            // Create skinned meshes
            for (int i = 0; i < meshesToCreate.Count; i++)
            {
                string meshName = meshesToCreate[i];
                var go = MeshImporter.Import(shortName, meshName, bones, poses, importType, false, out var moose,
                    false);

                if (go == null)
                {
                    Debug.LogError($"SkeletonImporter: Cannot create skinned mesh: {meshName}");
                    continue;
                }

                go.transform.parent = skeletonRoot.transform;
                
                if (assetName == "ivm")
                {
                    go.GetComponent<Renderer>().renderingLayerMask = 0;
                }

                // LANTERN ONLY START
                if(handler != null)
                {
                    if (meshName == assetName || meshName.EndsWith("00"))
                    {
                        handler.AddPrimaryMesh(go);
                    }
                    else
                    {
                        handler.AddSecondaryMesh(go);
                        go.SetActive(false);
                    }
                }

                if (i == 0)
                {
                    (handler as NonPlayableVariantHandler)?.SetAdditionalMaterials(moose);
                }
                // LANTERN ONLY END
            }

            postProcess?.Invoke(skeletonRoot);

            var savePath = PathHelper.GetSavePath(shortName, importType);
            PrefabUtility.SaveAsPrefabAsset(skeletonRoot, savePath + assetName + ".prefab");
            AssetDatabase.Refresh();
            UnityEngine.Object.DestroyImmediate(skeletonRoot);
        }

        private static void CreateSkeletonBone(int boneIndex, List<List<string>> skeletonLines, Transform parent,
            Transform[] bones, Matrix4x4[] poses, Transform root, string zoneName, AssetImportType importType)
        {
            List<string> boneData = skeletonLines[boneIndex];

            GameObject newBone;

            // Spawn bones with attached meshes
            if (boneData.Count != 2)
            {
                var meshName = boneData[2];
                
                if (!string.IsNullOrEmpty(meshName))
                {
                    var collision = MeshImporter.Import(zoneName, meshName + "_collision", null, null, importType, true,
                        out _, false);
                    newBone = MeshImporter.Import(zoneName, meshName, null, null, importType, false, out _, false,
                        o =>
                        {
                            // Copy collision to new mesh
                            if (collision != null)
                            {
                                var col = collision.GetComponent<MeshCollider>();
                                var col2 = o.GetComponent<MeshCollider>();
                                col2.sharedMesh = col.sharedMesh;
                            }
                        });

                    if (newBone == null)
                    {
                        Debug.LogError($"SkeletonImporter: Could not create bone mesh: {meshName}");
                        newBone = new GameObject(boneData[0]);
                    }
                    else
                    {
                        newBone.name = boneData[0];
                    }
                }
                else
                {
                    newBone = new GameObject(boneData[0]);
                }
            }
            else
            {
                newBone = new GameObject(boneData[0]);
            }

            newBone.transform.parent = parent;
            bones[boneIndex] = newBone.transform;

            poses[boneIndex] = bones[boneIndex].worldToLocalMatrix * root.localToWorldMatrix;

            if (boneData.Count == 1)
            {
                return;
            }

            List<string> childBones = TextParser.ParseStringToList(boneData[1]);

            foreach (var childBoneIndex in childBones)
            {
                CreateSkeletonBone(Convert.ToInt32(childBoneIndex), skeletonLines, newBone.transform, bones, poses,
                    root, zoneName, importType);
            }
        }
    }
}