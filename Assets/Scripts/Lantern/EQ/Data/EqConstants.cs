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
        public static int PlayableRaceCount = 13;
        public static int ClassCount = 14;
        public static int FaceCount = 8;
        public static int GenderCount = 2;
        public static int MaxNameLength = 15;
        public static int DeityCount = 17;
        public static int StartingCityCount = 13;
        public static int SecondsPerDay = 4320;
        public static int SkillPointCategories = 7;
        public static int MaxLevel = 99;
        public static float TickInterval = 6.0f;
        public static int SpellEffectCount = 12;
        public static int SpellLevelLimit = 61;
        public static int RaceCount = 198;
        public static int SkyCount = 5;

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
        public const float CameraHeightIncrement = 1.24f;
        public const float MainCameraFov = 110f;
        public const float InventoryCameraFov = 86f;

        // Distance
        public static float NpcInteractionRange = 20f;
        public static float ObjectInteractionRange = 20f;
        public static float DoorInteractionRange = 20f;
        public static float ObjectPickupRange = 30f;
        public static float BeggingRange = 15f;
        public static float ExperienceRange = 500f;
        public static float GroundItemDropHeight = 0.5f;

        // Models
        public static string DefaultModelMale = "hum";
        public static string DefaultModelFemale = "huf";
        public static string DefaultItemModel = "IT63";
        public static int DefaultModelSize = 6;
        public static int ZonelineMagicNumber = 999999;

        // Music
        public static int MusicFadeQuickMs = 100;
        public static int MusicFadeMinMs = 2000;

        // Audio
        public static float AudioVolumeCharacter = 0.25f;
        public static float AudioVolumeDoor = 0.45f;
        public static float AudioVolumeSound2d = 0.1156f;
        public static float AudioVolumeSound3d = 0.2275f;
        public static double VelocityMinSound = -20f;      // TODO: Placeholder
        public static double VelocityMinDamage = -100f;     // TODO: Placeholder
        // Skills
        public static int SkillLevelMax = 252;
        public static int SkillCannotLearn = 255;
        public static int SkillNotYetLearned = 254;
        public static int SkillIdMax = 73;

        // Money
        public static int CopperPerPlatinum = 1000;
        public static int CopperPerGold = 100;
        public static int CopperPerSilver = 10;

        // Doors
        public static int DoorOpenTypeInvisible = 54;
    }
}
