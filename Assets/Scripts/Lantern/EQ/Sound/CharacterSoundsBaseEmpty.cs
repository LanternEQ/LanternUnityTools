using UnityEngine;

namespace Lantern.EQ.Sound
{
    public class CharacterSoundsBaseEmpty : CharacterSoundsBase
    {
        public override void AnimationPlayed(string animationName)
        {
        }

        public override void SetCurrentVariant(int variantId)
        {
        }

        public override void AddSoundClip(CharacterSoundType type, AudioClip clip, int variant)
        {
        }
    }
}
