using System.Collections.Generic;
using UnityEngine;

namespace Lantern.Helpers
{
    // TODO: Move to own enum
    public enum AnimationType
    {
        CombatKick,
        CombatPiercing,
        Combat2HSlash,
        Combat2HBlunt,
        CombatThrowing,
        Combat1HSlash,
        CombatBash,
        CombatHandToHand,
        CombatArchery,
        CombatSwimAttack,
        CombatRoundKick,
        Damage1,
        Damage2,
        DamageTrap,
        DamageDrowningBurning,
        DamageDeath,
        LocomotionWalk,
        LocomotionRun,
        LocomotionJumpRun,
        LocomotionJumpStand,
        LocomotionFall,
        LocomotionCrouchWalk,
        LocomotionClimb,
        LocomotionCrouch,
        LocomotionSwimTread,
        PassiveStand,
        PassiveStandArmsAtSides,
        IdleStand,
        IdleStandArmsAtSides,
        PassiveSitStand,
        PassiveSitting,
        PassiveRotating,
        SocialWave,
        None,
        LocomotionSwimMove,
        IdleSit,
        SpellCast42,
        SpellCast43,
        SpellCast44,
        SocialCheer,
        SocialMourn,
        SocialRude,
        SocialYawn,
        SocialNod,
        SocialAmazed,
        SocialPlead,
        SocialClap,
        SocialDistress,
        SocialBlush,
        SocialChuckle,
        SocialBurp,
        SocialDuck,
        SocialLookAround,
        SocialDance,
        SocialBlink,
        SocialGlare,
        SocialDrool,
        SocialKneel,
        SocialLaugh,
        SocialPoint,
        SocialPonder,
        SocialReady,
        SocialSalute,
        SocialShiver,
        SocialTapFoot,
        SocialBow,
        InstrumentString,
        InstrumentWind
    }
    
    public static class AnimationHelper
    {
        private static Dictionary<string, AnimationType> _animationTypeMapping = new Dictionary<string, AnimationType>
                {
                    {"c01", AnimationType.CombatKick},
                    {"c02", AnimationType.CombatPiercing},
                    {"c03", AnimationType.Combat2HSlash},
                    {"c04", AnimationType.Combat2HBlunt},
                    {"c05", AnimationType.CombatThrowing},
                    {"c06", AnimationType.Combat1HSlash},
                    {"c07", AnimationType.CombatBash},
                    {"c08", AnimationType.CombatHandToHand},
                    {"c09", AnimationType.CombatArchery},
                    {"c10", AnimationType.CombatSwimAttack},
                    {"c11", AnimationType.CombatRoundKick},
                    {"d01", AnimationType.Damage1},
                    {"d02", AnimationType.Damage2},
                    {"d03", AnimationType.DamageTrap},
                    {"d04", AnimationType.DamageDrowningBurning},
                    {"d05", AnimationType.DamageDeath},
                    {"l01", AnimationType.LocomotionWalk},
                    {"l02", AnimationType.LocomotionRun},
                    {"l03", AnimationType.LocomotionJumpRun},
                    {"l04", AnimationType.LocomotionJumpStand},
                    {"l05", AnimationType.LocomotionFall},
                    {"l06", AnimationType.LocomotionCrouchWalk},
                    {"l07", AnimationType.LocomotionClimb},
                    {"l08", AnimationType.LocomotionCrouch},
                    {"l09", AnimationType.LocomotionSwimTread},
                    {"p01", AnimationType.PassiveStand},
                    {"p02", AnimationType.PassiveSitStand},
                    {"p08", AnimationType.PassiveStandArmsAtSides},
                    {"o01", AnimationType.IdleStand},
                    {"o02", AnimationType.IdleStandArmsAtSides},
                    {"p07", AnimationType.PassiveSitting},
                    {"o03", AnimationType.IdleSit},
                    {"p06", AnimationType.LocomotionSwimMove},
                    {"p03", AnimationType.PassiveRotating},
                    {"s01", AnimationType.SocialCheer},
                    {"s02", AnimationType.SocialMourn},
                    {"s03", AnimationType.SocialWave},
                    {"s04", AnimationType.SocialRude},
                    {"s05", AnimationType.SocialYawn},
                    {"s06", AnimationType.SocialNod},
                    {"s07", AnimationType.SocialAmazed},
                    {"s08", AnimationType.SocialPlead},
                    {"s09", AnimationType.SocialClap},
                    {"s10", AnimationType.SocialDistress},
                    {"s11", AnimationType.SocialBlush},
                    {"s12", AnimationType.SocialChuckle},
                    {"s13", AnimationType.SocialBurp},
                    {"s14", AnimationType.SocialDuck},
                    {"s15", AnimationType.SocialLookAround},
                    {"s16", AnimationType.SocialDance},
                    {"s17", AnimationType.SocialBlink},
                    {"s18", AnimationType.SocialGlare},
                    {"s19", AnimationType.SocialDrool},
                    {"s20", AnimationType.SocialKneel},
                    {"s21", AnimationType.SocialLaugh},
                    {"s22", AnimationType.SocialPoint},
                    {"s23", AnimationType.SocialPonder},
                    {"s24", AnimationType.SocialReady},
                    {"s25", AnimationType.SocialSalute},
                    {"s26", AnimationType.SocialShiver},
                    {"s27", AnimationType.SocialTapFoot},
                    {"s28", AnimationType.SocialBow},
                    {"t02", AnimationType.InstrumentString},
                    {"t03", AnimationType.InstrumentWind},
                    {"t04", AnimationType.SpellCast42},
                    {"t05", AnimationType.SpellCast43},
                    {"t06", AnimationType.SpellCast44}, // correct
                    // todo: finish
                };
        
