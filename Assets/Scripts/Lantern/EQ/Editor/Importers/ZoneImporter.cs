using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Infrastructure.EQ.TextParser;
using Lantern.EQ.Editor.AssetBundles;
using Lantern.EQ.Editor.Helpers;
using Lantern.EQ.Lantern;
using Lantern.EQ.Lighting;
using Lantern.EQ.Objects;
using UnityEditor;
using UnityEngine;

namespace Lantern.EQ.Editor.Importers
{
    public class ZoneImporter : LanternEditorWindow
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

        private bool _preinstantiateObjects;
        private bool _preinstantiateDoors;
        private bool _rebuildBundles = true;

        private enum ZoneImportType
        {
            SingleZone,
            Batch
        }

        private ZoneImportType _importType = ZoneImportType.SingleZone;
        private ZoneBatchType _zoneBatchType = ZoneBatchType.All;

        private static readonly List<string> Text1 = new()
        {
            "This process creates zone prefabs from intermediate EverQuest data.",
            "This usually takes 2-6 minutes per zone depending on complexity."
        };

        private static readonly List<string> Text2 = new()
        {
            "EverQuest zone data (one folder per zone) must be located in:",
            "\fAssets/EQAssets/",
        };

        private static readonly List<string> Text3 = new()
        {
            "Zone prefabs will be output to:",
            "\fAssets/Content/AssetBundleContent/Zones/"
        };

        private static readonly List<string> Text4 = new()
        {
            "Importing all zones may takes several hours and the editor may become unresponsive."
        };

        private static readonly List<string> Text5 = new()
        {
            "Pre-instantiating objects and doors should only be done if you're not building for the LanternEQ client."
        };

        /// <summary>
        /// Opens the zone importer settings window
        /// </summary>
        [MenuItem("EQ/Assets/Import Zone &z", false, 1)]
        public static void ShowImportDialog()
        {
            GetWindow<ZoneImporter>("Import Zone", typeof(EditorWindow));
        }

        private void OnGUI()
        {
            DrawInfoBox(Text1, "d_console.infoicon");
            DrawInfoBox(Text2, "d_Collab.FolderConflict");
            DrawInfoBox(Text3, "d_Collab.FolderMoved");
            DrawHorizontalLine();

            DrawEnumPopup("Import Type", ref _importType);

            if (_importType == ZoneImportType.SingleZone)
            {
                DrawTextField("Zone Shortname", ref _zoneShortname);
            }
            else if (_importType == ZoneImportType.Batch)
            {
                DrawEnumPopup("Batch Type", ref _zoneBatchType);
            }

            if (_importType == ZoneImportType.Batch && _zoneBatchType == ZoneBatchType.All)
            {
                DrawInfoBox(Text4, "d_console.warnicon");
            }

            if (_preinstantiateDoors || _preinstantiateObjects)
            {
                DrawInfoBox(Text5, "d_console.warnicon");
            }

            DrawToggle("Pre-instantiate Objects", ref _preinstantiateObjects);
            DrawToggle("Pre-instantiate Doors", ref _preinstantiateDoors);
            DrawToggle("Rebuild Bundles", ref _rebuildBundles);

            if (DrawButton("Start Import"))
            {
                ImportZone();
            }
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

            StartImport();
            var splitNames = _zoneShortname.Split(';').ToList();

            if (_importType == ZoneImportType.Batch)
            {
                splitNames = ImportHelper.GetBatchZoneNames(_zoneBatchType);
            }

            List<string> successful = new List<string>();
            List<string> failed = new List<string>();

            foreach (var shortname in splitNames)
            {
                if (ImportZone(shortname))
                {
                    successful.Add(shortname);
                }
                else
                {
                    failed.Add(shortname);
                }
            }

            if (_rebuildBundles)
            {
                BuildAssetBundles.BuildAllAssetBundles(false);
            }

            var importTime = FinishImport();
            string importResult = GetFormattedImportResult(importTime, successful, failed);

            EditorUtility.DisplayDialog("ZoneImport" + (_rebuildBundles ? "/BuildBundles" : string.Empty),
                importResult,
                "OK");

            // LANTERN ONLY
            Shader.SetGlobalColor("_DayNightColor", Color.white);
        }

