using UnityEngine;

namespace Lantern.EQ.Environment
{
    public class SkyLayer : MonoBehaviour
    {
        public enum SkyLayerType
        {
            SkyLayer,
            CloudLayer
        }

        [SerializeField]
        private SkyLayerType _layerType;

        [SerializeField]
        private Vector2 _uvPan;

        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField]
        private Renderer _renderer;

        private Material[] _layerMaterials;
        private Vector2 _uvPanCurrent;

        [Header("Reference Values - Read Only")]
        [SerializeField]
        private float _minHeight;

        [SerializeField]
        private float _maxHeight;

        [SerializeField]
        private Color _skydomeMinHeightColor = Color.white;

        [SerializeField]
        private Color _skydomeMaxHeightColor = Color.white;

        [SerializeField]
        private Vector2 _currentPan;

        private static readonly int ColorMin = Shader.PropertyToID("_ColorMin");
        private static readonly int ColorMax = Shader.PropertyToID("_ColorMax");
        private static readonly int HeightMin = Shader.PropertyToID("_HeightMin");
        private static readonly int HeightMax = Shader.PropertyToID("_HeightMax");
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        private void Awake()
        {
            _layerMaterials = _renderer.materials;
            gameObject.layer = LayerMask.NameToLayer("IgnoreTarget"); // Hide Name Tags from Targeting Camera
            SetMinMaxHeights();
        }

        public void UpdateTimeLate(float deltaTime, float currentTime)
        {
            UpdatePan(deltaTime);
            UpdateGradientColor(currentTime);
        }

#if UNITY_EDITOR
        public void SetSkyLayerData(Vector2 uvPanSpeed, SkyLayerType layerType)
        {
            _uvPan = uvPanSpeed;
            _layerType = layerType;
            _meshFilter = GetComponent<MeshFilter>();
            _renderer = GetComponent<Renderer>();
        }
#endif

        private void SetMinMaxHeights()
        {
            var bounds = _meshFilter.sharedMesh.bounds;
            var lossyScale = transform.lossyScale;
            _minHeight = bounds.min.y / lossyScale.y;
            _maxHeight = bounds.max.y / lossyScale.y;
        }

        private void UpdateGradientColor(float currentTime)
        {
            if (_layerMaterials == null)
            {
                return;
            }

            SkyGradientColor.Evaluate(currentTime, out var color1, out var color2, _layerType);

            foreach(var m in _layerMaterials)
            {
                m.SetColor(ColorMin, color1);
                m.SetColor(ColorMax, color2);
                m.SetFloat(HeightMin, _minHeight);
                m.SetFloat(HeightMax, _maxHeight);
            }

            _skydomeMinHeightColor = color1;
            _skydomeMaxHeightColor = color2;
        }

        private void UpdatePan(float deltaTime)
        {
            if (_layerMaterials == null)
            {
                return;
            }

            _uvPanCurrent.x += _uvPan.x * deltaTime;
            _uvPanCurrent.y += _uvPan.y * deltaTime;
            _uvPanCurrent.x %= 1.0f;
            _uvPanCurrent.y %= 1.0f;

            foreach (var m in _layerMaterials)
            {
                m.SetTextureOffset(BaseMap,  new Vector2(_uvPanCurrent.x, _uvPanCurrent.y));
            }

            _currentPan = _uvPanCurrent;
        }
    }
}
