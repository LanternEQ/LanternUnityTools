using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lantern.EQ
{
    public class SunlightSetterDynamic : MonoBehaviour
    {
        [SerializeField] 
        private ZoneMeshSunlightValues _sunlightValues;
    
        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private float _lastSunlight = 1.0f;

        private float _sunlightRecaptureDelay = 1f;
        private float _sunlightRecaptureCurrent;

        [SerializeField]
        private List<Renderer> _childRenderers;
        
        private bool _forceUpdate;

        private Action<float, bool> _newSunlightCallback;

        private RaycastHit[] _hit = new RaycastHit[1];
        
        [SerializeField]
        private List<SunlightSetterChild> _childLightSetters = new List<SunlightSetterChild>();

        [SerializeField]
        private bool _isPlayer;

        // TODO: This might be breaking dynamic light
        private MaterialPropertyBlock _block;

        private Vector3 _captureHeight;
        private bool _isInstantSunlight;

        private void Start()
        {
            if (_isPlayer)
            {
                _sunlightRecaptureDelay = 0f;
            }
        }

        public void SetInstantSunlight()
        {
            _isInstantSunlight = true;
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_sunlightValues == null)
            {
                _sunlightValues = FindObjectOfType<ZoneMeshSunlightValues>();
            }

            _sunlightRecaptureCurrent = Random.Range(0f, 0.5f);
            _block = new MaterialPropertyBlock();
            _captureHeight = new Vector3(0, 5, 0);

            FindChildRenderers();
            ForceUpdate();
            UpdateMeshColor();
        }
        
        public void FindChildRenderers()
        {
            _childRenderers = GetComponentsInChildren<Renderer>(true).ToList();
        }
        
        public void ForceUpdate()
        {
            _forceUpdate = true;
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            if (_sunlightValues == null)
            {
                _sunlightValues = FindObjectOfType<ZoneMeshSunlightValues>();
            }
            
            UpdateMeshColor();
        }

        private void Update()
        {
            UpdateMesh();
        }

        private void UpdateMeshColor()
        {
            _sunlightRecaptureCurrent = Mathf.Max(_sunlightRecaptureCurrent - Time.deltaTime, 0f);
            
            // Don't update unless there has been movement
            if (!_forceUpdate && _lastPosition == transform.position && _lastRotation == transform.rotation)
            {
                return;
            }
            
            var t = transform;
            _lastPosition = t.position;
            _lastRotation = t.rotation;

            if (_sunlightValues != null)
            {
                if (!(_sunlightRecaptureCurrent > 0f) || _forceUpdate)
                {
                    if (RaycastHelper.TryGetSunlightValueRuntime(transform.position + _captureHeight, _sunlightValues,
                        out var newSunlight))
                    {
                        _lastSunlight = newSunlight;
                    }

                    _sunlightRecaptureCurrent = _sunlightRecaptureDelay;
                }
            }
        
            foreach (var renderer in _childRenderers)
            {
                if (!renderer.gameObject.activeSelf)
                {
                    continue;
                }
                
                for (int i = 0; i < renderer.sharedMaterials.Length; ++i)
                {
                    _block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(_block, i);
                    _block.SetFloat("_DynamicSunlight", _lastSunlight);
                    renderer.SetPropertyBlock(_block, i);
                }
            }
            
            // Update fog color
            if (_isPlayer)
            {
                // TODO: move this up
                if (_newSunlightCallback != null)
                {
                    _newSunlightCallback(_lastSunlight, _isInstantSunlight);
                }
                
                if (_isInstantSunlight)
                {
                    _isInstantSunlight = false;
                }
            }

            foreach (var childSetters in _childLightSetters)
            {
                childSetters.UpdateLight(_lastSunlight);
            }
            
            _forceUpdate = false;
        }

        public void AddNewSunlightCallback(Action<float, bool> callback)
        {
            _newSunlightCallback = callback;
        }

        public void AddSunlightChild(SunlightSetterChild childSetter)
        {
            _childLightSetters.Add(childSetter);
            childSetter.UpdateLight(_lastSunlight);
        }
        
        public void RemoveSunlightChild(SunlightSetterChild childSetter)
        {
            _childLightSetters.Remove(childSetter);
        }

        public void SetIsPlayer()
        {
            _isPlayer = true;
        }
    }
}