        private bool ImportZone(string shortname)
        {
            shortname = shortname.ToLower();

            DeleteOldAssets(shortname);

            var path = PathHelper.GetSystemPathFromUnity(PathHelper.GetRootLoadPath(shortname));
            if (!Directory.Exists(path))
            {
                Debug.LogError($"ZoneImporter: No folder at path: {path}");
                return false;
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
            CreateAmbientLightValues();
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
            ImportHelper.TagAllAssetsForBundles(PathHelper.GetRootSavePath(shortname, false), shortname);
            DestroyImmediate(_prefabRoot);
            return true;
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
                    var vcsn = go.AddComponent<VertexColorSetter>();
                    vcsn.FindMeshFilters();
                }

                if (go.name.StartsWith("ladder"))
                {
                    var currentCollider = go.GetComponent<Collider>();
                    if (currentCollider != null)
                    {
                        DestroyImmediate(currentCollider);
                    }

                    go.AddComponent<BoxCollider>();
                    var climbTrigger = go.AddComponent<BoxCollider>();
                    climbTrigger.isTrigger = true;
                    var size = climbTrigger.size;
                    size.x += 1f;
                    size.y += 1f;
                    size.z += 1f;
                    climbTrigger.size = size;
                }

                var rb = go.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.isKinematic = true;
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

            ImportHelper.LoadTextAsset(globalLightTextAssetPath, out var globalLightText);

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

            AmbientLightSetterGlobal lightSetter = _prefabRoot.AddComponent<AmbientLightSetterGlobal>();
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

            GameObject zoneObject = (GameObject)PrefabUtility.InstantiatePrefab(zonePrefab);
            zoneObject.transform.parent = _zoneRoot.transform;
            zoneObject.layer = LanternLayers.Zone;
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
            var sounds2dTextAssetPath =
                PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "sound2d_instances.txt";
            var sounds3dTextAssetPath =
                PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "sound3d_instances.txt";
            SoundImporter.CreateSoundInstances(sounds2dTextAssetPath, sounds3dTextAssetPath, _soundRoot.transform);
        }

        private void ImportMusic(string shortname)
        {
            var musicTextAssetPath = PathHelper.GetLoadPath(shortname, AssetImportType.Zone) + "music_instances.txt";
            MusicImporter.CreateMusicInstances(musicTextAssetPath, _musicRoot.transform, shortname);
        }

        private void ImportDoors(string shortname)
        {
            DoorImporter.CreateDoorInstances(shortname, _doorsRoot.transform);
        }

        private void ScalePrefab()
        {
            _prefabRoot.transform.localScale = new Vector3(LanternConstants.WorldScale, LanternConstants.WorldScale,
                LanternConstants.WorldScale);
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

        private void CreateAmbientLightValues()
        {
            var ambientLight = _prefabRoot.AddComponent<ZoneAmbientLightValues>();

            foreach (Transform child in _prefabRoot.transform)
            {
                if (child.name != "Zone")
                {
                    continue;
                }

                var meshFilter = child.GetComponentInChildren<MeshFilter>();
                ambientLight.SetMeshFilter(meshFilter);
            }
        }

        private void SaveZonePrefab(string shortname)
        {
            PrefabUtility.SaveAsPrefabAsset(_prefabRoot,
                PathHelper.GetRootSavePath(shortname) + shortname + ".prefab");
        }

        private string GetFormattedImportResult(int importTime, List<string> successful, List<string> failed)
        {
            StringBuilder importResult = new StringBuilder();
            importResult.AppendLine(
                $"Zone(s) import {(_rebuildBundles ? "and build bundles " : String.Empty)}finished in {importTime} seconds.");
            importResult.AppendLine();
            if (successful.Count > 0)
            {
                importResult.AppendLine("SUCCESSFUL");
                for (var i = 0; i < successful.Count; i++)
                {
                    if (i != 0)
                    {
                        importResult.Append(", ");
                    }

                    importResult.Append(successful[i]);
                }

                importResult.AppendLine();
                importResult.AppendLine();
            }

            if (failed.Count > 0)
            {
                importResult.AppendLine("FAILED");
                for (var i = 0; i < failed.Count; i++)
                {
                    if (i != 0)
                    {
                        importResult.Append(", ");
                    }

                    importResult.Append(failed[i]);
                }
            }

            return importResult.ToString();
        }
    }
}
