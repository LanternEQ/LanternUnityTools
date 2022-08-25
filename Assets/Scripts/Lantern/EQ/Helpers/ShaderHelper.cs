namespace Lantern.EQ.Helpers
{
    public static class ShaderHelper
    {
        private static string _urpLit = "Universal Render Pipeline/Simple Lit";
        private static string _urpUnlit = "Universal Render Pipeline/Unlit";

        private static string _invisible = "EQ/Invisible";
    
        private static string _eqLit = "EQNew/EQSimpleLit";
        private static string _eqUnlit = "EQ/Unlit";
        private static string _eqSky = "EQ/Sky";

        public static string GetLitShaderName()
        {
            return _urpLit;
        }

        public static string GetUnlitShaderName()
        {
            return _urpUnlit;
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
