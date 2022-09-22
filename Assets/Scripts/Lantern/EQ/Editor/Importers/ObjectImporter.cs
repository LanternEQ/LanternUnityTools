using System;
using System.Collections.Generic;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Helpers;
using Lantern.EQ.Objects;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class ObjectImporter
    {
        public static void CreateObjectInstances(string shortname, string objectInstanceListPath, Transform objectsRoot, ObjectData objectData)
        {
            ImportHelper.LoadTextAsset(objectInstanceListPath, out var objectInstanceText);

            if (string.IsNullOrEmpty(objectInstanceText))
            {
                Debug.LogError("Could not load object instance asset: " + objectInstanceListPath);
                return;
            }

            var parsedObjectLines = TextParser.ParseTextByDelimitedLines(objectInstanceText, ',');

            GameObject loadedPrefab = null;
            foreach (var objectInstance in parsedObjectLines)
            {
                string objectPrefabName = objectInstance[0];

                var position = new Vector3(Convert.ToSingle(objectInstance[1]),
                    Convert.ToSingle(objectInstance[2]), Convert.ToSingle(objectInstance[3]));

                // Ignore objects that have fallen to the bottom of the world
                if (position.y < -30000)
                {
                    continue;
                }

                var rotation = Quaternion.Euler(Convert.ToSingle(objectInstance[4]),
                    Convert.ToSingle(objectInstance[5]), Convert.ToSingle(objectInstance[6]));
                var scale = new Vector3(Convert.ToSingle(objectInstance[7]),
                    Convert.ToSingle(objectInstance[8]), Convert.ToSingle(objectInstance[9]));

                int vcIndex = Convert.ToInt32(objectInstance[10]);
                var loadPath = PathHelper.GetLoadPath(shortname, AssetImportType.Objects) + "VertexColors/vc_" +
                               vcIndex + ".txt";

                List<Color> colors = new List<Color>();

                if (vcIndex != -1)
                {
                    ImportHelper.LoadTextAsset(loadPath, out var vertexColors);

                    if (vertexColors != null)
                    {
                        var lines = TextParser.ParseTextByDelimitedLines(vertexColors, ',');

                        if (lines != null)
                        {
                            foreach (var line in lines)
                            {
                                float r = Convert.ToInt32(line[0]) / (float)byte.MaxValue;
                                float g = Convert.ToInt32(line[1]) / (float)byte.MaxValue;
                                float b = Convert.ToInt32(line[2]) / (float)byte.MaxValue;
                                float a = Convert.ToInt32(line[3]) / (float)byte.MaxValue;

                                colors.Add(new Color(r, g, b, a));
                            }
                        }
                    }
                }

                if(objectsRoot != null)
                {
                    if (loadedPrefab == null || loadedPrefab.name != objectPrefabName)
                    {
                        string prefabLoadPath = PathHelper.GetSavePath(shortname, AssetImportType.Objects) + objectPrefabName + ".prefab";
                        loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabLoadPath);
                    }

                    if (loadedPrefab == null)
                    {
                        Debug.LogError("Could not load object prefab asset: " + objectPrefabName);
                        continue;
                    }

                    GameObject newObject = (GameObject) PrefabUtility.InstantiatePrefab(loadedPrefab, objectsRoot.transform);
                    newObject.transform.position = position;
                    newObject.transform.rotation = rotation;
                    newObject.transform.localScale = scale;
                }
                else
                {
                    Dictionary<string, ObjectData.ObjectBoundingInfo> gameObjects = new Dictionary<string, ObjectData.ObjectBoundingInfo>();

                    if (!gameObjects.ContainsKey(objectPrefabName))
                    {
                        string prefabLoadPath = PathHelper.GetSavePath(shortname, AssetImportType.Objects) + objectPrefabName + ".prefab";
                        loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabLoadPath);

                        if (loadedPrefab == null)
                        {
                            Debug.LogError("Could not find prefab: " + objectPrefabName);
                            continue;
                        }

                        gameObjects[objectPrefabName] = BoundsHelper.CalculateSpherePositionAndRadiusForModel(loadedPrefab);

                        objectData.AddObjectInstance(objectPrefabName, position,
                            rotation.eulerAngles,
                            scale.x, colors.Count == 0 ? null : colors, gameObjects[objectPrefabName]);
                    }
                }
            }
        }
    }
}
