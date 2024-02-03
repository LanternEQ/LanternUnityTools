using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infrastructure.Lantern.SQLite;
using Lantern.EQ.AssetBundles;
using Lantern.EQ.Environment;
using Lantern.EQ.Helpers;
using Lantern.EQ.Lantern;
using Lantern.EQ.Lighting;
using Lantern.EQ.Objects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lantern.EQ.Viewers
{
    public class ZoneViewer : ViewerBase
    {
        [Header("Zone Viewer")] [SerializeField]
        private ZoneViewerOptions _zoneViewerOptions;

        [SerializeField] private string _zoneToLoad;

        [SerializeField] private SunlightController _sunlightController;

        [SerializeField] private GameObject _selectionUi;

        private SkyController _sky;

        private float _time;
        private GameObject _zone;
        private Color _baseFogColor;
        private ZoneViewerState _viewerState;

        private List<Zone> _zonesInList;
        private int _zoneScrollIndex = 0;

        [FormerlySerializedAs("_zones")] [SerializeField] private List<SelectionItemBase> _zoneButtons;

        [SerializeField] private FreeCamera _freeCamera;

        [SerializeField] private GameObject _scrollBar;

        private enum ZoneViewerState
        {
            SelectingZone = 0,
            Viewer = 1
        }

        protected override void Awake()
        {
            base.Awake();
            _zonesInList = new List<Zone>();
            InitializeOptions();
            if (!LoadSky())
            {
                Debug.LogError("Unable to load sky");
            }

            SetZoneViewerState(_viewerState);
        }

        private void SetZoneViewerState(ZoneViewerState viewerState)
        {
            _viewerState = viewerState;
            _selectionUi.SetActive(viewerState == ZoneViewerState.SelectingZone);

            if (_viewerState == ZoneViewerState.SelectingZone)
            {
                if (_zone != null)
                {
                    Destroy(_zone);
                    _zone = null;
                }

                QueryZoneData();

                //_freeCamera.SetEnableState(false);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            if (_viewerState == ZoneViewerState.Viewer)
            {
                if (!LoadZone(_zoneToLoad, true))
                {
                    ShowError($"Unable to load zone: {_zoneToLoad}");
                    return;
                }

                //_freeCamera.SetEnableState(true);
                _sunlightController.UpdateTime(_time);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void SearchFieldUpdated(string filter)
        {
            QueryZoneData(filter.ToLower());
        }

        private void QueryZoneData(string filter = "")
        {
            var zoneLoader = new DatabaseLoader(Path.Combine(Application.streamingAssetsPath, "Database"));
            var allZones = zoneLoader.GetDatabase()
                ?.Table<Zone>().ToList().OrderBy(x => x.long_name).ToList();

            if (allZones == null || allZones.Count == 0)
            {
                Debug.LogError("No zones found.");
                return;
            }

            _zonesInList.Clear();

            // Figure out which zones are valid
            foreach (var z in allZones)
            {
                if (!AssetBundleHelper.DoesZoneBundleExist(z.short_name))
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(filter) && !z.long_name.ToLower().Contains(filter) &&
                    !z.short_name.ToLower().Contains(filter))
                {
                    continue;
                }

                _zonesInList.Add(z);
            }

            UpdateZoneListVisual();
            _scrollBar.SetActive(_zonesInList.Count > _zoneButtons.Count);
        }

        private void UpdateZoneListVisual()
        {
            for (int i = 0; i < _zoneButtons.Count; i++)
            {
                var button = _zoneButtons[i];
                if (_zonesInList.Count <= i)
                {
                    button.Clear();
                }
                else
                {
                    var zone = _zonesInList[i];
                    button.Initialize(Action, zone.long_name, zone.short_name);
                    button.SetIndex(i);
                }
            }
        }

        private void Action(int index)
        {
            var zone = _zonesInList[index];
            _zoneToLoad = zone.short_name;
            SetFog(zone.fogRed / 255f, zone.fogGreen / 255f, zone.fogBlue / 255f, zone.fogMinClip, zone.fogMaxClip,
                true);
            var safePoint = new Vector3(zone.safe_x, zone.safe_y, zone.safe_z);
            safePoint = PositionHelper.GetEqDatabaseToEqPosition(safePoint) * LanternConstants.WorldScale;
            safePoint.y += 3.75f;
            //_freeCamera.Initialize(safePoint, Quaternion.identity);
            SetZoneViewerState(ZoneViewerState.Viewer);
        }

        private void InitializeOptions()
        {
            _time = _zoneViewerOptions.StartTime;
        }

        private bool LoadSky()
        {
            // Load sky bundle
            var sky = AssetBundleLoader.LoadAsset<GameObject>(LanternAssetBundleId.Sky, "Sky");

            if (sky == null)
            {
                return false;
            }

            // Instantiate the sky prefab and scale it up to make sure it's not clipped by the camera
            // Uniform scaling does not make the sky appear larger
            var skyGo = Instantiate(sky);
            skyGo.transform.localScale = new Vector3(10f, 10f, 10f);
            skyGo.transform.parent = _freeCamera.transform;

            // Get the SkyController component and initialize values
            _sky = skyGo.GetComponent<SkyController>();
            _sky.SetSecondsPerDay(100);
            _sky.SetEnabledSky(1);
            _sky.UpdateTime(_time);
            return true;
        }

        public bool LoadZone(string zoneName, bool loadObjects)
        {
            var zone = AssetBundleLoader.LoadAsset<GameObject>(zoneName, LanternAssetBundleId.Zones, zoneName);

            if (zone == null)
            {
                return false;
            }

            _zone = Instantiate(zone);
            _zone.transform.localScale = new Vector3(LanternConstants.WorldScale, LanternConstants.WorldScale,
                LanternConstants.WorldScale);

            // Spawn zone objects
            var zoneObjects = _zone.GetComponent<ObjectData>();
            var objects = zoneObjects.GetObjects();

            if (loadObjects)
            {
                // This is a super rough way to load all zone objects
                // The client will spawn/despawn objects as you walk around the zone
                // and will never have everything loaded at once
                foreach (var o in objects)
                {
                    var obj = AssetBundleLoader.LoadAsset<GameObject>(zoneName, LanternAssetBundleId.Zones, o.Name);

                    if (obj == null)
                    {
                        Debug.LogError($"Unable to load door prefab: {o.Name}");
                        continue;
                    }

                    var spawnedObject = Instantiate(obj);
                    spawnedObject.transform.position = o.Position * LanternConstants.WorldScale;
                    spawnedObject.transform.rotation = Quaternion.Euler(o.Rotation);
                    spawnedObject.transform.localScale =
                        new Vector3(o.Scale, o.Scale, o.Scale) * LanternConstants.WorldScale;
                    spawnedObject.transform.parent = _zone.transform;
                }
            }

            if (true)
            {
                var databaseLoader = new DatabaseLoader(Path.Combine(Application.streamingAssetsPath, "Database"));
                var doors = databaseLoader.GetDatabase()
                    ?.Table<Doors>().Where(x => x.zone == _zoneToLoad);
                foreach (var d in doors)
                {
                    var dr = AssetBundleLoader.LoadAsset<GameObject>(zoneName, LanternAssetBundleId.Zones, d.name);

                    if (dr == null)
                    {
                        Debug.LogError($"Unable to load door prefab: {d.name}");
                        continue;
                    }

                    var spawnedObject = Instantiate(dr);
                    spawnedObject.transform.position =
                        PositionHelper.GetEqDatabaseToLanternPosition(d.pos_x, d.pos_y, d.pos_z);
                    spawnedObject.transform.rotation =
                        Quaternion.Euler(0.0f, RotationHelper.GetEqToLanternRotation(-d.heading), 0.0f);
                    spawnedObject.transform.localScale = Vector3.one * LanternConstants.WorldScale;
                    spawnedObject.transform.parent = _zone.transform;
                }
            }

            return true;
        }

        private void Update()
        {
            //if (!_zoneViewerOptions.TickTime)
            {
                //return;
            }

            HandleInput();

            _freeCamera.GetCamera().backgroundColor = WorldLightColor.Evaluate(_time);
            Shader.SetGlobalColor("_DayNightColor", WorldLightColor.Evaluate(_time));
            RenderSettings.fogColor = _baseFogColor;
            _freeCamera.GetCamera().backgroundColor = _baseFogColor;
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SetZoneViewerState(ZoneViewerState.Viewer);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetZoneViewerState(ZoneViewerState.SelectingZone);
            }
        }

        public void SetFog(float colorR, float colorG, float colorB, float start, float end, bool isFogEnabled)
        {
            RenderSettings.fog = isFogEnabled;
            _baseFogColor = new Color(colorR, colorG, colorB);
            RenderSettings.fogStartDistance = start * LanternConstants.WorldScale;
            RenderSettings.fogEndDistance = end * LanternConstants.WorldScale;
        }
    }


    public class Doors
    {
        [PrimaryKey] public int id { get; set; }
        public int doorid { get; set; }

        public string zone { get; set; }

        public float pos_x { get; set; }
        public float pos_y { get; set; }
        public float pos_z { get; set; }
        public string name { get; set; }
        public int opentype { get; set; }
        public float heading { get; set; }
    }

    public class Zone
    {
        [PrimaryKey] public int id { get; set; }
        public string short_name { get; set; }
        public string long_name { get; set; }
        public float safe_x { get; set; }
        public float safe_y { get; set; }
        public float safe_z { get; set; }
        public float safe_heading { get; set; }
        public float minClip { get; set; }
        public float maxClip { get; set; }
        public float fogMinClip { get; set; }
        public float fogMaxClip { get; set; }
        public int fogBlue { get; set; }
        public int fogRed { get; set; }
        public int fogGreen { get; set; }
        public int sky { get; set; }
    }
}
