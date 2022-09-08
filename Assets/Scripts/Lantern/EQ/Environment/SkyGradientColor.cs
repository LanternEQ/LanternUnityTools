using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lantern.EQ.Environment
{
    public static class SkyGradientColor
    {
        private static int _hourCount = 24;

        private static List<KeyValuePair<float, Color>> _hourColorsHorizon = new List<KeyValuePair<float, Color>>()
        {
            new KeyValuePair<float, Color>(0.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 82 / 255)), // 0
            new KeyValuePair<float, Color>(1.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 82 / 255)), // 1
            new KeyValuePair<float, Color>(2.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 82 / 255)), // 2
            new KeyValuePair<float, Color>(3.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 82 / 255)), // 3
            new KeyValuePair<float, Color>(4.0f * 1.0f / _hourCount,
                new Color((float) 150 / 255, (float) 22 / 255, (float) 58 / 255)), // 4
            new KeyValuePair<float, Color>(5.0f * 1.0f / _hourCount,
                new Color((float) 190 / 255, (float) 26 / 255, (float) 22 / 255)), // 5
            new KeyValuePair<float, Color>(6.0f * 1.0f / _hourCount,
                new Color((float) 229 / 255, (float) 84 / 255, (float) 22 / 255)), // 6
            new KeyValuePair<float, Color>(7.0f * 1.0f / _hourCount,
                new Color((float) 234 / 255, (float) 86 / 255, (float) 138 / 255)), // 7
            new KeyValuePair<float, Color>(8.0f * 1.0f / _hourCount,
                new Color((float) 198 / 255, (float) 158 / 255, (float) 242 / 255)), // 8
            new KeyValuePair<float, Color>(9.0f * 1.0f / _hourCount,
                new Color((float) 238 / 255, (float) 250 / 255, (float) 254 / 255)), // 9
            new KeyValuePair<float, Color>(10.0f * 1.0f / _hourCount,
                new Color((float) 238 / 255, (float) 250 / 255, (float) 254 / 255)), // 10
            new KeyValuePair<float, Color>(11.0f * 1.0f / _hourCount,
                new Color((float) 238 / 255, (float) 250 / 255, (float) 254 / 255)), // 11
            new KeyValuePair<float, Color>(12.0f * 1.0f / _hourCount,
                new Color((float) 238 / 255, (float) 250 / 255, (float) 254 / 255)), // 12
            new KeyValuePair<float, Color>(13.0f * 1.0f / _hourCount,
                new Color((float) 238 / 255, (float) 250 / 255, (float) 254 / 255)), // 13
            new KeyValuePair<float, Color>(14.0f * 1.0f / _hourCount,
                new Color((float) 238 / 255, (float) 250 / 255, (float) 254 / 255)), // 14
            new KeyValuePair<float, Color>(15.0f * 1.0f / _hourCount,
                new Color((float) 238 / 255, (float) 250 / 255, (float) 254 / 255)), // 15
            new KeyValuePair<float, Color>(16.0f * 1.0f / _hourCount,
                new Color((float) 178 / 255, (float) 154 / 255, (float) 230 / 255)), // 16
            new KeyValuePair<float, Color>(17.0f * 1.0f / _hourCount,
                new Color((float) 206 / 255, (float) 86 / 255, (float) 174 / 255)), // 17
            new KeyValuePair<float, Color>(18.0f * 1.0f / _hourCount,
                new Color((float) 186 / 255, (float) 62 / 255, (float) 34 / 255)), // 18
            new KeyValuePair<float, Color>(19.0f * 1.0f / _hourCount,
                new Color((float) 150 / 255, (float) 14 / 255, (float) 58 / 255)), // 19
            new KeyValuePair<float, Color>(20.0f * 1.0f / _hourCount,
                new Color((float) 98 / 255, (float) 6 / 255, (float) 118 / 255)), // 20
            new KeyValuePair<float, Color>(21.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 82 / 255)), // 21
            new KeyValuePair<float, Color>(22.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 82 / 255)), // 22
            new KeyValuePair<float, Color>(23.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 82 / 255)), // 23
        };

        private static List<KeyValuePair<float, Color>> _hourColorsPoleSky = new List<KeyValuePair<float, Color>>()
        {
            new KeyValuePair<float, Color>(0.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 51 / 255)), // 0
            new KeyValuePair<float, Color>(1.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 51 / 255)), // 1
            new KeyValuePair<float, Color>(2.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 51 / 255)), // 2
            new KeyValuePair<float, Color>(3.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 51 / 255)), // 3
            new KeyValuePair<float, Color>(4.0f * 1.0f / _hourCount,
                new Color((float) 82 / 255, (float) 34 / 255, (float) 97 / 255)), // 4
            new KeyValuePair<float, Color>(5.0f * 1.0f / _hourCount,
                new Color((float) 119 / 255, (float) 46 / 255, (float) 146 / 255)), // 5
            new KeyValuePair<float, Color>(6.0f * 1.0f / _hourCount,
                new Color((float) 179 / 255, (float) 102 / 255, (float) 180 / 255)), // 6
            new KeyValuePair<float, Color>(7.0f * 1.0f / _hourCount,
                new Color((float) 143 / 255, (float) 198 / 255, (float) 219 / 255)), // 7
            new KeyValuePair<float, Color>(8.0f * 1.0f / _hourCount,
                new Color((float) 138 / 255, (float) 160 / 255, (float) 234 / 255)), // 8
            new KeyValuePair<float, Color>(9.0f * 1.0f / _hourCount,
                new Color((float) 184 / 255, (float) 238 / 255, (float) 246 / 255)), // 9
            new KeyValuePair<float, Color>(10.0f * 1.0f / _hourCount,
                new Color((float) 184 / 255, (float) 238 / 255, (float) 246 / 255)), // 10
            new KeyValuePair<float, Color>(11.0f * 1.0f / _hourCount,
                new Color((float) 184 / 255, (float) 238 / 255, (float) 246 / 255)), // 11
            new KeyValuePair<float, Color>(12.0f * 1.0f / _hourCount,
                new Color((float) 184 / 255, (float) 238 / 255, (float) 246 / 255)), // 12
            new KeyValuePair<float, Color>(13.0f * 1.0f / _hourCount,
                new Color((float) 184 / 255, (float) 238 / 255, (float) 246 / 255)), // 13
            new KeyValuePair<float, Color>(14.0f * 1.0f / _hourCount,
                new Color((float) 184 / 255, (float) 238 / 255, (float) 246 / 255)), // 14
            new KeyValuePair<float, Color>(15.0f * 1.0f / _hourCount,
                new Color((float) 184 / 255, (float) 238 / 255, (float) 246 / 255)), // 15
            new KeyValuePair<float, Color>(16.0f * 1.0f / _hourCount,
                new Color((float) 111 / 255, (float) 122 / 255, (float) 207 / 255)), // 16
            new KeyValuePair<float, Color>(17.0f * 1.0f / _hourCount,
                new Color((float) 130 / 255, (float) 55 / 255, (float) 163 / 255)), // 17
            new KeyValuePair<float, Color>(18.0f * 1.0f / _hourCount,
                new Color((float) 121 / 255, (float) 15 / 255, (float) 76 / 255)), // 18
            new KeyValuePair<float, Color>(19.0f * 1.0f / _hourCount,
                new Color((float) 95 / 255, (float) 0 / 255, (float) 82 / 255)), // 19
            new KeyValuePair<float, Color>(20.0f * 1.0f / _hourCount,
                new Color((float) 43/ 255, (float) 0 / 255, (float) 71 / 255)), // 20
            new KeyValuePair<float, Color>(21.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 51 / 255)), // 21
            new KeyValuePair<float, Color>(22.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 51 / 255)), // 22
            new KeyValuePair<float, Color>(23.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 51 / 255)), // 23
        };

        private static List<KeyValuePair<float, Color>> _hourColorsPoleCloud = new List<KeyValuePair<float, Color>>()
        {
            new KeyValuePair<float, Color>(0.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 66 / 255)), // 0
            new KeyValuePair<float, Color>(1.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 66 / 255)), // 1
            new KeyValuePair<float, Color>(2.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 66 / 255)), // 2
            new KeyValuePair<float, Color>(3.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 66 / 255)), // 3
            new KeyValuePair<float, Color>(4.0f * 1.0f / _hourCount,
                new Color((float) 122 / 255, (float) 30 / 255, (float) 101 / 255)), // 4
            new KeyValuePair<float, Color>(5.0f * 1.0f / _hourCount,
                new Color((float) 154 / 255, (float) 54 / 255, (float) 105 / 255)), // 5
            new KeyValuePair<float, Color>(6.0f * 1.0f / _hourCount,
                new Color((float) 185 / 255, (float) 100 / 255, (float) 104 / 255)), // 6
            new KeyValuePair<float, Color>(7.0f * 1.0f / _hourCount,
                new Color((float) 222 / 255, (float) 94 / 255, (float) 226 / 255)), // 7
            new KeyValuePair<float, Color>(8.0f * 1.0f / _hourCount,
                new Color((float) 158 / 255, (float) 146 / 255, (float) 238 / 255)), // 8
            new KeyValuePair<float, Color>(9.0f * 1.0f / _hourCount,
                new Color((float) 210 / 255, (float) 242 / 255, (float) 250 / 255)), // 9
            new KeyValuePair<float, Color>(10.0f * 1.0f / _hourCount,
                new Color((float) 210 / 255, (float) 242 / 255, (float) 250 / 255)), // 10
            new KeyValuePair<float, Color>(11.0f * 1.0f / _hourCount,
                new Color((float) 210 / 255, (float) 242 / 255, (float) 250 / 255)), // 11
            new KeyValuePair<float, Color>(12.0f * 1.0f / _hourCount,
                new Color((float) 210 / 255, (float) 242 / 255, (float) 250 / 255)), // 12
            new KeyValuePair<float, Color>(13.0f * 1.0f / _hourCount,
                new Color((float) 210 / 255, (float) 242 / 255, (float) 250 / 255)), // 13
            new KeyValuePair<float, Color>(14.0f * 1.0f / _hourCount,
                new Color((float) 210 / 255, (float) 242 / 255, (float) 250 / 255)), // 14
            new KeyValuePair<float, Color>(15.0f * 1.0f / _hourCount,
                new Color((float) 210 / 255, (float) 242 / 255, (float) 250 / 255)), // 15
            new KeyValuePair<float, Color>(16.0f * 1.0f / _hourCount,
                new Color((float) 142 / 255, (float) 130 / 255, (float) 218 / 255)), // 16
            new KeyValuePair<float, Color>(17.0f * 1.0f / _hourCount,
                new Color((float) 178 / 255, (float) 70 / 255, (float) 182 / 255)), // 17
            new KeyValuePair<float, Color>(18.0f * 1.0f / _hourCount,
                new Color((float) 150 / 255, (float) 22 / 255, (float) 50 / 255)), // 18
            new KeyValuePair<float, Color>(19.0f * 1.0f / _hourCount,
                new Color((float) 122 / 255, (float) 6 / 255, (float) 74 / 255)), // 19
            new KeyValuePair<float, Color>(20.0f * 1.0f / _hourCount,
                new Color((float) 66 / 255, (float) 0 / 255, (float) 94 / 255)), // 20
            new KeyValuePair<float, Color>(21.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 66 / 255)), // 21
            new KeyValuePair<float, Color>(22.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 66 / 255)), // 22
            new KeyValuePair<float, Color>(23.0f * 1.0f / _hourCount,
                new Color((float) 0 / 255, (float) 0 / 255, (float) 66 / 255)), // 23
        };

        public static void Evaluate(float time, out Color color1, out Color color2, SkyLayer.SkyLayerType layerType)
        {
            color1 = Evaluate(time, _hourColorsHorizon);
            color2 = Evaluate(time, layerType == SkyLayer.SkyLayerType.CloudLayer ? _hourColorsPoleSky : _hourColorsPoleCloud);
        }

        private static Color Evaluate(float time, List<KeyValuePair<float, Color>> hourColors)
        {
            time = Mathf.Clamp01(time);

            if (time >= 1.0f)
            {
                return hourColors.Last().Value;
            }

            float adjustedValue = time * _hourCount;

            int floorValue = (int)adjustedValue;
            int nextValue = floorValue + 1;

            if (nextValue == 24)
            {
                nextValue = 0;
            }

            float interpolation = adjustedValue % 1.0f;

            Color color1 = hourColors[floorValue].Value;
            Color color2 = hourColors[nextValue].Value;

            return Color.Lerp(color1, color2, interpolation);
        }
    }
}
