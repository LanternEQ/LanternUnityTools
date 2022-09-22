using System.Collections.Generic;
using Lantern.EQ.Data;
using UnityEngine;

namespace Lantern.EQ.Animation
{
    public static class AnimationHelper
    {
        private static Dictionary<string, AnimationType> _animationTypeMapping = new Dictionary<string, AnimationType>
        {
            { "c01", AnimationType.CombatKick },
            { "c02", AnimationType.CombatPiercing },
            { "c03", AnimationType.Combat2HSlash },
            { "c04", AnimationType.Combat2HBlunt },
            { "c05", AnimationType.CombatThrowing },
            { "c06", AnimationType.Combat1HSlash },
            { "c07", AnimationType.CombatBash },
            { "c08", AnimationType.CombatHandToHand },
            { "c09", AnimationType.CombatArchery },
            { "c10", AnimationType.CombatSwimAttack },
            { "c11", AnimationType.CombatRoundKick },
            { "d01", AnimationType.Damage1 },
            { "d02", AnimationType.Damage2 },
            { "d03", AnimationType.DamageTrap },
            { "d04", AnimationType.DamageDrowningBurning },
            { "d05", AnimationType.DamageDeath },
            { "l01", AnimationType.LocomotionWalk },
            { "l01R", AnimationType.LocomotionWalkReverse },
            { "l02", AnimationType.LocomotionRun },
            { "l02R", AnimationType.LocomotionRunReverse },
            { "l03", AnimationType.LocomotionJumpRun },
            { "l04", AnimationType.LocomotionJumpStand },
            { "l05", AnimationType.LocomotionFall },
            { "l06", AnimationType.LocomotionDuckWalk },
            { "l07", AnimationType.LocomotionClimb },
            { "l08", AnimationType.Duck },
            { "l09", AnimationType.LocomotionSwimTread },
            { "p01", AnimationType.PassiveStand },
            { "p02", AnimationType.PassiveSitStand },
            { "p02R", AnimationType.PassiveStandSit },
            { "p05", AnimationType.Loot },
            { "p05R", AnimationType.LootReverse },
            { "p08", AnimationType.PassiveStandArmsAtSides },
            { "o01", AnimationType.IdleStand },
            { "o02", AnimationType.IdleStandArmsAtSides },
            { "p07", AnimationType.PassiveSitting },
            { "o03", AnimationType.IdleSit },
            { "p06", AnimationType.LocomotionSwimMove },
            { "p03", AnimationType.PassiveRotating },
            { "p03R", AnimationType.PassiveRotatingReverse },
            { "s01", AnimationType.SocialCheer },
            { "s02", AnimationType.SocialMourn },
            { "s03", AnimationType.SocialWave },
            { "s04", AnimationType.SocialRude },
            { "s05", AnimationType.SocialYawn },
            { "s06", AnimationType.SocialNod },
            { "s07", AnimationType.SocialAmazed },
            { "s08", AnimationType.SocialPlead },
            { "s09", AnimationType.SocialClap },
            { "s10", AnimationType.SocialDistress },
            { "s11", AnimationType.SocialBlush },
            { "s12", AnimationType.SocialChuckle },
            { "s13", AnimationType.SocialBurp },
            { "s14", AnimationType.SocialDuck },
            { "s15", AnimationType.SocialLookAround },
            { "s16", AnimationType.SocialDance },
            { "s17", AnimationType.SocialBlink },
            { "s18", AnimationType.SocialGlare },
            { "s19", AnimationType.SocialDrool },
            { "s20", AnimationType.SocialKneel },
            { "s21", AnimationType.SocialLaugh },
            { "s22", AnimationType.SocialPoint },
            { "s23", AnimationType.SocialPonder },
            { "s24", AnimationType.SocialReady },
            { "s25", AnimationType.SocialSalute },
            { "s26", AnimationType.SocialShiver },
            { "s27", AnimationType.SocialTapFoot },
            { "s28", AnimationType.SocialBow },
            { "t02", AnimationType.InstrumentString },
            { "t03", AnimationType.InstrumentWind },
            { "t04", AnimationType.SpellCast42 },
            { "t05", AnimationType.SpellCast43 },
            { "t06", AnimationType.SpellCast44 }, // correct
            { "t07", AnimationType.CombatFlyingKick },
            { "t08", AnimationType.CombatRapidPunch },
            { "t09", AnimationType.CombatHeavyPunch },
            { "l08R", AnimationType.DuckReverse },
            // todo: finish
        };

