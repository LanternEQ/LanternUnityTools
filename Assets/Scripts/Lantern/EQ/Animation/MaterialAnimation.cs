using System.Collections.Generic;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    /// <summary>
    /// Controls material animation for a single mesh.
    /// </summary>
    public class MaterialAnimation : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        [SerializeField]
        private List<MaterialAnimationData> _instances;

        private MaterialPropertyBlock _pb;

        private void Start()
        {
            _pb = new MaterialPropertyBlock();
        }
        private void Update()
        {
            foreach (var instance in _instances)
            {
                UpdateInstance(instance);
            }
        }

        private void UpdateInstance(MaterialAnimationData instance)
        {
            instance.DelayCurrent -= (int) (Time.deltaTime * 1000);

            while (instance.DelayCurrent < 0)
            {
                instance.DelayCurrent += instance.Delay;

                instance.TextureIndex++;

                if (instance.TextureIndex >= instance.Textures.Count)
                {
                    instance.TextureIndex = 0;
                }

                var newTexture = instance.Textures[instance.TextureIndex];

                if (newTexture == null)
                {
                    continue;
                }

                // TODO: Remove property blocks
                _renderer.GetPropertyBlock(_pb, instance.Index);
                _pb.SetTexture("_BaseMap", instance.Textures[instance.TextureIndex]);
                _renderer.SetPropertyBlock(_pb, instance.Index);
            }
        }

#if UNITY_EDITOR
        public void AddInstance(MaterialAnimationData matchingMaterialAnimationData, int i)
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            _instances ??= new List<MaterialAnimationData>();
            _instances.Add(new MaterialAnimationData
            {
                Index = i,
                Delay = matchingMaterialAnimationData.Delay,
                Material = matchingMaterialAnimationData.Material,
                Textures = matchingMaterialAnimationData.Textures
            });
        }
        #endif
    }
}
