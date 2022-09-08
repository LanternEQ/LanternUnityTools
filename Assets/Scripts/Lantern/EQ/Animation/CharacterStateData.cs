using UnityEngine;
using Lantern.EQ.Data;

namespace Lantern.EQ.Animation
{
    /// <summary>
    /// This class contains all information needed to represent a character animation state.
    /// </summary>
    public class CharacterStateData
    {
        public bool IsDead;
        public bool IsSwimming;
        public bool IsDucking;
        public bool IsLooting;
        public bool IsStanding;
        public bool IsJumping;
        public Vector3 Velocity;
        public float RotationVelocity;
        public float WalkSpeedThreshold = EqConstants.WalkSpeedThreshold;
        public bool IsGrounded;
        public bool IsFlying;
        public bool IsClimbing;
        public AnimationType? SocialAnimation;
        public AnimationType? ActiveAnimation;
    }
}
