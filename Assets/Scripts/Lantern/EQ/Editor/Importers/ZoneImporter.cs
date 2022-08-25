using System;
using System.IO;
using System.Linq;
using Lantern.Editor.Helpers;
using Lantern.EQ;
using Lantern.Logic;
using UnityEditor;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public class ZoneImporter : EditorWindow
    {
        /// <summary>
        /// The shortname of the zone that will be imported - e.g. arena, qeynos2, gfaydark
        /// </summary>
        private static string _zoneShortname;

        private GameObject _prefabRoot;
        private GameObject _zoneRoot;
        private GameObject _objectRoot;
        private GameObject _lightRoot;
        private GameObject _musicRoot;
        private GameObject _soundRoot;
        private GameObject _doorsRoot;

        private bool _preinstantiateObjects = false;
        private bool _preinstantiateDoors = false;

        /// <summary>
        /// Opens the zone importer settings window
        /// </summary>
        [MenuItem("EQ/Import/Zone &z", false, 10)]
        public static void ShowImportDialog()
        {
            GetWindow(typeof(ZoneImporter), true, "Import Zone");
        }

        /// <summary>
        /// Draws the settings window for the zone importer
        /// </summary>
        private void OnGUI()
        {
            // Force the window size
            int minHeight = 80;
            minSize = maxSize = new Vector2(225, minHeight);
            EditorGUIUtility.labelWidth = 100;
            _zoneShortname = EditorGUILayout.TextField("Zone Shortname", _zoneShortname);
            _preinstantiateObjects = GUILayout.Toggle(_preinstantiateObjects, "Preinstantiate Objects");
            _preinstantiateDoors = GUILayout.Toggle(_preinstantiateDoors, "Preinstantiate Doors");
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
        /// Imports the specified zone(s)
        /// </summary>
        private void ImportZone()
        {
            if (string.IsNullOrEmpty(_zoneShortname))
            {
                return;
            }

            _zoneShortname = _zoneShortname.ToLower();

            if (_zoneShortname == "sky")
            {
                Debug.LogError("ZoneImporter: Importing sky not supported");
                return;
            }

            var startTime = EditorApplication.timeSinceStartup;
            var splitNames = _zoneShortname.Split(';').ToList();

            if (splitNames.Count == 1 && (splitNames[0] == "all" || splitNames[0] == "antonica"
                                                                 || splitNames[0] == "odus" ||
                                                                 splitNames[0] == "faydwer" || splitNames[0] == "other"
                                                                 || splitNames[0] == "kunark" ||
                                                                 splitNames[0] == "velious" ||
                                                                 splitNames[0] == "planes"))
            {
                if (!ImportHelper.LoadTextAsset($"Assets/Content/ClientData/zonelist_{splitNames[0]}.txt",
                    out var allShortnames))
                {
                    Debug.LogError("$ZoneImporter: Unable to load zone list for specifier: {splitNames[0]}");
                    return;
                }

                splitNames = TextParser.ParseTextByNewline(allShortnames);
            }

            Close();

            foreach (var shortname in splitNames)
            {
                ImportZone(shortname);
            }

            EditorUtility.DisplayDialog("ZoneImport",
                $"Zone(s) import finished in {(int) (EditorApplication.timeSinceStartup - startTime)} seconds", "OK");

            // LANTERN ONLY
            Shader.SetGlobalColor("_DayNightColor", Color.white);
        }

        private void ImportZone(string shortname)
        {
            shortname = shortname.ToLower();

            DeleteOldAssets(shortname);

            var path = PathHelper.GetSystemPathFromUnity(PathHelper.GetRootLoadPath(shortname));
            if (!Directory.Exists(path))
            {
                Debug.LogError($"ZoneImporter: No folder at path: {path}");
                return;
            }

            TextureHelper.CopyTextures(shortname, AssetImportType.Zone);
            TextureHelper.CopyTextures(shortname, AssetImportType.Objects);

            // Prepare zone prefab roots
            CreatePrefabRoots(shortname);
            CreateZoneMesh(shortname);
            CreateObjectMeshes(shortname);
            CopyBspTree(shortname);
            CreateGlobalAmbientLightSetter(shortname);
            CreateZonePrefab(shortname);
            ImportObjects(shortname);
            ImportLights(shortname);
            ImportSounds(shortname);
            ImportMusic(shortname);

            if (_preinstantiateDoors)
            {
                ImportDoors(shortname);
            }

            ScalePrefab();
            TagRoots();
            SaveZonePrefab(shortname);
            TagAllAssetsForBundles(PathHelper.GetRootSavePath(shortname, false), shortname);
            DestroyImmediate(_prefabRoot);
        }

        private void DeleteOldAssets(string shortname)
        {
            var savePath = PathHelper.GetRootSavePath(shortname);
            var osDirectory = PathHelper.GetSystemPathFromUnity(savePath);
            if (Directory.Exists(osDirectory))
            {
                Directory.Delete(osDirectory, true);
            }

            AssetDatabase.Refresh();
        }

        private void CreatePrefabRoots(string shortname)
        {
            _prefabRoot = new GameObject(shortname);
            _zoneRoot = new GameObject("Zone");
            _objectRoot = new GameObject("Objects");
            _lightRoot = new GameObject("Lights");
            _musicRoot = new GameObject("Music");
            _soundRoot = new GameObject("Sounds");
            _doorsRoot = new GameObject("Doors");

            _musicRoot.transform.parent =
                _soundRoot.transform.parent = _doorsRoot.transform.parent = _lightRoot.transform.parent =
                    _objectRoot.transform.parent = _zoneRoot.transform.parent = _prefabRoot.transform;
        }

        private static void CreateZoneMesh(string shortname)
        {
            ActorStaticImporter.Import(shortname, shortname, AssetImportType.Zone);
        }

        private static void CreateObjectMeshes(string shortname)
        {
            ActorStaticImporter.ImportList(shortname, AssetImportType.Objects, go =>
            {
                if (go != null)
                {
                    var vcsn = go.AddComponent<VertexColorSetterNew>();
                    vcsn.FindMeshFilters();
                }
            });

            ActorSkeletalImporter.ImportList(shortname, AssetImportType.Objects);
        }

        private void CopyBspTree(string shortname)
        {
            var bspTreeAssetPath = PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "bsp_tree.txt";

            var systemPath = PathHelper.GetSystemPathFromUnity(bspTreeAssetPath);
            if (!File.Exists(systemPath))
            {
                Debug.LogError($"ZoneImporter: Unable to load BSP tree asset at: {systemPath}");
                return;
            }

            var destinationPath =
                PathHelper.GetSystemPathFromUnity(PathHelper.GetRootSavePath(shortname) + "bsp_tree.txt");

            File.Copy(systemPath, destinationPath);
            AssetDatabase.Refresh();
        }

        private void CreateGlobalAmbientLightSetter(string shortname)
        {
            var globalLightTextAssetPath =
                PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "ambient_light.txt";

            string globalLightText = string.Empty;

            ImportHelper.LoadTextAsset(globalLightTextAssetPath, out globalLightText);

            if (string.IsNullOrEmpty(globalLightText))
            {
                return;
            }

            var parsedLightLines = TextParser.ParseTextByDelimitedLines(globalLightText, ',');

            if (parsedLightLines.Count > 1)
            {
                Debug.LogWarning("ZoneImporter: More than one global ambient light color specified");
            }

            var colorValues = parsedLightLines[0];

            GlobalAmbientLightSetter lightSetter = _prefabRoot.AddComponent<GlobalAmbientLightSetter>();
            lightSetter.SetGlobalLightColor(new Color(Convert.ToInt32(colorValues[0]) / 255f,
                Convert.ToInt32(colorValues[1]) / 255f,
                Convert.ToInt32(colorValues[2]) / 255f));
        }

        private void CreateZonePrefab(string shortname)
        {
            var zonePrefabPath = PathHelper.GetSavePath(shortname, AssetImportType.Zone) + shortname +
                                 ".prefab";

            var zonePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(zonePrefabPath);

            if (zonePrefab == null)
            {
                Debug.LogError("ZoneImporter: Could not load zone prefab at path: " + zonePrefabPath);
                return;
            }

            GameObject zoneObject = (GameObject) PrefabUtility.InstantiatePrefab(zonePrefab);
            zoneObject.transform.parent = _zoneRoot.transform;
            zoneObject.layer = LanternLayers.Zone;
            zoneObject.AddComponent<ZoneMeshSunlightValues>();
        }

        private void ImportObjects(string shortname)
        {
            var loader = _prefabRoot.AddComponent<ObjectData>();
            var objectInstanceListPath =
                PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "object_instances.txt";
            ObjectImporter.CreateObjectInstances(shortname, objectInstanceListPath,
                _preinstantiateObjects ? _objectRoot.transform : null, loader);
        }

        private void ImportLights(string shortname)
        {
            var lightInstanceListPath = PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "light_instances.txt";
            LightImporter.CreateLightInstances(shortname, lightInstanceListPath, _lightRoot.transform);
        }

        private void ImportSounds(string shortname)
        {
            var soundsTextAssetPath = PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "sound_instances.txt";
            SoundImporter.CreateSoundInstances(soundsTextAssetPath, _soundRoot.transform);
        }

        private void ImportMusic(string shortname)
        {
            var musicTextAssetPath = PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "music_instances.txt";
            //MusicImporter.CreateMusicInstances(musicTextAssetPath, _musicRoot.transform);
        }

        private void ImportDoors(string shortname)
        {
            //DoorImporter.CreateDoorInstances(shortname, FindObjectOfType<ZoneMeshSunlightValues>(),
              //  _doorsRoot.transform);
        }

        private void ScalePrefab()
        {
            _prefabRoot.transform.localScale = new Vector3(LanternConstants.WorldScale, LanternConstants.WorldScale,
                LanternConstants.WorldScale);

            // TODO: Move this into the audio importer
            AudioSource[] audio = FindObjectsOfType<AudioSource>();

            foreach (AudioSource audioSource in audio)
            {
                audioSource.maxDistance *= LanternConstants.WorldScale;
            }
        }

        private void TagRoots()
        {
            _prefabRoot.tag = LanternTags.PrefabRoot;
            _zoneRoot.tag = LanternTags.ZoneRoot;
            _objectRoot.tag = LanternTags.ObjectRoot;
            _musicRoot.tag = LanternTags.MusicRoot;
            _soundRoot.tag = LanternTags.SoundRoot;
            _lightRoot.tag = LanternTags.LightRoot;
            _doorsRoot.tag = LanternTags.DoorRoot;
        }

        private void SaveZonePrefab(string shortname)
        {
            PrefabUtility.SaveAsPrefabAsset(_prefabRoot,
                PathHelper.GetRootSavePath(shortname) + shortname + ".prefab");
        }

        public static void TagAllAssetsForBundles(string folderPath, string bundleTag)
        {
            AssetImporter importer = AssetImporter.GetAtPath(folderPath);

            if (importer == null)
            {
                return;
            }

            importer.SetAssetBundleNameAndVariant(bundleTag, string.Empty);
            importer.SaveAndReimport();
        }
    }
}