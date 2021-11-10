using System.Collections.Generic;
using Lantern.EQ.Animation;
using Lantern.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lantern
{
    public class AnimationStateSetter : MonoBehaviour
    {
        [SerializeField] private UniversalAnimationController _animationController;

        private PlayerAnimationState _currentState;
        private PlayerAnimationState _lastState;

        private float _lastMoveAxisForward;

        private float _currentIdleTimer = 0.0f;


        private bool _isRelaxed = false;

        private void Start()
        {
            _animationController = GetComponentInChildren<UniversalAnimationController>();
            _lastState = PlayerAnimationState.None;
            ResetIdleTimer();
        }
        
        private void OnShowAnimationDebug(ShowAnimationDebugMessage message)
        {
            _showAnimationDebug = true;
        }

        private void PlaySocialAnimation(AnimationType animation)
        {
            _animationController.PlayOneShotAnimation(animation, 1.0f, AnimationImportance.Social,
                AnimationType.PassiveStandArmsAtSides);
            ResetIdleTimer();
        }

        public void SetInputs(/*ref PlayerCharacterInputs inputs*/)
        {
            if (_animationController == null)
            {
                return;
            }
            
            /*_currentState = GetCurrentState(inputs);

            if (_currentState != PlayerAnimationState.Standing && _currentState != PlayerAnimationState.Sitting)
            {
                ResetIdleTimer();
            }

            if (_showAnimationDebug && IsNewState(_currentState))
            {
                ServiceFactory.Get<ConsoleService>().AddMessage(_currentState.ToString(), Color.white);
            }*/

            // Death
            if (IsNewState(PlayerAnimationState.Dead))
            {
                _animationController.SetNewConstantState(AnimationType.DamageDeath, AnimationImportance.Death);
            }
            
            if (IsNewState(PlayerAnimationState.SwimmingStill))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionSwimTread, AnimationImportance.Movement);
            }
            
            if (IsNewState(PlayerAnimationState.SwimmingMoving))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionSwimMove, AnimationImportance.Movement);
            }

            if (IsNewState(PlayerAnimationState.Falling))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionFall, AnimationImportance.Movement);
            }

            // Sitting / standing
            if (IsNewState(PlayerAnimationState.Sitting))
            {
                _animationController.PlayOneShotAnimation(AnimationType.PassiveSitStand, 1.0f, 1,
                    AnimationType.PassiveSitting);
                // _animationController.SetNewConstantState(AnimationType.PassiveSitting);
                ResetIdleTimer();
            }

            if (IsNewState(PlayerAnimationState.CrouchingMoving))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionCrouchWalk, AnimationImportance.Movement, 1f);
            }

            if (IsNewState(PlayerAnimationState.Standing))
            {
                if (_lastState == PlayerAnimationState.Sitting)
                {
                    _animationController.PlayOneShotAnimation(AnimationType.PassiveSitStand, -1.0f, 1,
                        AnimationType.PassiveStandArmsAtSides);
                }
                else if (_lastState == PlayerAnimationState.CrouchingStill)
                {
                    _animationController.PlayOneShotAnimation(AnimationType.LocomotionCrouch, -1.0f, 1,
                        AnimationType.PassiveStand);
                }
                else
                {
                    if (_lastState == PlayerAnimationState.RotatingLeft ||
                        _lastState == PlayerAnimationState.RotatingRight)
                    {
                        _animationController.ResolveCurrentStateWhenFinished(AnimationType.PassiveStand);
                    }


                    _animationController.SetNewConstantStateIfNot(AnimationType.PassiveStand,
                        AnimationType.PassiveStandArmsAtSides, AnimationImportance.Movement);
                }

                ResetIdleTimer();
            }

            /*if (IsNewState(PlayerAnimationState.Running))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionRun, AnimationImportance.Movement, 
                    AnimationSpeedHelper.GetRunSpeedTimeScale(_playerStateService.SpeedMultiplier));
                ResetIdleTimer();
            }

            if (IsNewState(PlayerAnimationState.Walking))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionWalk, AnimationImportance.Movement,
                    AnimationSpeedHelper.GetWalkSpeedTimeScale(_playerStateService.SpeedMultiplier));
                ResetIdleTimer();
            }*/

            if (IsNewState(PlayerAnimationState.WalkingBackwards))
            {
                _animationController.SetNewConstantState(AnimationType.LocomotionWalk, AnimationImportance.Movement,
                    -1f);
                ResetIdleTimer();
            }

            if (IsNewState(PlayerAnimationState.RotatingRight))
            {
                _animationController.SetNewConstantState(AnimationType.PassiveRotating, AnimationImportance.Rotating);
                ResetIdleTimer();
            }

            if (IsNewState(PlayerAnimationState.RotatingLeft))
            {
                _animationController.SetNewConstantState(AnimationType.PassiveRotating, AnimationImportance.Rotating,
                    -1f);
                ResetIdleTimer();
            }

            if (IsNewState(PlayerAnimationState.CrouchingStill))
            {
                if (_lastState != PlayerAnimationState.CrouchingMoving)
                {
                    _animationController.PlayOneShotAnimation(AnimationType.LocomotionCrouch, 1.0f, 1,
                        AnimationType.None);
                    ResetIdleTimer();
                }
                else
                {
                    _animationController.ResolveCurrentStateWhenFinished(AnimationType.None);
                }
            }

            _lastState = _currentState;
        }

        public void DoOneShotTest(AnimationType type, float speed = 1.0f)
        {
            _animationController.PlayOneShotAnimation(type, speed);
        }

        private bool IsNewState(PlayerAnimationState state)
        {
            return _currentState == state && _lastState != state;
        }

        public enum PlayerAnimationState
        {
            None, // on entry
            Dead,
            InAir,
            Falling,
            RotatingRight,
            RotatingLeft,
            Running,
            Walking,
            WalkingBackwards,
            Standing,
            StandingArmsAtSide,
            Sitting,
            CrouchingStill,
            CrouchingMoving,
            SwimmingStill,
            SwimmingMoving,
        }

        public float _fallSpeedThreshold = -10f;
        private bool _showAnimationDebug;

        /*private PlayerAnimationState GetCurrentState(PlayerCharacterInputs inputs)
        {
            if (_playerStateService._isDead)
            {
                return PlayerAnimationState.Dead;
            }

            if (_playerStateService._swimming)
            {
                if (_motor.Velocity.x == 0f && _motor.Velocity.z == 0)
                {
                    return PlayerAnimationState.SwimmingStill;
                }
                else
                {
                    return PlayerAnimationState.SwimmingMoving;
                }
            }

            if (!_motor.GroundingStatus.IsStableOnGround && !_playerStateService._isFlying)
            {
                return _motor.Velocity.y < _fallSpeedThreshold
                    ? PlayerAnimationState.Falling
                    : PlayerAnimationState.InAir;
            }

            if (!_playerStateService._isStanding)
            {
                return PlayerAnimationState.Sitting;
            }

            if (inputs.MoveAxisForward != 0f || inputs.MoveAxisRight != 0f)
            {
                if (_playerStateService._isCrouching)
                {
                    return PlayerAnimationState.CrouchingMoving;
                }

                if (inputs.MoveAxisForward < 0f)
                {
                    return PlayerAnimationState.WalkingBackwards;
                }

                return _playerStateService._isRunning ? PlayerAnimationState.Running : PlayerAnimationState.Walking;
            }

            if (inputs.RotateAxis != 0)
            {
                if (_playerStateService._isCrouching)
                {
                    return PlayerAnimationState.CrouchingMoving;
                }

                return inputs.RotateAxis > 0f ? PlayerAnimationState.RotatingRight : PlayerAnimationState.RotatingLeft;
            }

            if (inputs.RotateAxisMouse != 0)
            {
                if (_playerStateService._isCrouching)
                {
                    return PlayerAnimationState.CrouchingMoving;
                }

                return inputs.RotateAxisMouse > 0f
                    ? PlayerAnimationState.RotatingRight
                    : PlayerAnimationState.RotatingLeft;
            }

            if (_playerStateService._isCrouching)
            {
                return PlayerAnimationState.CrouchingStill;
            }

            return PlayerAnimationState.Standing;
        }*/

        /*private void HandleLand(PlayerCharacterInputs inputs)
        {
        }*/

        private void Update()
        {
            if (_animationController == null)
            {
                return;
            }
            
            if (_currentState == PlayerAnimationState.Sitting || _currentState == PlayerAnimationState.Standing || _currentState == PlayerAnimationState.StandingArmsAtSide)
                HandleIdleCheck();
        }

        private void HandleIdleCheck()
        {
            _currentIdleTimer -= Time.deltaTime;
            
            if (_currentIdleTimer <= 0.0f)
            {
                bool hasSecondIdle = _animationController.HasAnimation(AnimationType.PassiveStandArmsAtSides);

                if (hasSecondIdle)
                {
                    AnimationType oneShot = AnimationType.None;
                    AnimationType resolutionState = AnimationType.None;
                    
                    if (_currentState == PlayerAnimationState.Sitting)
                    {
                        oneShot = AnimationType.IdleSit;
                        resolutionState = AnimationType.PassiveSitting;
                    }
                    else
                    {
                        oneShot = _isRelaxed ? AnimationType.IdleStandArmsAtSides : AnimationType.IdleStand;
                        resolutionState = AnimationType.PassiveStandArmsAtSides;
                    }
                    
                    _animationController.PlayOneShotAnimation(oneShot, 1f, 0, resolutionState);
                    ResetIdleTimer(false);
                    _isRelaxed = true;
                }
                else
                {
                    _animationController.PlayOneShotAnimation(AnimationType.IdleStand);
                    ResetIdleTimer(false);
                }
            }
        }

        /*private bool IsStill(PlayerCharacterInputs inputs)
        {
            return inputs.MoveAxisForward == 0f && inputs.MoveAxisRight == 0f && inputs.RotateAxis == 0f;
        }

        private bool IsJustRotating(PlayerCharacterInputs inputs)
        {
            return inputs.MoveAxisForward == 0f && inputs.MoveAxisRight == 0f && inputs.RotateAxis != 0;
        }*/

        private void ResetIdleTimer(bool hasMoved = true)
        {
            _currentIdleTimer = Random.Range(5.0f, 20f);

            if (hasMoved)
            {
                _isRelaxed = false;
            }
        }

        /*public void DoJump(JumpType type)
        {
            if (_animationController == null)
            {
                return;
            }
            
            if (type == JumpType.Still)
            {
                _animationController.PlayOneShotSequence(new List<AnimationType>
                        {AnimationType.LocomotionCrouch, AnimationType.LocomotionJumpStand},
                    AnimationType.PassiveStandArmsAtSides);
            }
            else
            {
                _animationController.PlayOneShotAnimation(AnimationType.LocomotionJumpRun);
            }
        }*/

        public void SetUniversalAnimationController(UniversalAnimationController controller)
        {
            _animationController = controller;
        }
    }

    internal class ShowAnimationDebugMessage
    {
    }
}