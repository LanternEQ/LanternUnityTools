using System;
using Lantern.EQ.Data;
using Lantern.EQ.Lantern;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lantern.EQ.Animation
{
    /// <summary>
    /// Updates animation state and forwards it to the CharacterAnimationController
    /// </summary>
    public class CharacterAnimationLogic : MonoBehaviour
    {
        [SerializeField]
        private CharacterAnimationController _animationController;

        private CharacterAnimationState _currentState;
        private CharacterAnimationState _lastState;

        private float _idleCheckInterval = 1f;
        private float _currentIdleTimer;

        private bool _isRelaxed;
        private bool _lastRelaxed;
        private bool _isInitialized;
        private CharacterStateData _state;
        private Action<string> _animationDebugCallback;

        public void Initialize(CharacterAnimationState state)
        {
            if (_animationController == null)
            {
                enabled = false;
                return;
            }

            _currentState = CharacterAnimationState.Standing;
            _animationController.Initialize(AnimationType.PassiveStand);
            _lastState = CharacterAnimationState.None;
            ResetIdleTimer();
            _isInitialized = true;
        }

#if UNITY_EDITOR
        public void InitializeImport()
        {
            _animationController = gameObject.GetComponent<CharacterAnimationController>();
        }
#endif

        public void UpdateState(CharacterStateData data)
        {
            if (_animationController == null)
            {
                return;
            }

            _currentState = GetCharacterAnimationState(data);
            CheckForStateUpdate();

            if (AnimationHelper.IsVariableSpeedState(_currentState))
            {
                var animSpeed = AnimationHelper.GetAnimationSpeed(data);
                _animationController.UpdateAnimationSpeed(animSpeed.AnimationType, animSpeed.Speed);
            }

            if (_state.SocialAnimation.HasValue)
            {
                PlaySocialAnimation(_state.SocialAnimation.Value);
            }

            if (_state.ActiveAnimation.HasValue)
            {
                PlayActiveAnimation(_state.ActiveAnimation.Value);
            }

            _lastState = _currentState;
        }

        private void PlaySocialAnimation(AnimationType animation)
        {
            _animationController.PlayOneShotAnimation(animation, 1.0f, AnimationImportance.Social,
                true, AnimationType.PassiveStandArmsAtSides);
            ResetIdleTimer();
        }

        private void PlayActiveAnimation(AnimationType animation)
        {
            _animationController.PlayOneShotAnimation(animation, 1.0f, AnimationImportance.Action,
                true, AnimationType.PassiveStandArmsAtSides);
            ResetIdleTimer();
        }

        private CharacterAnimationState GetCharacterAnimationState(CharacterStateData data)
        {
            _state = data;

            if (data.IsDead)
            {
                return CharacterAnimationState.Dead;
            }

            if (data.IsJumping)
            {
                return data.Velocity.x > 0 ? CharacterAnimationState.JumpingMoving : CharacterAnimationState.JumpingStill;
            }

            if (data.IsSwimming)
            {
                if (Mathf.Approximately(data.Velocity.x, 0f) && Mathf.Approximately(data.Velocity.z, 0f))
                {
                    return CharacterAnimationState.SwimmingStill;
                }

                return CharacterAnimationState.SwimmingMoving;
            }

            if (!data.IsGrounded && !data.IsFlying && data.Velocity.y < EqConstants.FallVelocityThreshold * LanternConstants.WorldScale)
            {
                return CharacterAnimationState.Falling;
            }

            if (data.IsClimbing && !Mathf.Approximately(data.Velocity.y, 0f))
            {
                return CharacterAnimationState.Climbing;
            }

            if (!data.IsStanding)
            {
                return CharacterAnimationState.Sitting;
            }

            if (data.IsLooting)
            {
                return CharacterAnimationState.Looting;
            }

            if (data.Velocity.x != 0f || data.Velocity.z != 0f)
            {
                if (data.IsDucking)
                {
                    return CharacterAnimationState.DuckingMoving;
                }

                if (data.Velocity.x < 0f)
                {
                    return CharacterAnimationState.WalkingBackwards;
                }

                return data.Velocity.x > data.WalkSpeedThreshold ? CharacterAnimationState.Running : CharacterAnimationState.Walking;
            }

            if (data.RotationVelocity != 0)
            {
                if (data.IsDucking)
                {
                    return CharacterAnimationState.DuckingMoving;
                }

                return data.RotationVelocity > 0f ? CharacterAnimationState.RotatingRight : CharacterAnimationState.RotatingLeft;
            }

            if (data.IsDucking)
            {
                return CharacterAnimationState.DuckingStill;
            }

            return CharacterAnimationState.Standing;
        }

        private void CheckForStateUpdate()
        {
            if (_currentState != CharacterAnimationState.Standing && _currentState != CharacterAnimationState.Sitting)
            {
                ResetIdleTimer();
            }

            // If player is dead, there is nothing else to update
            if (_currentState == CharacterAnimationState.Dead)
            {
                // Death
                if (IsNewState(CharacterAnimationState.Dead))
                {
                    _animationController.PlayOneShotAnimation(AnimationType.DamageDeath, 1f,AnimationImportance.Death, true, AnimationType.None);
                }

                _lastState = CharacterAnimationState.Dead;
            }
            else if (IsNewState(CharacterAnimationState.JumpingStill))
            {
                _animationController.PlayOneShotAnimation(AnimationType.LocomotionJumpStand, 1.0f, AnimationImportance.Jump);
            }
            else if (IsNewState(CharacterAnimationState.JumpingMoving))
            {
                _animationController.PlayOneShotAnimation(AnimationType.LocomotionJumpRun, 1.0f, AnimationImportance.Jump);
            }
            else if (HasTransitionedFrom(CharacterAnimationState.Sitting))
            {
                _animationController.PlayOneShotAnimation(AnimationType.PassiveStandSit, 1.0f, AnimationImportance.SitStand, true,
                        AnimationType.PassiveStandArmsAtSides);
            }
            else if (IsNewState(CharacterAnimationState.SwimmingStill))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionSwimTread, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.Falling))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionFall, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.Sitting))
            {
                _animationController.PlayOneShotAnimation(AnimationType.PassiveSitStand, 1.0f, AnimationImportance.SitStand, true,
                    AnimationType.PassiveSitting);
            }
            else if (IsNewState(CharacterAnimationState.Climbing))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionClimb, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.DuckingMoving))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionDuckWalk, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.Standing))
            {
                if (_lastState == CharacterAnimationState.DuckingStill)
                {
                    _animationController.PlayOneShotAnimation(AnimationType.DuckReverse, 1.0f, AnimationImportance.SitStand, true, AnimationType.PassiveStandArmsAtSides);
                }
                else if (_lastState == CharacterAnimationState.Looting)
                {
                    _animationController.PlayOneShotAnimation(AnimationType.LootReverse, 1.0f, AnimationImportance.SitStand, true, AnimationType.PassiveStandArmsAtSides);
                }
                else
                {
                    // Rotating does not resolve immediately
                    if (_lastState == CharacterAnimationState.RotatingLeft ||
                        _lastState == CharacterAnimationState.RotatingRight)
                    {
                        _animationController.ResolveCurrentStateWhenFinished(AnimationType.PassiveStand, 1.0f, AnimationImportance.Social);
                    }

                    _animationController.SetNewConstantStateIfNot(AnimationType.PassiveStand,
                        AnimationType.PassiveStandArmsAtSides, AnimationImportance.Social);
                }
            }
            else if (IsNewState(CharacterAnimationState.Looting))
            {
                _animationController.PlayOneShotAnimation(AnimationType.Loot, 1f, AnimationImportance.Jump, true, AnimationType.None);
            }
            else if (IsNewState(CharacterAnimationState.RotatingRight))
            {
                _animationController.SetNewConstantState(AnimationType.PassiveRotating, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.RotatingLeft))
            {
                _animationController.SetNewConstantState(AnimationType.PassiveRotatingReverse, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.Running))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionRun, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.Walking))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionWalk, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.WalkingBackwards))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionWalkReverse, AnimationImportance.Movement);
            }
            else if (IsNewState(CharacterAnimationState.DuckingStill))
            {
                if (_lastState != CharacterAnimationState.DuckingMoving)
                {
                    _animationController.PlayOneShotAnimation(AnimationType.Duck, 1.0f, 1,
                        true, AnimationType.None);
                    ResetIdleTimer();
                }
                else
                {
                    _animationController.ResolveCurrentStateWhenFinished(AnimationType.None);
                }
            }
            else if (IsNewState(CharacterAnimationState.SwimmingMoving))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionSwimMove, AnimationImportance.Movement);
            }
        }

        /// <summary>
        /// Used to determine that we have left a state
        /// Sitting is a good example. It can transition to many different states.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private bool HasTransitionedFrom(CharacterAnimationState state)
        {
            return _lastState == state && _currentState != state;
        }

        private bool IsNewState(CharacterAnimationState state)
        {
            return _currentState == state && _lastState != state;
        }

        private void Awake()
        {
            // If this class is not initialized when created, we assume that it should be in the stand position
            // This is useful when the prefab is dragged into the scene
            if (!_isInitialized)
            {
                Initialize(CharacterAnimationState.Standing);
            }
        }

        private void Update()
        {
            // These animation states have accompanying idle animations
            if (_currentState == CharacterAnimationState.Sitting
                || _currentState == CharacterAnimationState.Standing
                || _currentState == CharacterAnimationState.StandingArmsAtSide)
            {
                UpdateIdleState();
            }
        }

        private void UpdateIdleState()
        {
            _currentIdleTimer -= Time.deltaTime;

            if (_currentIdleTimer < 0f)
            {
                ResetIdleTimer(false);

                if (Random.Range(1, 100) <= 80)
                {
                    return;
                }

                bool shouldSkipIdle = false;
                AnimationType oneShot;
                AnimationType resolutionState;

                if (_currentState == CharacterAnimationState.Sitting)
                {
                    oneShot = AnimationType.IdleSit;
                    resolutionState = AnimationType.PassiveSitting;
                }
                else
                {
                    oneShot = _isRelaxed ? AnimationType.IdleStandArmsAtSides : AnimationType.IdleStand;
                    resolutionState = AnimationType.PassiveStandArmsAtSides;
                    // characters with _o02 animations wait an additional cycle after relaxing
                    shouldSkipIdle = _isRelaxed && !_lastRelaxed && _animationController.GetClipName(oneShot)?.EndsWith("_o02") == true;
                    SetRelaxed();
                }

                if (shouldSkipIdle)
                {
                    return;
                }

                DelayIdleTimer(oneShot, resolutionState);
                _animationController.PlayOneShotAnimation(oneShot, 1f, AnimationImportance.Social, true, resolutionState);
            }
        }

        private void SetRelaxed(bool state = true)
        {
            _lastRelaxed = _isRelaxed;
            // elf based characters don't appear to relax into IdleStandArmsAtSides in trilogy despite having the animation
            // TODO: allow as a Lantern optional feature
            _isRelaxed = state && _animationController.GetClipName(AnimationType.IdleStandArmsAtSides) != "elf_o02";
        }

        private void DelayIdleTimer(AnimationType idle, AnimationType passive)
        {
            var crossfadeTime = _animationController.GetFadeTime();
            var idleLength = _animationController.GetClipLength(idle);
            var passiveLength = _animationController.GetClipLength(passive);

            _currentIdleTimer = idleLength + passiveLength + crossfadeTime;
        }

        private void ResetIdleTimer(bool hasMoved = true)
        {
            _currentIdleTimer = _idleCheckInterval;

            if (hasMoved)
            {
                SetRelaxed(false);
            }
        }

        public CharacterAnimationState GetCurrentState()
        {
            return _currentState;
        }

        public string GetCurrentClip()
        {
            return _animationController.GetClipName();
        }

        public float GetIdleTimer()
        {
            return _currentIdleTimer;
        }

        public float GetAnimationSpeed()
        {
            return _animationController.GetClipSpeed();
        }

        public bool GetIsPlaying()
        {
            return _animationController.IsPlaying();
        }

        public void SetAnimationDebug(Action<string> callback)
        {
            _animationDebugCallback = callback;
                _animationController.SetAnimationDebug(_animationDebugCallback);
        }

        // TODO: Deprecate this. Should all go through something else.
        public void PlayOneShotAnimation(AnimationType animationType, float speed = 1f, int importance = 0, AnimationType? newConstantState = null)
        {

            _animationController.PlayOneShotAnimation(animationType, speed, importance, true, newConstantState);
        }
    }
}
