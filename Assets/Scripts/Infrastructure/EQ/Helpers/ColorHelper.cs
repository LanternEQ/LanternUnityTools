using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.EQ.Helpers
{
    public static class ColorHelper
    {
        public static Color GetColorFromInts(int r, int g, int b)
        {
            return new Color(r / (float)byte.MaxValue, g / (float)byte.MaxValue, b / (float)byte.MaxValue, 1.0f);
        }

        public static Color GetColorFromIntList(List<int> ints)
        {
            if (ints.Count != 3)
            {
                Debug.LogError("Unable to get color from int list. Invalid count.");
                return Color.white;
            }

            return GetColorFromInts(ints[0], ints[1], ints[2]);
        }

        public static Color GetColorFromFloatList(List<float> floats)
        {
            if (floats.Count != 3)
            {
                Debug.LogError("Unable to get color from float list. Invalid count.");
                return Color.white;
            }

            return new Color(floats[0], floats[1], floats[2], 1f);
        }
    }
}
