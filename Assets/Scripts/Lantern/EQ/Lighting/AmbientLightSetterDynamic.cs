using System;
using System.Collections.Generic;
using System.Linq;
using Lantern.EQ.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lantern.EQ.Lighting
{
    /// <summary>
    /// Updates ambient light on objects that move dynamically in the world
    /// </summary>
    public class AmbientLightSetterDynamic : MonoBehaviour
    {
        /// <summary>
        /// A reference to the zone sunlight values
        /// </summary>
        [SerializeField]
        private ZoneAmbientLightValues _sunlightValues;

        private Vector3 _lastPosition;
        private Quaternion _lastRotation;
        private float _lastSunlight = 1.0f;

        private float _sunlightRecaptureDelay = 1f;
        private float _sunlightRecaptureCurrent;

        [SerializeField]
        private List<Renderer> _renderers;
        private Action<float> _updateAmbientLightCallback;

        [SerializeField]
        private List<AmbientLightSetterChild> _childAmbientLightSetters = new List<AmbientLightSetterChild>();

        // TODO: Replace as property blocks no longer with with URP
        private MaterialPropertyBlock _block;

        private Vector3 _captureHeight;
        private bool _isInstantSunlight;
        private bool _forceUpdate;

        public void SetInstantSunlight()
        {
            _isInstantSunlight = true;
        }

        public void Initialize(float captureHeight, ZoneAmbientLightValues sunlightValues, Action<float> updateAmbientLightCallback)
        {
            _captureHeight = new Vector3(0, 5, 0);
            _sunlightValues = sunlightValues;
            _updateAmbientLightCallback = updateAmbientLightCallback;
            _sunlightRecaptureCurrent = Random.Range(0f, 0.5f);

            ForceUpdate();

            // Deprecate post 0.1.5 - MPB do not work with URP
            _block = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Serializes all renderers for this character
        /// </summary>
#if UNITY_EDITOR
        public void FindRenderers()
        {
            _renderers = GetComponentsInChildren<Renderer>(true).ToList();
        }
#endif

        public void ForceUpdate()
        {
            _forceUpdate = true;
            UpdateLight();
        }

        public void Update()
        {
            UpdateLight();
        }

        private void UpdateLight()
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
                    else
                    {
                        _lastSunlight = 1.0f;
                    }

                    _sunlightRecaptureCurrent = _sunlightRecaptureDelay;
                }
            }

            foreach (var renderer in _renderers)
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

            // TODO: move this up
            _updateAmbientLightCallback?.Invoke(_lastSunlight);

            if (_isInstantSunlight)
            {
                _isInstantSunlight = false;
            }

            foreach (var childSetters in _childAmbientLightSetters)
            {
                childSetters.UpdateLight(_lastSunlight);
            }

            _forceUpdate = false;
        }

        public void AddChild(AmbientLightSetterChild child)
        {
            _childAmbientLightSetters.Add(child);
            child.UpdateLight(_lastSunlight);
        }

        public void RemoveSunlightChild(AmbientLightSetterChild childSetter)
        {
            _childAmbientLightSetters.Remove(childSetter);
        }
    }
}