        // TODO: Move away. Used for just the character viewer
        private static Dictionary<string, string> _animationNameMapping = new Dictionary<string, string>
        {
            {"c01", "Combat Kick"},
            {"c02", "Combat Piercing"},
            {"c03", "Combat 2H Slash"},
            {"c04", "Combat 2H Blunt"},
            {"c05", "Combat Throwing"},
            {"c06", "Combat 1H Slash Left"},
            {"c07", "Combat Bash"},
            {"c08", "Combat Hand to Hand"},
            {"c09", "Combat Archery"},
            {"c10", "Combat Swim Attack"},
            {"c11", "Combat Round Kick"},
            {"d01", "Damage 1"},
            {"d02", "Damage 2"},
            {"d03", "Damage from Trap"},
            {"d04", "Drowning/Burning"},
            {"d05", "Dying"},
            {"l01", "Walk"},
            {"l02", "Run"},
            {"l03", "Jump (Running)"},
            {"l04", "Jump (Standing)"},
            {"l05", "Falling"},
            {"l06", "Crouch Walk"},
            {"l07", "Climbing"},
            {"l08", "Crouching"},
            {"l09", "Swim Treading"},
            {"o01", "Idle"},
            {"s01", "Cheer"},
            {"s02", "Mourn"},
            {"s03", "Wave"},
            {"s04", "Rude"},
            {"s05", "Yawn"},
            {"p01", "Stand"},
            {"p02", "Sit/Stand"},
            {"p03", "Shuffle Feet"},
            {"p04", "Float/Walk/Strafe"},
            {"p05", "Kneel"},
            {"p06", "Swim"},
            {"p07", "Sitting"},
            {"t01", "UNUSED????"},
            {"t02", "Stringed Instrument"},
            {"t03", "Wind Instrument"},
            {"t04", "Cast Pull Back"},
            {"t05", "Raise and Loop Arms"},
            {"t06", "Cast Push Forward"},
            {"t07", "Flying Kick"},
            {"t08", "Rapid Punches"},
            {"t09", "Large Punch"},
            {"s06", "Nod"},
            {"s07", "Amazed"},
            {"s08", "Plead"},
            {"s09", "Clap"},
            {"s10", "Distress"},
            {"s11", "Blush"},
            {"s12", "Chuckle"},
            {"s13", "Burp"},
            {"s14", "Duck"},
            {"s15", "Look Around"},
            {"s16", "Dance"},
            {"s17", "Blink"},
            {"s18", "Glare"},
            {"s19", "Drool"},
            {"s20", "Kneel"},
            {"s21", "Laugh"},
            {"s22", "Point"},
            {"s23", "Ponder"},
            {"s24", "Ready"},
            {"s25", "Salute"},
            {"s26", "Shiver"},
            {"s27", "Tap Foot"},
            {"s28", "Bow"},
            {"p08", "Stand (Arms at Sides)"},
            {"o02", "Idle (Arms at Sides)"},
            {"o03", "Idle (Sitting)"},
            {"pos", "Pose"},
            {"drf", "Pose"}, // tentacle pose
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
                case "sky":
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
    }
}