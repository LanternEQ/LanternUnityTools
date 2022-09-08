using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    [Serializable]
    public class MaterialAnimationData
    {
        public int Index;
        public Material Material; // not used but good for debugging
        public List<Texture> Textures;
        public int TextureIndex;
        public int Delay;
        public int DelayCurrent;
    }
}
