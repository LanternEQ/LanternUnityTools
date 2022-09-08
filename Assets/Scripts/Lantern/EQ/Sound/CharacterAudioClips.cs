using System;
using Infrastructure.EQ.SerializableDictionary;

namespace Lantern.EQ.Sound
{
    [Serializable]
    public class CharacterAudioClips : SerializableDictionary<CharacterSoundType, AudioClipGroup>
    {
    }
}
