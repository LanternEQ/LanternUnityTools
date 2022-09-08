namespace Lantern.EQ.Animation
{
    /// <summary>
    /// Animation states a character can be in.
    /// Not all characters have all animations.
    /// </summary>
    public enum CharacterAnimationState
    {
        None = 0, // on entry
        Dead = 1,
        Falling = 3,
        RotatingRight = 4,
        RotatingLeft = 5,
        Standing = 9,
        StandingArmsAtSide = 10,
        Sitting = 11,
        DuckingStill = 12,
        DuckingMoving = 13,
        SwimmingStill = 14,
        SwimmingMoving = 15,
        Looting = 16,
        JumpingStill = 17,
        JumpingMoving = 18,
        WalkingBackwards = 19,
        Running = 20,
        Walking = 21,
        Climbing = 22
    }
}
