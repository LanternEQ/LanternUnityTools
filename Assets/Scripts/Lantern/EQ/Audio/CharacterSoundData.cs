using System.Collections.Generic;
using Lantern.EQ.Sound;
using UnityEngine;

namespace Lantern.EQ.Audio
{
    public class CharacterSoundData
    {
        public Dictionary<CharacterSoundType, List<AudioClip>> Sounds;

        public CharacterSoundData()
        {
            Sounds = new Dictionary<CharacterSoundType, List<AudioClip>>();
        }
    }
}
