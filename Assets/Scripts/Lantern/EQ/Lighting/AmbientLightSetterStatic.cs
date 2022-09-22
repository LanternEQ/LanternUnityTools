using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern.EQ.Lighting
{
    /// <summary>
    /// Sets ambient light of static objects
    /// </summary>
    public class AmbientLightSetterStatic : MonoBehaviour
    {
        [SerializeField]
        protected List<Renderer> _childRenderers;

        protected float AmbientLight;

        //[SerializeField]
        //private List<RendererVisibilitySetter> _setter;

        public void Awake()
        {
            ProcessIsVisible();
        }

        public void UpdateMeshColor()
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();

            if (_childRenderers == null)
            {
                FindChildRenderers();
            }

            foreach (var renderer in _childRenderers)
            {
                renderer.GetPropertyBlock(block);
                block.SetFloat("_DynamicSunlight", AmbientLight);
                renderer.SetPropertyBlock(block);
            }
        }

        public void ProcessIsVisible()
        {
            UpdateMeshColor();

            /*foreach (var setter in _setter)
            {
                setter.enabled = false;
            }*/

            enabled = false;
        }

        public void FindChildRenderers()
        {
            //_setter = new List<RendererVisibilitySetter>();
            _childRenderers = GetComponentsInChildren<Renderer>().ToList();
        }

        public void SetSunlightValue(float value)
        {
            AmbientLight = value;
        }
    }
}
