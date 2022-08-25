using System;
using System.Collections.Generic;
using Lantern.Helpers;
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
                ImportHelper.LoadTextAsset(loadPath, out var vertexColors);

                if (vertexColors == null)
                {
                    continue;
                }
                
                var lines = TextParser.ParseTextByDelimitedLines(vertexColors, ',');

                if (lines == null)
                {
                    Debug.LogError($"LINES ARE NULL? VERIFY ON PC for: {vcIndex}");
                    continue;
                }
                
                List<Color> colors = new List<Color>();
            
                foreach (var line in lines)
                {
                    float r = Convert.ToInt32(line[0]) / 255.0f;
                    float g = Convert.ToInt32(line[1]) / 255.0f;
                    float b = Convert.ToInt32(line[2]) / 255.0f;
                    float a = Convert.ToInt32(line[3]) / 255.0f;
            
                    colors.Add(new Color(r, g, b, a));
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
                    Dictionary<string, ObjectData.BoundingInfo> gameObjects = new Dictionary<string, ObjectData.BoundingInfo>();

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