using System;
using System.Collections;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    /// <summary>
    /// Animation controller that supports EQ functionality
    /// Designed as a replacement for the Animator component
    /// This class should only contain generic triggering logic
    /// </summary>
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.Animation _animation;

        // TODO: Put in config
        private float _fadeTime = 0.25f;

        /// <summary>
        /// A mapping of animation to clip names (used to references animation clips)
        /// </summary>
        [SerializeField]
        private AnimationClipMapping _clips;

        private Coroutine _resolveOneShot;
        private Coroutine _resolveConstantState;

        private AnimationType _currentConstantState;

        private string _constantStateClip;
        private int _constantStatePriority;
        private float _constantStateSpeed = 1.0f;

        private string _oneShotClip;
        private int _oneShotPriority;
        private float _oneShotSpeed;

        private Action<string> _animationDebugCallback;

        public void Initialize(AnimationType initialAnimation)
        {
            SetNewConstantState(initialAnimation, 0);

            // Sampling updates the skeleton immediately to avoid animation "pop in"
            _animation.Sample();
        }

        public void SetAnimationDebug(Action<string> callback)
        {
            _animationDebugCallback = callback;
        }

        public void SetNewConstantStateIfNot(AnimationType animationType, AnimationType ifNotAnimation, int priority,
            float speed = 1f)
        {
            if (_currentConstantState == ifNotAnimation)
            {
                return;
            }

            SetNewConstantState(animationType, priority, speed);
        }

        private bool CanPlayNewConstantState(int priority)
        {
            if (IsOneShotPlaying())
            {
                return priority > _oneShotPriority;
            }

            return true;
        }

        public void SetNewConstantState(AnimationType animationType, int priority, float speed = 1f)
        {
            if (!HasAnimation(animationType))
            {
                return;
            }

            // Ignore same state
            if(_currentConstantState == animationType)
            {
                return;
            }

            _currentConstantState = animationType;
            _constantStateSpeed = speed;
            _constantStatePriority = priority;

            if (_resolveConstantState != null)
            {
                StopCoroutine(_resolveConstantState);
                _resolveConstantState = null;
            }

            // Determine if it has importance enough to play now
            if (!CanPlayNewConstantState(priority))
            {
                return;
            }

            if (IsOneShotPlaying())
            {
                StopOneShot();
            }

            string clipName = _clips[animationType];
            _constantStateClip = clipName;

            if (!_animation.isPlaying)
            {
                _animation.Play(clipName);
                _animationDebugCallback?.Invoke("SetNewConstantState (P): " + AnimationHelper.GetDebugName(animationType));
            }
            else
            {
                _animation.CrossFade(clipName, _fadeTime);
                _animationDebugCallback?.Invoke("SetNewConstantState (CF): " +
                                                AnimationHelper.GetDebugName(animationType));
            }

            _currentConstantState = animationType;
        }

        private void StopOneShot()
        {
            if (_resolveOneShot == null)
            {
                return;
            }

            StopCoroutine(_resolveOneShot);
            _resolveOneShot = null;
            _oneShotPriority = 0;
            _oneShotSpeed = 1f;
            _oneShotClip = string.Empty;
        }

        /*public void PlayOneShotSequence(List<AnimationType> animationTypes, AnimationType? newConstantState = null)
        {
            if (animationTypes.Count == 0)
            {
                return;
            }

            if (animationTypes.Count == 1)
            {
                PlayOneShotAnimation(animationTypes[0]);
                return;
            }

            foreach (var anim in animationTypes)
            {
                if (!_clips.ContainsKey(anim))
                {
                    return;
                }
            }

            string fullName = _clips[animationTypes[0]];

            _animation.CrossFade(fullName, _fadeTime);
            _oneShotClip = fullName;
            float length = _animation[fullName].length;

            for (int i = 0; i < animationTypes.Count; ++i)
            {
                if (i == 0)
                {
                    continue;
                }

                fullName = _clips[animationTypes[i]];
                _animation.CrossFadeQueued(fullName, _fadeTime);
                length += _animation[fullName].length;
                _oneShotClip = fullName;
            }

            if (animationTypes.Last() != AnimationType.DamageDeath)
            {
                if (newConstantState.HasValue)
                {
                    _currentConstantState = newConstantState.Value;
                }

                length -= animationTypes.Count * _fadeTime;

                // TODO: Sequence speed
                AbortReturnToConstantState();
                _returnToConstantState = StartCoroutine(ReturnToDefault(length, _constantStateSpeed, _constantStatePriority));
            }
        }*/

        private void KillOneShotCoroutine()
        {
            if (_resolveOneShot != null)
            {
                StopCoroutine(_resolveOneShot);
                _animationDebugCallback?.Invoke("One shot coroutine killed");
            }
        }

        private bool IsOneShotPlaying()
        {
            return _resolveOneShot != null;
        }

        private bool CanPlayOneShot(int priority, bool canSelfInterrupt)
        {
            if (canSelfInterrupt)
            {
                return (IsOneShotPlaying() && priority >= _oneShotPriority) || (!IsOneShotPlaying() && priority >= _constantStatePriority);
            }
            return (IsOneShotPlaying() && priority > _oneShotPriority) || (!IsOneShotPlaying() && priority >= _constantStatePriority);
        }

        public void PlayOneShotAnimation(AnimationType animationType, float speed = 1f, int importance = 0, bool canSelfInterrupt = true, AnimationType? newConstantState = null)
        {
            if (!HasAnimation(animationType))
            {
                return;
            }

            if (!CanPlayOneShot(importance, canSelfInterrupt))
            {
                return;
            }

            string fullName = _clips[animationType];

            // Avoid restarting the same one shot
            if (fullName == _oneShotClip)
            {
                return;
            }

            _oneShotClip = fullName;
            _oneShotPriority = importance;
            _oneShotSpeed = speed;

            if (!_animation.isPlaying)
            {
                _animation[fullName].time = 0f;
                _animation.Play(fullName);
                _animationDebugCallback?.Invoke("P one shot: " + AnimationHelper.GetDebugName(animationType));
            }
            else
            {
                _animation.CrossFade(fullName, _fadeTime);
                _animationDebugCallback?.Invoke($"CF to one shot ({_animation[fullName].speed}): " + AnimationHelper.GetDebugName(animationType));
            }

            if (newConstantState.HasValue)
            {
                _currentConstantState = newConstantState.Value;
                _constantStatePriority = 0;
            }

            float returnTime = _animation[fullName].length;

            if (!newConstantState.HasValue || newConstantState.Value != AnimationType.None)
            {
                returnTime -= _fadeTime;
            }

            if (_resolveOneShot != null)
            {
                StopCoroutine(_resolveOneShot);
                _resolveOneShot = null;
            }

            _resolveOneShot = StartCoroutine(ReturnToDefault(returnTime));
            _animationDebugCallback?.Invoke("Returning to constant state in : " + returnTime);
        }

        private void AbortReturnToConstantState()
        {
            if (_resolveOneShot == null)
            {
                return;
            }

            StopCoroutine(_resolveOneShot);
            _resolveOneShot = null;
        }

        private IEnumerator ReturnToDefault(float delay)
        {
            if (delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }

            if (_currentConstantState == AnimationType.None)
            {
                _animation.Stop();
                _resolveOneShot = null;
                _animationDebugCallback?.Invoke("Resolving to state NON");
                yield break;
            }

            if(!_clips.ContainsKey(_currentConstantState))
            {
                _animationDebugCallback?.Invoke("No animation found for: " + AnimationHelper.GetDebugName(_currentConstantState));
                yield break;
            }

            string clipName = _clips[_currentConstantState];

            var animClip = _animation[clipName];
            animClip.speed = _constantStateSpeed;
            _animation.CrossFade(clipName, _fadeTime);

            _animationDebugCallback?.Invoke("Resolving (crossfade) back to: " + AnimationHelper.GetDebugName(_currentConstantState));
            _constantStateClip = clipName;

            StopOneShot();
        }

        private bool HasAnimation(AnimationType animationType)
        {
            return _clips.ContainsKey(animationType);
        }

        public bool HasAnimation(string animationName)
        {
            _animation = GetComponent<UnityEngine.Animation>();

            foreach (AnimationState animationClip in _animation)
            {
                if (animationClip.name.Split('_')[1] == animationName)
                {
                    return true;
                }
            }

            return false;
        }

        public void ResolveCurrentStateWhenFinished(AnimationType resolutionState, float resolutionSpeed = 1f,
            int priority = 0)
        {
            // figure out time remaining
            float timeRemaining;
            string fullName = _clips[_currentConstantState];

            var clip = _animation[fullName];
            if (clip.speed >= 0f)
            {
                timeRemaining = clip.length - Mathf.Repeat(clip.time, clip.length);
            }
            else
            {
                timeRemaining = clip.length - (clip.length - Mathf.Repeat(clip.time, clip.length));
            }

            if (timeRemaining < _fadeTime)
            {

            }

            timeRemaining -= _fadeTime;

            _currentConstantState = resolutionState;
            _constantStateSpeed = resolutionSpeed;
            _constantStatePriority = priority;

            if (timeRemaining <= 0.1f)
            {

            }

            if (timeRemaining <= _fadeTime)
            {
                _animation.CrossFade(fullName, _fadeTime);
                _constantStateClip = fullName;
            }

            AbortReturnToConstantState();
            _resolveConstantState = StartCoroutine(ReturnToDefault(timeRemaining));
        }

        public string GetClipName()
        {
            return _constantStateClip;
        }

        public string GetClipName(AnimationType animationType)
        {
            _clips.TryGetValue(animationType, out var clip);
            return clip;
        }

        public float GetClipLength(AnimationType animationType)
        {
            var clipName = GetClipName(animationType);
            if (clipName == null)
            {
                return 0f;
            }

            return _animation[clipName].length;
        }

        public float GetFadeTime()
        {
            return _fadeTime;
        }

        public void UpdateAnimationSpeed(AnimationType animationType, float animSpeed)
        {
            // TODO: Is this an error? Probably not.
            if (_currentConstantState != animationType)
            {
                return;
            }

            _animation[_constantStateClip].speed = animSpeed;
        }

        public float GetClipSpeed()
        {
            if (_animation.GetClip(_constantStateClip) == null)
            {
                return 0f;
            }

            return _animation[_constantStateClip].speed;
        }

        public bool IsPlaying()
        {
            return _animation.isPlaying;
        }

#if UNITY_EDITOR
        public void InitializeImport()
        {
            _animation = GetComponent<UnityEngine.Animation>();
            BuildClipList();
        }

        private void BuildClipList()
        {
            _clips = new AnimationClipMapping();

            var start = Time.realtimeSinceStartupAsDouble;
            foreach (AnimationState animationClip in _animation)
            {
                var animationSuffix = animationClip.name.Split('_')[1];
                AnimationType? animationType = AnimationHelper.GetAnimationType(animationSuffix);

                if (!animationType.HasValue || _clips.ContainsKey(animationType.Value))
                {
                    continue;
                }

                _clips[animationType.Value] = animationClip.name;
            }

            foreach (var animationFallback in AnimationHelper.AnimationFallbacks)
            {
                if (!_clips.ContainsKey(animationFallback.Key) && _clips.ContainsKey(animationFallback.Value))
                {
                    _clips[animationFallback.Key] = _clips[animationFallback.Value];
                }
            }
        }
#endif
    }
}
