using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    [Serializable]
    public class AnimatedMaterial
    {
        public int Index;
        public Material Material; // not used but good for debugging
        public List<Texture> Textures;
        public int TextureIndex;
        public int Delay;
        public int DelayCurrent;
    }
    
    /// <summary>
    /// Controls texture animation for a single object. Most texture animation is done via the AnimatedMaterialSetter
    /// This will update independently of other objects.
    /// </summary>
    public class TextureAnimation : MonoBehaviour
    {
        [SerializeField] 
        private Renderer _renderer;
    
        [SerializeField]
        private List<AnimatedMaterial> _instances;

        private MaterialPropertyBlock _pb;

        private void Start()
        {
            _pb = new MaterialPropertyBlock();
        }
        private void Update()
        {
            for (var i = 0; i < _instances.Count; i++)
            {
                var instance = _instances[i];
            
                instance.DelayCurrent -= (int) (Time.deltaTime * 1000);

                while (instance.DelayCurrent < 0)
                {
                    instance.DelayCurrent += instance.Delay;

                    instance.TextureIndex++;

                    if (instance.TextureIndex >= instance.Textures.Count)
                    {
                        instance.TextureIndex = 0;
                    }
                }
                
                _renderer.GetPropertyBlock(_pb, instance.Index);
                _pb.SetTexture("_BaseMap", instance.Textures[instance.TextureIndex]);
                _renderer.SetPropertyBlock(_pb, instance.Index);
            }
        }

        public void AddInstance(AnimatedMaterial matchingMaterial, int i)
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }
            
            if (_instances == null)
            {
                _instances = new List<AnimatedMaterial>();
            }
            
            _instances.Add(new AnimatedMaterial
            {
                Index = i,
                Delay = matchingMaterial.Delay,
                Material = matchingMaterial.Material,
                Textures = matchingMaterial.Textures
            });
        }
    }
}
