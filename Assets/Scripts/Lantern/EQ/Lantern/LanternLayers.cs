namespace Lantern.EQ.Lantern
{
    /// <summary>
    /// Layers the Lantern client uses for various objects in the world
    /// </summary>
    public static class LanternLayers
    {
        public static int Player => 8;
        public static int Skydome => 9;
        public static int Zone => 10;
        public static int Object => 11;
        public static int Door => 12;
        public static int Invisible => 13;
        public static int ObjectsStaticLit => 14;
        public static int ObjectsDynamicLit => 15;
        public static int ZoneRaycast => 16;
        public static int IgnoreTarget => 17;
        public static int EntityName => 18;
    }
}
