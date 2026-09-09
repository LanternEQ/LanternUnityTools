using System;

namespace Lantern.EQ.Audio
{
    [Serializable]
    public class Sound3dData
    {
        public string ClipName;
        public float Volume;
        public float Cooldown;
        public float CooldownRandom;
        public int Multiplier;
    }
}
