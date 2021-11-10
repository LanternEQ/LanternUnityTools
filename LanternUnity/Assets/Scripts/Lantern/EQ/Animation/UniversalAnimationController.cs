using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lantern.Helpers;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    public static class AnimationImportance
    {
        public static int Social = 0;
        public static int Movement = 2;
        public static int Rotating = 1;
        public static int Damage = 3;
        public static int Death = 5;

    }

    public class UniversalAnimationController : MonoBehaviour
    {
        private float _fadeTime = 0.3f;
        private Dictionary<AnimationType, string> ClipNames = new Dictionary<AnimationType, string>();
    
        private UnityEngine.Animation _animation;
        
        private AnimationType _currentConstantState;

        private Coroutine _returnToConstantState;
        private int _currentAnimationPriority;
        private bool _showAnimationDebug;

        private float _constantStateSpeed = 1.0f;

        private void Awake()
        {
            _animation = GetComponent<UnityEngine.Animation>();
            BuildClipList();
            SetNewConstantState(AnimationType.PassiveStand, 1);
            _animation.Sample();
        }

        public void SetShowAnimationDebug(bool isEnabled)
        {
            _showAnimationDebug = isEnabled;
        }

        private void BuildClipList()
        {
            foreach (AnimationState animationClip in _animation)
            {
                var animationSuffix = animationClip.name.Split('_')[1];
                AnimationType? animationType = AnimationHelper.GetAnimationType(animationSuffix);

                if (!animationType.HasValue)
                {
                    continue;
                }
                
                ClipNames[animationType.Value] = animationClip.name;
            }        
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

        public void SetNewConstantState(AnimationType animationType, int priority, float speed = 1f)
        {
            if (!ClipNames.ContainsKey(animationType))
            {
                return;
            }

            string clipName = ClipNames[animationType];
            
            var animClip = _animation[clipName];
            animClip.speed = speed;
            _constantStateSpeed = speed;
            
            if (speed == -1f)
            {
                _animation[clipName].time = _animation[clipName].length;
            }

            if(_currentConstantState == animationType)
            {
                return;
            }

            if (_returnToConstantState == null || priority > _currentAnimationPriority)
            {
                if (!_animation.isPlaying)
                {
                    _animation.Play(clipName);
                }
                else
                {
                    _animation.CrossFade(clipName, _fadeTime);
                }
                _currentAnimationPriority = priority;
            }

            _currentConstantState = animationType;
        }

        public void PlayOneShotSequence(List<AnimationType> animationTypes, AnimationType? newConstantState = null)
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
                if (!ClipNames.ContainsKey(anim))
                {
                    return;
                }
            }
            
            string fullName = ClipNames[animationTypes[0]];

            _animation.CrossFade(fullName, _fadeTime);
            float length = _animation[fullName].length;
            
            for (int i = 0; i < animationTypes.Count; ++i)
            {
                if (i == 0)
                {
                    continue;
                }
                
                fullName = ClipNames[animationTypes[i]];
                _animation.CrossFadeQueued(fullName, _fadeTime);
                length += _animation[fullName].length;
            }
            
            if (animationTypes.Last() != AnimationType.DamageDeath)
            {
                if (newConstantState.HasValue)
                {
                    _currentConstantState = newConstantState.Value;
                }

                length -= animationTypes.Count * _fadeTime;

                if (_returnToConstantState != null)
                {
                    StopCoroutine(_returnToConstantState);
                }

                // TODO: Sequence speed
                _returnToConstantState = StartCoroutine(ReturnToDefault(length, _constantStateSpeed));
            }
        }

        private void KillOneShotCoroutine()
        {
            if (_returnToConstantState != null)
            {
                StopCoroutine(_returnToConstantState);
                if (_showAnimationDebug)
                {
                    Debug.Log("One shot coroutine killed");
                }
            }
        }

        public void PlayOneShotAnimation(AnimationType animationType, float speed = 1f, int importance = 0, AnimationType? newConstantState = null)
        {
            if (!ClipNames.ContainsKey(animationType))
            {
                return;
            }
            
            string fullName = ClipNames[animationType];

            _animation[fullName].speed = speed;

            if (speed == -1)
            {
                _animation[fullName].time = _animation[fullName].length;
            }
            
            _animation.CrossFade(fullName, _fadeTime);

            if (newConstantState.HasValue)
            {
                _currentConstantState = newConstantState.Value;
            }

            float returnTime = _animation[fullName].length;

            if (!newConstantState.HasValue || newConstantState.Value != AnimationType.None)
            {
                returnTime -= _fadeTime;
            }

            if (_returnToConstantState != null)
            {
                StopCoroutine(_returnToConstantState);
            }

            _currentAnimationPriority = importance;
            
            _returnToConstantState = StartCoroutine(ReturnToDefault(returnTime, _constantStateSpeed));
        }

        private IEnumerator ReturnToDefault(float delay, float resolutionSpeed)
        {
            if (delay > 0.0f)
            {
                yield return new WaitForSeconds(delay);
            }
            
            if (_currentConstantState == AnimationType.None)
            {
                _animation.Stop();
                _returnToConstantState = null;
                if (_showAnimationDebug)
                {
                    Debug.Log("One shot coroutine finished to NONE");
                }
                yield break;
            }
            
            if(!ClipNames.ContainsKey(_currentConstantState))
            {
                Debug.LogError("No animation found for: " + _currentConstantState);
                yield break;
            }
            
            string clipName = ClipNames[_currentConstantState];
        
            var animClip = _animation[clipName];
            animClip.speed = resolutionSpeed;

            if (animClip.speed == -1f)
            {
                animClip.time = animClip.length;
            }
            
            _animation.CrossFade(clipName, _fadeTime);
            _returnToConstantState = null;

            if (_showAnimationDebug)
            {
                Debug.Log("One shot coroutine finished");
            }
        }

        public bool HasAnimation(AnimationType animationType)
        {
            return ClipNames.ContainsKey(animationType);
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

        public void ResolveCurrentStateWhenFinished(AnimationType resolutionState, float resolutionSpeed = 1f)
        {
            // figure out time remaining
            float timeRemaining = 0.0f;
            string fullName = ClipNames[_currentConstantState];

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

            if (_returnToConstantState != null)
            {
                StopCoroutine(_returnToConstantState);
            }

            if (timeRemaining <= 0.1f)
            {
                
            }

            if (timeRemaining <= _fadeTime)
            {
                _animation.CrossFade(fullName, _fadeTime);
            }
            
            _returnToConstantState = StartCoroutine(ReturnToDefault(timeRemaining, _constantStateSpeed));
        }
    }
}
