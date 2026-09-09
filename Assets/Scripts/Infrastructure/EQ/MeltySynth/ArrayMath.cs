using System.Numerics;
using System.Runtime.InteropServices;

namespace Infrastructure.EQ.MeltySynth
{
    internal static class ArrayMath
    {
        public static void MultiplyAdd(float a, float[] x, float[] destination)
        {
            // LANTERN: use older implementation with il2cpp
            // See also: https://github.com/homy-game-studio/hgs-unity-tone/issues/4
#if ENABLE_IL2CPP
            for (var i = 0; i < destination.Length; i++)
            {
                destination[i] += a * x[i];
            }
            return;
#endif

            var vx = MemoryMarshal.Cast<float, Vector<float>>(x);
            var vd = MemoryMarshal.Cast<float, Vector<float>>(destination);

            var count = 0;

            for (var i = 0; i < vd.Length; i++)
            {
                vd[i] += a * vx[i];
                count += Vector<float>.Count;
            }

            for (var i = count; i < destination.Length; i++)
            {
                destination[i] += a * x[i];
            }
        }

        public static void MultiplyAdd(float a, float step, float[] x, float[] destination)
        {
            for (var i = 0; i < destination.Length; i++)
            {
                destination[i] += a * x[i];
                a += step;
            }
        }
    }
}
