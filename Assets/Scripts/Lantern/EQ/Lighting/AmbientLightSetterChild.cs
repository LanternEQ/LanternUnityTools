using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern.EQ.Lighting
{
    /// <summary>
    /// Sets ambient light for a mesh that is a child of another light setter
    /// Used for equipment
    /// </summary>
    public class AmbientLightSetterChild : MonoBehaviour
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
                if (!renderer.gameObject.activeSelf)
                {
                    continue;
                }

                for (int i = 0; i < renderer.sharedMaterials.Length; ++i)
                {
                    // TODO: Deprecate property blocks
                    renderer.GetPropertyBlock(_propertyBlock, i);
                    _propertyBlock.SetFloat("_DynamicSunlight", sunlightValue);
                    renderer.SetPropertyBlock(_propertyBlock, i);
                }
            }
        }

        public void FindChildRenderers()
        {
            //_setter = new List<RendererVisibilitySetter>();
            _childRenderers = GetComponentsInChildren<Renderer>().ToList();
        }
    }
}
