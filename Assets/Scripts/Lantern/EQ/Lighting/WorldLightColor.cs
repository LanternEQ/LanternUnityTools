using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class WorldLightColor
{
    private static int _hourCount = 24;

    private static List<KeyValuePair<int, Color>> _hourColors = new List<KeyValuePair<int, Color>>()
    {
        new KeyValuePair<int, Color>(0, new Color(0.0f, 0.0f, 0.3f)),
        new KeyValuePair<int, Color>(1, new Color(0.0f, 0.0f, 0.3f)),
        new KeyValuePair<int, Color>(2, new Color(0.0f, 0.0f, 0.3f)),
        new KeyValuePair<int, Color>(3, new Color(0.0f, 0.0f, 0.3f)),
        new KeyValuePair<int, Color>(4, new Color(0.1f, 0.1f, 0.3f)),
        new KeyValuePair<int, Color>(5, new Color(0.35f, 0.2f, 0.3f)),
        new KeyValuePair<int, Color>(6, new Color(0.6f, 0.3f, 0.3f)),
        new KeyValuePair<int, Color>(7, new Color(0.6f, 0.4f, 0.4f)),
        new KeyValuePair<int, Color>(8, new Color(0.6f, 0.5f, 0.5f)),
        new KeyValuePair<int, Color>(9, new Color(0.6f, 0.6f, 0.6f)),
        new KeyValuePair<int, Color>(10, new Color(0.7f, 0.7f, 0.7f)),
        new KeyValuePair<int, Color>(11, new Color(0.8f, 0.8f, 0.8f)),
        new KeyValuePair<int, Color>(12, new Color(0.9f, 0.9f, 0.9f)),
        new KeyValuePair<int, Color>(13, new Color(0.9f, 0.9f, 0.9f)),
        new KeyValuePair<int, Color>(14, new Color(0.8f, 0.8f, 0.8f)),
        new KeyValuePair<int, Color>(15, new Color(0.7f, 0.7f, 0.7f)),
        new KeyValuePair<int, Color>(16, new Color(0.6f, 0.6f, 0.6f)),
        new KeyValuePair<int, Color>(17, new Color(0.6f, 0.5f, 0.5f)),
        new KeyValuePair<int, Color>(18, new Color(0.6f, 0.4f, 0.4f)),
        new KeyValuePair<int, Color>(19, new Color(0.6f, 0.3f, 0.3f)),
        new KeyValuePair<int, Color>(20, new Color(0.35f, 0.2f, 0.3f)),
        new KeyValuePair<int, Color>(21, new Color(0.1f, 0.1f, 0.3f)),
        new KeyValuePair<int, Color>(22, new Color(0.0f, 0.0f, 0.3f)),
        new KeyValuePair<int, Color>(23, new Color(0.0f, 0.0f, 0.3f)),
    };

    public static Color Evaluate(float time)
    {
        time = Mathf.Clamp01(time);
        
        if (time >= 1.0f)
        {
            return _hourColors.Last().Value;
        }

        float adjustedValue = time * _hourCount;
        
        int floorValue = (int)adjustedValue % _hourCount;
        int nextValue = (floorValue + 1) % _hourCount;
        float interpolation = adjustedValue % 1.0f;

        Color color1 = _hourColors[floorValue].Value;
        Color color2 = _hourColors[nextValue].Value;

        return Color.Lerp(color1, color2, interpolation);
    }
}