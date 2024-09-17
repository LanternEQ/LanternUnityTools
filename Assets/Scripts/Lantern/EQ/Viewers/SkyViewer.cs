using Lantern.EQ.AssetBundles;
using Lantern.EQ.Environment;
using TMPro;
using UnityEngine;

namespace Lantern.EQ.Viewers
{
    /// <summary>
    /// A simple sky viewer demonstrating how to set up and manipulate EverQuest's sky
    /// </summary>
    public class SkyViewer : ViewerBase
    {
        [Header("Sky Viewer")]
        [SerializeField]
        private SkyViewerOptions _options;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private TextMeshProUGUI _debugText;

        [SerializeField]
        private GameObject _plane;

        private SkyController _sky;
        private float _time;
        private int _skyIndex;

        protected override void Awake()
        {
            base.Awake();
            InitializeOptions();
            LoadSky();
        }

        private void InitializeOptions()
        {
            _time = _options.StartingTime;
            _skyIndex = _options.StartingSkyIndex;
            _plane.SetActive(_options.ShowGroundPlane);
        }

        private void LoadSky()
        {
            // Load sky bundle
            var sky = AssetBundleLoader.LoadAsset<GameObject>(LanternAssetBundleId.Sky, "Sky");

            if (sky == null)
            {
                ShowError("Unable to load sky prefab.");
                return;
            }

            // Instantiate the sky prefab and scale it up to make sure it's not clipped by the camera
            // Uniform scaling does not make the sky appear larger
            var skyGo = Instantiate(sky);
            skyGo.transform.localScale = new Vector3(10f, 10f, 10f);

            // Get the SkyController component and initialize values
            _sky = skyGo.GetComponent<SkyController>();
            _sky.SetSecondsPerDay(_options.SecondsPerDay);
            _sky.SetEnabledSky(_options.StartingSkyIndex);
            _sky.UpdateTime(_time);
        }

        private void Update()
        {
            HandleInput();
            _sky.UpdateTime(_time);
            UpdateText();
            UpdateTimeOfDay();
        }

        private void UpdateTimeOfDay()
        {
            _time += Time.deltaTime * 1f / _options.SecondsPerDay;
            _time %= 1f;
            _camera.backgroundColor = WorldLightColor.Evaluate(_time);
        }

        private void UpdateText()
        {
            DebugTextBuilder.Clear();
            DebugTextBuilder.AppendLine("Sky Viewer");
            DebugTextBuilder.AppendLine($"Sky index: {_skyIndex}");
            DebugTextBuilder.AppendLine($"Time: {_time:0.00}");
            DebugTextBuilder.AppendLine($"Sky Objects: {_sky.GetSkyObjectCount()}");
            _debugText.text = DebugTextBuilder.ToString();
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _skyIndex = Mathf.Min(_skyIndex + 1, 5);
                _sky.SetEnabledSky(_skyIndex);
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _skyIndex = Mathf.Max(_skyIndex - 1, 1);
                _sky.SetEnabledSky(_skyIndex);
            }
        }

        private void LateUpdate()
        {
            _sky.UpdateTimeLate(Time.deltaTime, _time);
            Shader.SetGlobalColor("_DayNightColor", WorldLightColor.Evaluate(_time));
        }
    }
}

