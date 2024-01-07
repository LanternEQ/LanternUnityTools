using UnityEngine;

namespace Lantern.EQ.Animation
{
    public class EquipmentAnimation : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Animation _animation;
        [SerializeField]
        private AnimationClipMapping _clips;

        public void Play(AnimationType animationType)
        {
            if (_animation == null || _clips == null)
            {
                return;
            }

            if (!_clips.TryGetValue(animationType, out var clipName))
            {
                return;
            }

            if (_animation[clipName] != null)
            {
                _animation.CrossFade(clipName);
                if (_animation.clip != null)
                {
                    _animation.CrossFadeQueued(_animation.clip.name);
                }
            }
        }

#if UNITY_EDITOR
        public void InitializeImport()
        {
            if (!TryGetComponent(out _animation))
            {
                return;
            }

            BuildClipList();
        }

        private void BuildClipList()
        {
            _clips = new AnimationClipMapping();

            foreach (AnimationState animationClip in _animation)
            {
                var animationSuffix = animationClip.name.Split('_')[1];
                AnimationType? animationType = AnimationHelper.GetAnimationType(animationSuffix);

                // use pos as the default animation.
                // it198 for example animates independent of the character's animation
                if (animationSuffix == "pos")
                {
                    _animation.clip = animationClip.clip;
                }

                if (!animationType.HasValue || _clips.ContainsKey(animationType.Value))
                {
                    continue;
                }

                _clips[animationType.Value] = animationClip.name;
            }

            // does equipment need fallbacks?
            /*
            foreach (var animationFallback in AnimationHelper.AnimationFallbacks)
            {
                if (!_animationClips.ContainsKey(animationFallback.Key) && _animationClips.ContainsKey(animationFallback.Value))
                {
                    _animationClips[animationFallback.Key] = _animationClips[animationFallback.Value];
                }
            }
            */
        }
#endif
    }
}
