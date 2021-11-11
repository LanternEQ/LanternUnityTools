using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern
{
    public class SunlightSetterStatic : MonoBehaviour
    {
        [SerializeField] 
        private float _sunlightValue;
        
        [SerializeField]
        private List<Renderer> _childRenderers;

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
                block.SetFloat("_DynamicSunlight", _sunlightValue);
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
            _sunlightValue = value;
        }
    }
}
