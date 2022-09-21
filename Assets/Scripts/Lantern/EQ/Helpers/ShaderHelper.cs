namespace Lantern.EQ.Helpers
{
    public static class ShaderHelper
    {
        // These are the shaders you want when using URP.
        // Every installation should have this.
        private static string _urpLit = "Universal Render Pipeline/Simple Lit";
        private static string _urpUnlit = "Universal Render Pipeline/Unlit";

        // Only compatible with URP 10.0.0+
        private static string _eqLit = "EQ/EQSimpleLit";
        private static string _eqUnlit = "EQ/EQUnlit";

        // No URP version dependencies.
        private static string _invisible = "EQ/EQInvisible";
        private static string _eqSky = "EQ/EQSky";

        public static string GetLitShaderName()
        {
            return _eqLit;
        }

        public static string GetUnlitShaderName()
        {
            return _eqUnlit;
        }

        public static string GetInvisibleShaderName()
        {
            return _invisible;
        }

        public static string GetSkyShaderName()
        {
            return _eqSky;
        }
    }
}
