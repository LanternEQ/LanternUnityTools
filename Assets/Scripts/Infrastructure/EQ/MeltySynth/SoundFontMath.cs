﻿using System;

namespace Infrastructure.EQ.MeltySynth
{
    internal static class SoundFontMath
    {
        public const float HalfPi = MathF.PI / 2;

        public static readonly float NonAudible = 1.0E-3F;

        private static readonly double logNonAudible = Math.Log(1.0E-3);

        public static float TimecentsToSeconds(float x)
        {
            return MathF.Pow(2F, (1F / 1200F) * x);
        }

        public static float CentsToHertz(float x)
        {
            return 8.176F * MathF.Pow(2F, (1F / 1200F) * x);
        }

        public static float CentsToMultiplyingFactor(float x)
        {
            return MathF.Pow(2F, (1F / 1200F) * x);
        }

        public static float DecibelsToLinear(float x)
        {
            return MathF.Pow(10F, 0.05F * x);
        }

        public static float LinearToDecibels(float x)
        {
            return 20F * MathF.Log10(x);
        }

        public static float KeyNumberToMultiplyingFactor(int cents, int key)
        {
            return TimecentsToSeconds(cents * (60 - key));
        }

        public static double ExpCutoff(double x)
        {
            if (x < logNonAudible)
            {
                return 0.0;
            }
            else
            {
                return Math.Exp(x);
            }
        }
    }
}
