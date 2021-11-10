using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lantern.EQ;
using Lantern.EQ.Animation;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Lantern.Editor.Importers
{
    public static class MeshImporter
    {
        public static GameObject Import(string zoneName, string assetName, Transform[] bones, Matrix4x4[] poses,
            AssetImportType assetImportType, bool isCollisionMesh, out List<Material[]> materialsInMesh,
            bool createPrefab, Action<GameObject> postProcess = null)
        {
            materialsInMesh = null;
            string savePath = PathHelper.GetSavePath(zoneName, assetImportType);
            string loadPath = PathHelper.GetLoadPath(zoneName, assetImportType);
            string folderPath = PathHelper.GetSystemPathFromUnity(savePath);
            Directory.CreateDirectory(folderPath);
            Directory.CreateDirectory($"{folderPath}Meshes/");
            
            string assetPath = loadPath + $"Meshes/{assetName}.txt";

            if (!ImportHelper.LoadTextAsset(assetPath, out var zoneModelAsset))
            {
                if(!isCollisionMesh)
                {
                    Debug.LogError("MeshImporter: Unable to load text asset at: " + assetPath);
                }
                
                return null;
            }

            var modelLines = TextParser.ParseTextByDelimitedLines(zoneModelAsset, ',');

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<List<Vector3>> animatedVertices = new List<List<Vector3>>();
            Dictionary<int, List<Vector3Int>> indices = new Dictionary<int, List<Vector3Int>>();
            List<IndexPair> newIndices = new List<IndexPair>();
            List<BoneWeight> boneWeights = new List<BoneWeight>();
            
            int highestIndex = 0;
            string materialListName = string.Empty;
            int animationDelayMs = 0;
            
            foreach (List<string> line in modelLines)
            {
                if (line.Count == 0)
                {
                    continue;
                }

                switch (line[0])
                {
                    case "ml":
                        materialListName = line[1];
                        break;
                    case "v":
                        vertices.Add(new Vector3(Convert.ToSingle(line[1]), Convert.ToSingle(line[2]),
                            Convert.ToSingle(line[3])));
                        break;
                    case "uv":
                        uvs.Add(new Vector2(Convert.ToSingle(line[1]), Convert.ToSingle(line[2])));
                        break;
                    case "n":
                        normals.Add(new Vector3(Convert.ToSingle(line[1]), Convert.ToSingle(line[3]),
                            Convert.ToSingle(line[2])));
                        break;
                    case "c":
                        colors.Add(new Vector4(Convert.ToSingle(line[3]) / 255f, Convert.ToSingle(line[2]) / 255f,
                            Convert.ToSingle(line[1]) / 255f, Convert.ToSingle(line[4]) / 255f));
                        break;
                    case "i":
                    {
                        int index = Convert.ToInt32(line[1]);

                        if (index > highestIndex)
                        {
                            highestIndex = index;
                        }

                        if (!indices.ContainsKey(index))
                        {
                            indices[index] = new List<Vector3Int>();
                            newIndices.Add(new IndexPair {Index = index, Values = new List<Vector3Int>()});
                        }

                        int vertex1 = Convert.ToInt32(line[2]);
                        int vertex2 = Convert.ToInt32(line[3]);
                        int vertex3 = Convert.ToInt32(line[4]);
                        indices[index].Add(new Vector3Int(vertex1, vertex2, vertex3));

                        foreach (var indexCluster in newIndices)
                        {
                            if (indexCluster.Index == index)
                            {
                                indexCluster.Values.Add(new Vector3Int(vertex1, vertex2, vertex3));
                            }
                        }
                        
                        break;
                    }

                    case "av":
                    {
                        int index = Convert.ToInt32(line[1]);
                        float x = Convert.ToSingle(line[2]);
                        float y = Convert.ToSingle(line[3]);
                        float z = Convert.ToSingle(line[4]);

                        if (animatedVertices.Count <= index)
                        {
                            animatedVertices.Add(new List<Vector3>());
                        }

                        animatedVertices[index].Add(new Vector3(x, y, z));

                        break;
                    }

                    case "ad":
                    {
                        animationDelayMs = Convert.ToInt32(line[1]);
                        break;
                    }

                    case "b":
                    {
                        int index = Convert.ToInt32(line[1]);
                        int start = Convert.ToInt32(line[2]);
                        int count = Convert.ToInt32(line[3]);

                        for (int i = start; i < start + count; ++i)
                        {
                            boneWeights.Add(new BoneWeight {weight0 = 1f, boneIndex0 = index});
                        }

                        break;
                    }
                }
            }

            List<int> usedIndices = new List<int>();
            List<int> originalIndices = new List<int>();
            usedIndices.Sort();

            foreach (var index in indices)
            {
                usedIndices.Add(index.Key);
            }

            Material[] materials = null;
            List<Material[]> allMaterials = null;
            List<AnimatedMaterial> animatedMaterials = new List<AnimatedMaterial>();

            if (materialListName != string.Empty)
            {
                string assetMaterialPath = loadPath + $"MaterialLists/{materialListName}.txt";
                allMaterials = MaterialImporter.LoadAllMaterialsForList(zoneName, assetMaterialPath, savePath,
                    assetImportType, animatedMaterials);

                if (allMaterials != null && allMaterials.Count != 0)
                {
                    materials = allMaterials[0];
                }

                materialsInMesh = allMaterials;
            }


            foreach (var indexPair in newIndices)
            {
                originalIndices.Add(indexPair.Index);
            }

            if (allMaterials != null && allMaterials.Count != 0)
            {
                for (int i = 0; i < allMaterials[0].Length; ++i)
                {
                    bool found = false;

                    foreach (var index in originalIndices)
                    {
                        if (index == i)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        for (var index = 0; index < newIndices.Count; index++)
                        {
                            var newIndex = newIndices[index];
                            if (originalIndices[index] > i)
                            {
                                newIndex.Index--;
                            }
                        }
                    }
                }
            }


            newIndices = newIndices.OrderBy(x => x.Index).ToList();

            // Is this needed?
            highestIndex++;

            Mesh mesh = null;

            // Empty collision meshes
            if (vertices.Count == 0)
            {
                mesh = new Mesh();
                AssetDatabase.CreateAsset(mesh, savePath + "Meshes/" + assetName + ".asset");
                return null;
            }

            mesh = CreateMesh(savePath, assetName, vertices, uvs, normals, colors,
                newIndices, highestIndex, boneWeights.ToArray(), poses?.ToArray());

            GameObject returnObject;

            if (boneWeights.Count == 0)
            {
                returnObject = CreateMeshAsset(zoneName, savePath, assetName, mesh, animatedVertices,
                    animationDelayMs, materials, assetImportType, isCollisionMesh, usedIndices, createPrefab,
                    animatedMaterials, postProcess);
            }
            else
            {
                returnObject = CreateSkinnedMeshAsset(savePath, assetName, mesh, allMaterials, boneWeights, bones, poses, createPrefab, usedIndices);
            }
            
            return returnObject;
        }

        private static GameObject CreateMeshAsset(string shortname, string savePath, string assetName, Mesh mesh,
            List<List<Vector3>> animatedVertices, int animationDelay, Material[] materials,
            AssetImportType assetImportType, bool isCollisionMesh, List<int> usedIndices, bool createPrefab,
            List<AnimatedMaterial> animatedMaterials, Action<GameObject> postProcess)
        {
            if (isCollisionMesh)
            {
                return null;
            }

            GameObject newObject = new GameObject(assetName);

            if (assetImportType == AssetImportType.Characters)
            {
                var usedIndicesScript = newObject.AddComponent<UsedIndices>();
                usedIndices.Sort();
                usedIndicesScript.Indices = usedIndices;
            }

            MeshFilter meshFilter = newObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            MeshRenderer meshRenderer = null;
            if (materials != null)
            {
                meshRenderer = newObject.AddComponent<MeshRenderer>();

                Material[] newMaterials = new Material[materials.Length];
                List<Material> newNewMaterials = new List<Material>();

                for (var i = 0; i < materials.Length; i++)
                {
                    var material = materials[i];

                    if (usedIndices.Contains(i))
                    {
                        newMaterials[i] = material;
                        newNewMaterials.Add(material);
                    }
                }

                meshRenderer.sharedMaterials = newNewMaterials.ToArray();
            }

            // Equipment animation
            if (meshRenderer != null && animatedMaterials != null)
            {
                List<AnimatedMaterial> am = new List<AnimatedMaterial>();
                TextureAnimation eta = null;

                for (int i = 0; i < meshRenderer.sharedMaterials.Length; ++i)
                {
                    var material = meshRenderer.sharedMaterials[i];

                    var matchingMaterial = animatedMaterials.FirstOrDefault(x => x.Material == material);

                    if (matchingMaterial == null)
                    {
                        continue;
                    }

                    if (eta == null)
                    {
                        eta = newObject.AddComponent<TextureAnimation>();
                    }

                    eta.AddInstance(matchingMaterial, i);
                }
            }


            List<Mesh> animatedMeshes = new List<Mesh>();

            for (var i = 0; i < animatedVertices.Count; i++)
            {
                List<Vector3> animationFrames = animatedVertices[i];
                animatedMeshes.Add(CopyAndSaveMesh(mesh,
                    savePath + "Meshes/" + assetName + "_" + i + ".asset",
                    animationFrames));
            }

            if (animatedMeshes.Count != 0)
            {
                MeshAnimatedVertices meshSetter = newObject.AddComponent<MeshAnimatedVertices>();
                meshSetter.SetData(animatedMeshes, animationDelay);
            }

            MeshCollider meshCollider = null;
            if (assetImportType == AssetImportType.Zone || assetImportType == AssetImportType.Objects)
            {
                meshCollider = newObject.AddComponent<MeshCollider>();
            }

            // Look for collision mesh
            var collisionMeshPath = PathHelper.GetSavePath(shortname, assetImportType) + "Meshes/" + assetName +
                                    "_collision" + ".asset";

            var collisionMesh = AssetDatabase.LoadAssetAtPath<Mesh>(collisionMeshPath);

            if (assetImportType == AssetImportType.Zone && meshCollider != null)
            {
                var collisionObject = new GameObject("collision");
                collisionObject.transform.parent = newObject.transform;
                var collisionObjectCollider = collisionObject.AddComponent<MeshCollider>();
                collisionObjectCollider.sharedMesh = collisionMesh == null ? meshCollider.sharedMesh : collisionMesh;
                collisionObject.layer = LanternLayers.Zone;

                var raycastObject = new GameObject("raycast");
                raycastObject.transform.parent = newObject.transform;
                var raycastObjectCollider = raycastObject.AddComponent<MeshCollider>();
                raycastObjectCollider.sharedMesh = meshCollider.sharedMesh;
                raycastObject.layer = LanternLayers.ZoneRaycast;
                raycastObject.tag = LanternTags.ZoneRaycast;
            }

            if (meshCollider != null && collisionMesh != null)
            {
                meshCollider.sharedMesh = collisionMesh;
            }

            if (createPrefab)
            {
                postProcess?.Invoke(newObject);
                PrefabUtility.SaveAsPrefabAsset(newObject, savePath + assetName + ".prefab");
                AssetDatabase.Refresh();
                Object.DestroyImmediate(newObject);
                return null;
            }

            return newObject;
        }
        
        private static GameObject CreateSkinnedMeshAsset(string savePath, string assetName, Mesh mesh,
            List<Material[]> materials, List<BoneWeight> boneWeights, Transform[] bones, Matrix4x4[] poses, bool createPrefab, List<int> usedIndices)
        {
            GameObject newObject = new GameObject(assetName);

            var usedIndicesScript = newObject.AddComponent<UsedIndices>();
            usedIndices.Sort();
            usedIndicesScript.Indices = usedIndices;

            SkinnedMeshRenderer meshRenderer = newObject.AddComponent<SkinnedMeshRenderer>();

            Material[] newMaterials = new Material[materials[0].Length];
            List<Material> newNewMaterials = new List<Material>();

            for (var i = 0; i < materials[0].Length; i++)
            {
                var material = materials[0][i];

                if (usedIndices.Contains(i))
                {
                    newMaterials[i] = material;
                    newNewMaterials.Add(material);
                }
            }

            meshRenderer.sharedMaterials = newNewMaterials.ToArray();
            meshRenderer.sharedMesh = mesh;
            meshRenderer.sharedMesh.bindposes = poses.ToArray();
            meshRenderer.sharedMesh.boneWeights = boneWeights.ToArray();
            meshRenderer.rootBone = bones[0];
            meshRenderer.bones = bones;

            if (!createPrefab)
            {
                return newObject;
            }
            
            PrefabUtility.SaveAsPrefabAsset(newObject, savePath + assetName + ".prefab");
            AssetDatabase.Refresh();
            Object.DestroyImmediate(newObject);
            return null;
        }

        private static Mesh CreateMesh(string savePath, string assetName, List<Vector3> vertices,
            List<Vector2> uvs, List<Vector3> normals, List<Color> colors, List<IndexPair> newIndices,
            int submeshCount, BoneWeight[] boneWeights, Matrix4x4[] poses)
        {
            var mesh = new Mesh();
            mesh.SetVertices(vertices);

            if (uvs != null)
            {
                mesh.SetUVs(0, uvs);
            }

            if (normals != null)
            {
                mesh.SetNormals(normals);
            }

            if (colors != null)
            {
                mesh.SetColors(colors);
            }

            int totalIndexCount = 0;

            foreach (var index in newIndices)
            {
                totalIndexCount += index.Values.Count * 3;
            }

            mesh.indexFormat = totalIndexCount >= UInt16.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
            mesh.subMeshCount = submeshCount;

            foreach (var indexList in newIndices)
            {
                int submeshIndex = indexList.Index;

                List<int> submeshIndices = new List<int>();

                foreach (var index in indexList.Values)
                {
                    submeshIndices.Add(index.x);
                    submeshIndices.Add(index.y);
                    submeshIndices.Add(index.z);
                }

                if (submeshIndex >= mesh.subMeshCount)
                {
                    continue;
                }

                if (submeshIndices.Max() >= vertices.Count)
                {
                    continue;   
                }

                mesh.SetTriangles(submeshIndices, submeshIndex);
            }

            mesh.boneWeights = boneWeights;
            mesh.bindposes = poses;

            AssetDatabase.CreateAsset(mesh, savePath + "Meshes/" + assetName + ".asset");

            return mesh;
        }

        private class IndexPair
        {
            public int Index;
            public List<Vector3Int> Values = new List<Vector3Int>();
        }

        private static Mesh CopyAndSaveMesh(Mesh oldMesh, string savePath, List<Vector3> animationFrames)
        {
            var newMesh = new Mesh
            {
                vertices = animationFrames.ToArray(),
                triangles = oldMesh.triangles,
                uv = oldMesh.uv,
                normals = oldMesh.normals,
                colors = oldMesh.colors,
                tangents = oldMesh.tangents,
                subMeshCount = oldMesh.subMeshCount,
                indexFormat = oldMesh.indexFormat
            };

            for (int i = 0; i < newMesh.subMeshCount; ++i)
            {
                newMesh.SetIndices(oldMesh.GetIndices(i), MeshTopology.Triangles, i);
            }

            AssetDatabase.CreateAsset(newMesh, savePath);
            return newMesh;
        }
    }
}