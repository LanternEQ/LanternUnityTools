#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public class ZoneImporter : EditorWindow
    {
        /// <summary>
        /// The directory in which the script looks for zone files
        /// </summary>
        private static string _zoneAssetPath = "Assets/ZoneExports/";

        /// <summary>
        /// The name of the folder the zone in which zone assets are stored
        /// </summary>
        private static string _zoneFolderPath = "Zone/";
        
        /// <summary>
        /// The name of the folder the zone in which object assets are stored
        /// </summary>
        private static string _objectsFolderPath = "Objects/";
        
        /// <summary>
        /// The name of the folder the zone in which character assets are stored
        /// </summary>
        private static string _charactersFolderPath = "Characters/";
        
        /// <summary>
        /// The shortname of the zone that will be imported - e.g. arena, qeynos2, gfaydark
        /// </summary>
        private static string _zoneShortname;

        /// <summary>
        /// The object that is created and serves as a root for zone subgroups
        /// </summary>
        private static GameObject _zoneRoot;

        /// <summary>
        /// The root for zone objects
        /// </summary>
        private static GameObject _objectRoot;

        /// <summary>
        /// The scaling that is applied to the zone on import
        /// </summary>
        private static float _scaleFactor = 0.5f;

        /// <summary>
        /// Should zone objects be imported?
        /// </summary>
        private static bool _importZoneObjects = true;

        /// <summary>
        /// Should lights be imported?
        /// </summary>
        private static bool _importLights = true;

        /// <summary>
        /// Opens the zone importer settings window
        /// </summary>
        [MenuItem("EQ/Editor/Import Zone &z")]
        private static void ShowImportDialog()
        {
            GetWindow(typeof(ZoneImporter));
        }

        /// <summary>
        /// Draws the settings window for the zone importer
        /// </summary>
        private void OnGUI()
        {
            // Force the window size
            minSize = maxSize = new Vector2(290, 100);
            EditorGUIUtility.labelWidth = 200;

            _zoneShortname = EditorGUILayout.TextField("Zone Shortname", _zoneShortname);

            _importZoneObjects = EditorGUILayout.Toggle("Import Zone Objects", _importZoneObjects);
            _importLights = EditorGUILayout.Toggle("Import Lights", _importLights);

            EditorGUILayout.Space();

            Rect r = EditorGUILayout.BeginHorizontal("Button");
            if (GUI.Button(r, GUIContent.none))
            {
                ImportZone();
            }

            GUILayout.Label("Import");

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        /// <summary>
        /// Creates the zone 
        /// </summary>
        private static void ImportZone()
        {
            if (string.IsNullOrEmpty(_zoneAssetPath))
            {
                Debug.LogError("ZoneImporter: Zone asset path invalid or not set");
                return;
            }

            if (string.IsNullOrEmpty(_zoneShortname))
            {
                Debug.LogError("ZoneImporter: You must supply a valid shortname");
                return;
            }

            _zoneShortname = _zoneShortname.ToLower();

            DestroyPreviousZone();
            RemoveInvalidLights();
            FixLightingSettings();
            FixAllModelMaterialReferences();

            if (!ImportZoneMesh())
            {
                return;
            }

            if (_importZoneObjects)
            {
                ImportZoneObjects();
            }

            if (_importLights)
            {
                SpawnLights();
            }

            FixMaterialImportSettings(_zoneFolderPath, _zoneShortname + "_materials.txt");
            FixMaterialImportSettings(_objectsFolderPath,  _zoneShortname +"_objects_materials.txt");
            FixMaterialShaderAssignment(_zoneFolderPath, _zoneShortname + "_materials.txt");
            FixMaterialShaderAssignment(_objectsFolderPath, _zoneShortname +"_objects_materials.txt");

            RotateAndScaleZone();     

            EditorSceneManager.MarkAllScenesDirty();
        }

        /// <summary>
        /// Destroys any existing instance of a zone with the matching shortname
        /// </summary>
        private static void DestroyPreviousZone()
        {
            GameObject oldZone = GameObject.Find(_zoneShortname);

            if (oldZone == null)
            {
                return;
            }
            
            Debug.Log("ZoneImporter: Destroying old copy of: " + _zoneShortname);
            DestroyImmediate(oldZone);
        }

        /// <summary>
        /// Removes lights that aren't used (directional + spot)
        /// </summary>
        private static void RemoveInvalidLights()
        {
            Light[] lights = FindObjectsOfType<Light>();

            for (int i = lights.Length - 1; i >= 0; i--)
            {
                if (lights[i].type == LightType.Area || lights[i].type == LightType.Point)
                {
                    continue;
                }

                DestroyImmediate(lights[i].gameObject);
            }
        }

        private static void FixLightingSettings()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.skybox = null;
            RenderSettings.ambientLight = Color.white;
        }

        /// <summary>
        /// Spawns the main zone geometry
        /// </summary>
        /// <returns>Whether or not the load has failed - fatal</returns>
        private static bool ImportZoneMesh()
        {
            string zoneFolderPath = _zoneAssetPath + _zoneShortname + "/" + _zoneFolderPath;
            
            Object zoneMesh = AssetDatabase.LoadAssetAtPath(zoneFolderPath + _zoneShortname + ".obj", typeof(Object));

            if (zoneMesh == null)
            {
                Debug.LogError("ZoneImporter: Unable to find zone mesh.");
                return false;
            }

            _zoneRoot = Instantiate(zoneMesh, Vector3.zero, Quaternion.identity) as GameObject;

            if (_zoneRoot == null)
            {
                Debug.LogError("ZoneImporter: Unable to instantiate the zone mesh.");
                return false;
            }

            _zoneRoot.name = _zoneShortname;

            // Scale it to account for DirectX coordinates
            _zoneRoot.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

            // Move all sub meshes to a submesh root object
            GameObject meshRoot = new GameObject("ZoneMeshes");

            for (int i = _zoneRoot.transform.childCount; i-- > 0;)
            {
                Transform child = _zoneRoot.transform.GetChild(i);
                child.name = _zoneShortname;
                child.parent = meshRoot.transform;
            }

            meshRoot.transform.parent = _zoneRoot.transform;

            // Set the collision model
            Object collisionModel = AssetDatabase.LoadAssetAtPath(zoneFolderPath + _zoneShortname + "_collision.obj", typeof(Object));

            if (collisionModel != null)
            {
                var meshCollider = _zoneRoot.transform.GetChild(0).GetChild(0).GetComponent<MeshCollider>();

                if (meshCollider != null)
                {
                    if (collisionModel is GameObject && ((GameObject)collisionModel).transform.childCount != 0)
                    {
                        GameObject collisionObj = ((GameObject)collisionModel).transform.GetChild(0).gameObject;
                        meshCollider.sharedMesh = collisionObj.GetComponent<MeshCollider>().sharedMesh;
                    }
      
                }
            }
            else
            {
                Debug.Log("ZoneImporter: No separate collision model found");
            }


            Object waterMesh =
                AssetDatabase.LoadAssetAtPath(zoneFolderPath + _zoneShortname + "_water.obj",
                    typeof(Object));

            if (waterMesh != null)
            {
                GameObject waterObject = Instantiate(waterMesh, Vector3.zero, Quaternion.identity) as GameObject;

                if (waterObject == null)
                {
                    Debug.LogError("ZoneImporter: Unable to instantiate the water object.");
                }
                else
                {
                    if (waterObject.transform.childCount == 0)
                    {
                        DestroyImmediate(waterObject);
                    }
                    else
                    {
                        // Access the subobject
                        GameObject waterSubmesh = waterObject.transform.GetChild(0).gameObject;

                        // Fix the clone append
                        waterSubmesh.name = _zoneShortname + "_water";

                        // Reparent the object
                        waterSubmesh.transform.SetParent(meshRoot.transform);

                        // Scale it to account for DirectX coordinates
                        waterSubmesh.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                        waterSubmesh.transform.SetParent(meshRoot.transform);

                        DestroyImmediate(waterObject);
                    }
                }
            }
            else
            {
                Debug.Log("ZoneImporter: No water mesh found");
            }

            Object lavaMesh = AssetDatabase.LoadAssetAtPath(zoneFolderPath + _zoneShortname + "_lava.obj",
                typeof(Object));
            
            if (lavaMesh != null)
            {
                var lavaObject = Instantiate(lavaMesh, Vector3.zero, Quaternion.identity) as GameObject;

                if (lavaObject == null)
                {
                    Debug.LogError("ZoneImporter: Unable to instantiate the lava object.");
                }
                else
                {
                    if (lavaObject.transform.childCount == 0)
                    {
                        DestroyImmediate(lavaObject);
                    }
                    else
                    {
                        // Access the subobject
                        GameObject lavaSubmesh = lavaObject.transform.GetChild(0).gameObject;

                        // Fix the clone append
                        lavaSubmesh.name = _zoneShortname + "_lava";

                        // Reparent the object
                        lavaSubmesh.transform.SetParent(meshRoot.transform);

                        // Scale it to account for DirectX coordinates
                        lavaSubmesh.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                        lavaSubmesh.transform.SetParent(meshRoot.transform);

                        DestroyImmediate(lavaObject);
                    }
                }
            }
            else
            {
                Debug.Log("ZoneImporter: No lava mesh found");
            }

            return true;
        }

        /// <summary>
        /// Spawns the zone objects in the world
        /// </summary>
        static void ImportZoneObjects()
        {
            // Populate objects
            string assetPath = _zoneAssetPath + _zoneShortname + "/" + _objectsFolderPath + _zoneShortname + "_objects.txt";

            var objectList = (TextAsset) AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));

            if (objectList == null)
            {
                Debug.LogError("ZoneImporter: Unable to find the object list at " + assetPath);
                return;
            }

            var objectListLines = TextParser.ParseTextByDelimitedLines(objectList.text, ',','#');

            if (objectListLines == null)
            {
                Debug.LogError("ZoneImporter: Error parsing object list");
                return;
            }
            
            _objectRoot = new GameObject("ObjectMeshes");

            if (_objectRoot == null)
            {
                Debug.LogError("ZoneImporter: Unable to create the objects root");
                return;
            }

            _objectRoot.transform.parent = _zoneRoot.transform;

            foreach (var objectLine in objectListLines)
            {
                SpawnObject(objectLine);
            }
        }

        /// <summary>
        /// Creates one instance of an object
        /// </summary>
        /// <param name="objectValues">The spawn parameters of the object</param>
        static void SpawnObject(List<string> objectValues)
        {
            if (objectValues == null || objectValues.Count == 0)
            {
                return;
            }
            
            Object objectAsset =
                AssetDatabase.LoadAssetAtPath(_zoneAssetPath + _zoneShortname + "/" + _objectsFolderPath + objectValues[0] + ".obj", typeof(Object));

            if (objectAsset == null)
            {
                Debug.LogError("ZoneImporter: Unable to find the object asset: " + objectValues[0] + ".obj");
                return;
            }

            var createdObject = Instantiate(objectAsset, new Vector3((float) Convert.ToDouble(objectValues[1]),
                (float) Convert.ToDouble(objectValues[2]), (float) Convert.ToDouble(objectValues[3])), Quaternion.identity, _objectRoot.transform) as GameObject;

            FixCloneNameAppend(createdObject);
            
            if (createdObject == null)
            {
                Debug.LogError("ZoneImporter: Failed to spawn object: " + objectValues[0]);
                return;
            }
            
            createdObject.transform.localRotation = Quaternion.Euler(0.0f, Convert.ToSingle(objectValues[5]), Convert.ToSingle(objectValues[6]) + 180.0f);
            createdObject.transform.localScale = new Vector3((float) Convert.ToDouble(objectValues[7]),
                -(float) Convert.ToDouble(objectValues[8]), Convert.ToSingle(objectValues[9]));

            createdObject = FixModelParent(createdObject, _objectRoot.transform);

            if (createdObject == null)
            {
                return;
            }

            CheckForSeparateCollisionModel(createdObject, _zoneAssetPath + _zoneShortname + "/" + _objectsFolderPath + objectValues[0] + "_collision.obj"); 
        }

        /// <summary>
        /// Checks for a collision model for the spawned object and uses it as the collider instead
        /// </summary>
        /// <param name="createdObject"></param>
        /// <param name="collisionModelPath"></param>
        private static void CheckForSeparateCollisionModel(GameObject createdObject, string collisionModelPath)
        {
            // Check to see if this model has another collision model
            var collisionModel = (GameObject)
                AssetDatabase.LoadAssetAtPath(collisionModelPath, typeof(GameObject));

            if (collisionModel == null)
            {
                return;
            }
            
            var objectCollider = createdObject.GetComponent<MeshCollider>();

            if (objectCollider == null)
            {
                return;
            }
            
            if (collisionModel.transform.childCount == 0)
            {
                // The entire mesh has no faces - possible for things like steam
                objectCollider.sharedMesh = null;
            }
            else
            {
                GameObject collisionObj = collisionModel.transform.GetChild(0).gameObject;

                MeshCollider collider = collisionObj.GetComponent<MeshCollider>();
                        
                if (collider != null)
                {
                    objectCollider.sharedMesh = collider.sharedMesh;
                }
            }
        }

        /// <summary>
        /// Assigns the game object to a new parent root
        /// </summary>
        /// <param name="createdObject">The object to reassign</param>
        /// <param name="newParent">The new parent</param>
        /// <returns>A reference to the child object</returns>
        private static GameObject FixModelParent(GameObject createdObject, Transform newParent)
        {
            if (createdObject == null || createdObject.transform.childCount == 0)
            {
                return null;
            }

            string name = createdObject.name;
            
            GameObject child = createdObject.transform.GetChild(0).gameObject;
            
            child.transform.SetParent(newParent, true);
            child.name = name;

            DestroyImmediate(createdObject);

            return child;
        }

        /// <summary>
        /// Spawns the lights in the world
        /// </summary>
        static void SpawnLights()
        {
            string lightInstancePath = _zoneAssetPath + _zoneShortname + "/" + _zoneFolderPath + _zoneShortname + "_lights.txt";
            
            var lightInstanceList = (TextAsset) AssetDatabase.LoadAssetAtPath(lightInstancePath, typeof(TextAsset));

            if (lightInstanceList == null)
            {
                Debug.LogError("ZoneImporter: Unable to get light instance list at: " + lightInstancePath);
                return;
            }

            List<List<string>> parsedInstances = TextParser.ParseTextByDelimitedLines(lightInstanceList.text, ',', '#');

            GameObject lightRoot = new GameObject("Lights");
            lightRoot.transform.parent = _zoneRoot.transform;
            
            for (int i = 0; i < parsedInstances.Count; ++i)
            {
                List<string> lightValues = parsedInstances[i];
                
                if (lightValues.Count == 0)
                    continue;

                GameObject lightObject = new GameObject("Light_" + i);
                lightObject.transform.parent = lightRoot.transform;

                var light = lightObject.AddComponent<Light>();
                
                lightObject.transform.position = new Vector3((float) Convert.ToDouble(lightValues[0]),
                    (float) Convert.ToDouble(lightValues[1]), (float) Convert.ToDouble(lightValues[2]));
                
                var lightColor = new Color((float) Convert.ToDouble(lightValues[4]), (float) Convert.ToDouble(lightValues[5]),
                    (float) Convert.ToDouble(lightValues[6]));
                light.color = lightColor;

                light.range = (float) Convert.ToDouble(lightValues[3]) * _scaleFactor;
            }
        }

        /// <summary>
        /// Fix the import settings for materials that are used in the zone
        /// </summary>
        /// <param name="path">The path to the materials</param>
        static void FixMaterialImportSettings(string rootFolder, string fileName)
        {
            string assetFolderPath = _zoneAssetPath + _zoneShortname + "/" + rootFolder;
            
            var materialList = (TextAsset) AssetDatabase.LoadAssetAtPath(assetFolderPath + fileName, typeof(TextAsset));

            if (materialList == null)
            {
                Debug.LogError("ZoneImporter: Unable to fix materials at: " + assetFolderPath + fileName);
                return;
            }

            List<List<string>> materialListInstances = TextParser.ParseTextByDelimitedLines(materialList.text, ',', '#');

            if (materialListInstances == null || materialListInstances.Count == 0)
            {
                Debug.LogError("ZoneImporter: Unable to parse material instance list");
                return;
            }
            
            foreach (List<string> materialListValues in materialListInstances)
            {
                if (materialListValues.Count == 0)
                {
                    continue;
                }
                
                string[] folders = new string[1];
                folders[0] = _zoneAssetPath + _zoneShortname;
                string[] paths = AssetDatabase.FindAssets(materialListValues[0] + " t:texture2D", folders);

                if (paths.Length == 0)
                {
                    Debug.LogError("ZoneImporter: No assets found for name: " + materialListValues[0]);
                    continue;
                }

                string assetPath = AssetDatabase.GUIDToAssetPath(paths[0]);
                TextureImporter importer = (TextureImporter) TextureImporter.GetAtPath(assetPath);

                if (importer == null)
                {
                    Debug.LogError("ZoneImporter: Unable to get importer at path: " +
                                   AssetDatabase.GUIDToAssetPath(paths[0]) + " with name: " + materialListValues[0]);
                    continue;
                }

                importer.textureType = TextureImporterType.Default;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.SaveAndReimport();
            }
        }

        /// <summary>
        /// Fixes the materials so that they use the correct shader
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="fileName">The path to the material</param>
        static void FixMaterialShaderAssignment(string rootFolder, string fileName)
        {
            string assetFolderPath = _zoneAssetPath + _zoneShortname + "/" + rootFolder;
            string materialFolderPath = assetFolderPath + "Materials/";
            
            var materialList = (TextAsset) AssetDatabase.LoadAssetAtPath(assetFolderPath + fileName, typeof(TextAsset));

            if (materialList == null)
            {
                Debug.LogError("ZoneImporter: Unable to fix shader assignment at path: " + assetFolderPath + fileName);
                return;
            }

            List<List<string>> materialListInstances = TextParser.ParseTextByDelimitedLines(materialList.text, ',', '#');

            if (materialListInstances == null || materialListInstances.Count == 0)
            {
                Debug.LogError("ZoneImporter: Unable to parse material instance list");
                return;
            }
            
            foreach (List<string> materialListValues in materialListInstances)
            {
                if (materialListValues.Count == 0)
                {
                    return;
                }

                string materialPath = materialFolderPath + materialListValues[0] + ".mat";
                
                var material =
                    AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;

                if (material == null)
                {
                    Debug.Log("ZoneImporter: Can't find material: " + materialListValues[0] + "at: " + materialPath + ". Perhaps it has been found elsewhere?");
                    continue;
                }

                string shaderName;

                if (materialListValues[0].StartsWith("d_"))
                {
                    shaderName = "Legacy Shaders/Diffuse";
                }
                else if (materialListValues[0].StartsWith("t_"))
                {
                    shaderName = "Legacy Shaders/Transparent/Diffuse";
                }
                else if (materialListValues[0].StartsWith("b_"))
                {
                    shaderName = "EQ/AlphaFromBrightness";
                }
                else if (materialListValues[0].StartsWith("m_"))
                {
                    shaderName = "Legacy Shaders/Transparent/Cutout/Diffuse";
                }
                else
                {
                    Debug.LogError("ZoneImporter: Unrecognized material type: " + materialListValues[0]);
                    shaderName = "EQ/Diffuse";
                }

                Shader shader = Shader.Find(shaderName);

                if (shader == null)
                {
                    Debug.LogError("ZoneImporter: Cannot find the shader: " + shaderName);
                    continue;
                }

                material.shader = shader;
            }
        }

        /// <summary>
        /// Scales the zone based on the scale factor and rotates it to be correctly oriented
        /// </summary>
        private static void RotateAndScaleZone()
        {
            _zoneRoot.transform.localScale = new Vector3(_scaleFactor, _scaleFactor, _scaleFactor);

            Light[] lights = FindObjectsOfType<Light>();

            foreach (Light light in lights)
            {
                light.range *= _scaleFactor;
            }

            _zoneRoot.transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Removed the "Clone" string from the end of instantiated objects - automatically added by Unity
        /// </summary>
        /// <param name="gameObject"></param>
        private static void FixCloneNameAppend(GameObject gameObject)
        {
            if (!gameObject.name.EndsWith("(Clone)"))
            {
                return;
            }

            gameObject.name = gameObject.name.Substring(0, gameObject.name.Length - 7);
        }
        
        /// <summary>
        /// Fixes import settings for zone, objects and characters folder
        /// </summary>
        private static void FixAllModelMaterialReferences()
        {
            string directory = _zoneAssetPath + _zoneShortname;
            FixModelMaterialReferences(directory + "/" + _zoneFolderPath);
            FixModelMaterialReferences(directory + "/" +  _objectsFolderPath);
            FixModelMaterialReferences(directory + "/" +  _charactersFolderPath);
        }
        
        /// <summary>
        /// Sets the correct settings for the models to be imported
        /// </summary>
        /// <param name="directory">The directory in which to look for models</param>
        private static void FixModelMaterialReferences(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Debug.LogError("ZoneImporter: Cannot fix model material references at path: " + directory);
                return;
            }
            
            string[] meshFiles = Directory.GetFiles(directory, "*.obj", SearchOption.AllDirectories);

            for (int i = 0; i < meshFiles.Length; ++i)
            {
                meshFiles[i] = Path.GetFileName(meshFiles[i]);
            }

            foreach (var meshPath in meshFiles)
            {
                string assetPath = directory + meshPath;
                ModelImporter importer = (ModelImporter) ModelImporter.GetAtPath(assetPath);

                importer.addCollider = true; //  we will replace the collision mesh where relevant
                importer.importNormals = ModelImporterNormals.Calculate;
                importer.importMaterials = true;
                importer.materialLocation = ModelImporterMaterialLocation.External;
                importer.materialSearch = ModelImporterMaterialSearch.RecursiveUp;
                importer.SaveAndReimport();
            }
        }
    }
}

#endif