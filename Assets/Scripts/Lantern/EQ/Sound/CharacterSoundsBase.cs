using UnityEngine;

namespace Lantern.EQ.Sound
{
    public abstract class CharacterSoundsBase : MonoBehaviour
    {
        public abstract void AnimationPlayed(string animationName);
        public abstract void SetCurrentVariant(int variantId);
        public abstract void AddSoundClip(CharacterSoundType type, AudioClip clip, int variant);
    }
}