        // TODO: Move away. Used for just the character viewer
        private static Dictionary<string, string> _animationNameMapping = new Dictionary<string, string>
        {
            { "c01", "Combat Kick" },
            { "c02", "Combat Piercing" },
            { "c03", "Combat 2H Slash" },
            { "c04", "Combat 2H Blunt" },
            { "c05", "Combat Throwing" },
            { "c06", "Combat 1H Slash Left" },
            { "c07", "Combat Bash" },
            { "c08", "Combat Hand to Hand" },
            { "c09", "Combat Archery" },
            { "c10", "Combat Swim Attack" },
            { "c11", "Combat Round Kick" },
            { "d01", "Damage 1" },
            { "d02", "Damage 2" },
            { "d03", "Damage from Trap" },
            { "d04", "Drowning/Burning" },
            { "d05", "Dying" },
            { "l01", "Walk" },
            { "l01R", "Walk (Reverse)" },
            { "l02", "Run" },
            { "l02R", "Run (Reverse)" },
            { "l03", "Jump (Running)" },
            { "l04", "Jump (Standing)" },
            { "l05", "Falling" },
            { "l06", "Crouch Walk" },
            { "l07", "Climbing" },
            { "l08", "Crouching" },
            { "l08R", "Crouching (Reverse)" },
            { "l09", "Swim Treading" },
            { "o01", "Idle" },
            { "s01", "Cheer" },
            { "s02", "Mourn" },
            { "s03", "Wave" },
            { "s04", "Rude" },
            { "s05", "Yawn" },
            { "p01", "Stand" },
            { "p02", "Sit/Stand" },
            { "p02R", "Sit/Stand (Reverse)" },
            { "p03", "Shuffle Feet" },
            { "p03R", "Shuffle Feet (Reverse)" },
            { "p04", "Float/Walk/Strafe" },
            { "p05", "Kneel" },
            { "p05R", "Kneel (Reverse)" },
            { "p06", "Swim" },
            { "p07", "Sitting" },
            { "t01", "UNUSED????" },
            { "t02", "Stringed Instrument" },
            { "t03", "Wind Instrument" },
            { "t04", "Cast Pull Back" },
            { "t05", "Raise and Loop Arms" },
            { "t06", "Cast Push Forward" },
            { "t07", "Flying Kick" },
            { "t08", "Rapid Punches" },
            { "t09", "Large Punch" },
            { "s06", "Nod" },
            { "s07", "Amazed" },
            { "s08", "Plead" },
            { "s09", "Clap" },
            { "s10", "Distress" },
            { "s11", "Blush" },
            { "s12", "Chuckle" },
            { "s13", "Burp" },
            { "s14", "Duck" },
            { "s15", "Look Around" },
            { "s16", "Dance" },
            { "s17", "Blink" },
            { "s18", "Glare" },
            { "s19", "Drool" },
            { "s20", "Kneel" },
            { "s21", "Laugh" },
            { "s22", "Point" },
            { "s23", "Ponder" },
            { "s24", "Ready" },
            { "s25", "Salute" },
            { "s26", "Shiver" },
            { "s27", "Tap Foot" },
            { "s28", "Bow" },
            { "p08", "Stand (Arms at Sides)" },
            { "o02", "Idle (Arms at Sides)" },
            { "o03", "Idle (Sitting)" },
            { "pos", "Pose" },
            { "drf", "Pose" }, // tentacle pose
        };

        public static AnimationPair AnimationFallbacks = new AnimationPair
        {
            // target => fallback
            { AnimationType.LocomotionWalk, AnimationType.LocomotionRun },
            { AnimationType.LocomotionWalkReverse, AnimationType.LocomotionRunReverse },
            { AnimationType.PassiveStandArmsAtSides, AnimationType.PassiveStand },
            { AnimationType.IdleStand, AnimationType.PassiveStand },
            { AnimationType.IdleStandArmsAtSides, AnimationType.PassiveStandArmsAtSides },
            { AnimationType.IdleSit, AnimationType.PassiveSitting },
        };

