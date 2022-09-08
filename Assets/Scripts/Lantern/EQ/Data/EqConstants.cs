namespace Lantern.EQ.Data
{
    /// <summary>
    /// Constants from the Trilogy EQ client
    /// </summary>
    public static class EqConstants
    {
        public static float DayStartTime = 0.25f;
        public static float NightStartTime = 0.75f;
        public static int MaxCharacterCount = 8;
        public static int RaceCount = 13;
        public static int ClassCount = 14;
        public static int FaceCount = 8;
        public static int GenderCount = 2;
        public static int MaxNameLength = 15;
        public static int DeityCount = 17;
        public static int StartingCityCount = 13;
        public static int SecondsPerDay = 4320;
        public static int SkillPointCategories = 7;
        public static int MaxLevel = 99;

        // Movement
        public static float Velocity = 42.65f;
        public static float SpeedWalk = 0.46f;
        public static float SpeedRun = 0.7f;
        public static float SpeedDuck = 0.23f;
        public static float Acceleration = 30f;
        public static float Gravity = -120f;
        public static float GravityWater = -10f;
        public static float SpeedJump = 20f;
        public static float SpeedSwim = 0.3f;
        public static float AccelerationWater = 5f;
        public static float TerminalVelocity = -200f;
        public static float SpeedRotation = 200f;

        // Animation
        public static float FallVelocityThreshold = -60f; // placeholder
        public static float WalkSpeedThreshold = 0.46f;
        public static float AnimationSpeedBase = 1f;
        public static float AnimationSpeedMultiplier = 1.066666126f;

        // Camera
        public const float CameraPivotIncrement = 1.24f;

        // Distance
        public static float NpcInteractionRange = 20f;
        public static float ObjectInteractionRange = 20f;
        public static float DoorInteractionRange = 20f;
        public static float BeggingRange = 15f;
    }
}
