using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern
{
    public class SunlightSetterChild : MonoBehaviour
    {
        [SerializeField]
        private List<Renderer> _childRenderers;

        //[SerializeField]
        //private List<RendererVisibilitySetter> _setter;

        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void UpdateLight(float sunlightValue)
        {
            if (_childRenderers == null)
            {
                FindChildRenderers();
            }
            
            foreach (var renderer in _childRenderers)
            {
                renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetFloat("_DynamicSunlight", sunlightValue);
                renderer.SetPropertyBlock(_propertyBlock);
            }
        }

        public void FindChildRenderers()
        {
            //_setter = new List<RendererVisibilitySetter>();
            _childRenderers = GetComponentsInChildren<Renderer>().ToList();
        }
    }
}