        public static string GetAnimationName(string animation)
        {
            return !_animationNameMapping.ContainsKey(animation) ? "<Missing>" : _animationNameMapping[animation];
        }

        public static bool IsLoopingAnimation(string animationName)
        {
            switch (animationName)
            {
                case "pos": // name is used for animated objects
                case "p01": // Stand
                case "l01": // Walk
                case "l02": // Run
                case "l05": // falling
                case "l06": // crouch walk
                case "l07": // climbing
                case "l09": // swim treading
                case "p03": // rotating
                case "p06": // swim
                case "p07": // sitting
                case "p08": // stand (arms at sides)
                    return true;
                default:
                    return false;
            }
        }

        public static AnimationType? GetAnimationType(string animationSuffix)
        {
            if (!_animationTypeMapping.ContainsKey(animationSuffix))
            {
                Debug.LogWarning("AnimationHelper: Unable to get animation type: " + animationSuffix);
                return null;
            }

            return _animationTypeMapping[animationSuffix];
        }

        public static bool IsVariableSpeedState(CharacterAnimationState state)
        {
            switch (state)
            {
                case CharacterAnimationState.Running:
                case CharacterAnimationState.Walking:
                case CharacterAnimationState.WalkingBackwards:
                case CharacterAnimationState.DuckingMoving:
                case CharacterAnimationState.SwimmingMoving:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsReverseAnimationNeeded(string animationName)
        {
            if (!_animationTypeMapping.TryGetValue(animationName, out var value))
            {
                return false;
            }

            switch (value)
            {
                case AnimationType.LocomotionWalk:
                case AnimationType.LocomotionRun:
                case AnimationType.Duck:
                case AnimationType.PassiveSitStand:
                case AnimationType.PassiveRotating:
                case AnimationType.Loot:
                case AnimationType.PassiveStandSit:
                    return true;
                default:
                    return false;
            }
        }

        public static AnimationSpeed GetAnimationSpeed(CharacterStateData data)
        {
            float speed = GetAnimationSpeed(Mathf.Abs(data.Velocity.x));

            if (data.IsSwimming)
            {
                return new AnimationSpeed { AnimationType = AnimationType.LocomotionSwimMove, Speed = speed };
            }

            if (data.Velocity.x > data.WalkSpeedThreshold)
            {
                return new AnimationSpeed { AnimationType = AnimationType.LocomotionRun, Speed = speed };
            }

            if (data.IsDucking)
            {
                return new AnimationSpeed
                    { AnimationType = AnimationType.LocomotionDuckWalk, Speed = Mathf.Abs(speed) };
            }

            return new AnimationSpeed
            {
                AnimationType = data.Velocity.x > 0f
                    ? AnimationType.LocomotionWalk
                    : AnimationType.LocomotionWalkReverse,
                Speed = speed
            };
        }

        private static float GetAnimationSpeed(float velocity)
        {
            return EqConstants.AnimationSpeedBase + velocity * EqConstants.AnimationSpeedMultiplier;
        }

        public static bool TrySplitAnimationName(string animationName, out string modelBase, out string animationType)
        {
            var parts = animationName.Split('_');
            if (parts.Length != 2)
            {
                modelBase = animationType = string.Empty;
                Debug.LogError($"Invalid animation name: {animationName}");
                return false;
            }

            modelBase = parts[0];
            animationType = parts[1];
            return true;
        }

        public static string GetDebugName(AnimationType animationType)
        {
            string animName = string.Empty;
            foreach (var animPair in _animationTypeMapping)
            {
                if (animationType == animPair.Value)
                {
                    animName = animPair.Key;
                    break;
                }
            }

            return animationType + $" ({animName})";
        }

        public static AnimationType? GetSpellAnimation(int index)
        {
            switch (index)
            {
                case 40:
                    return AnimationType.InstrumentWind;
                case 41:
                    return AnimationType.InstrumentString;
                case 42:
                    return AnimationType.SpellCast42;
                case 43:
                    return AnimationType.SpellCast43;
                case 44:
                    return AnimationType.SpellCast44;
            }

            return null;
        }
    }
}
