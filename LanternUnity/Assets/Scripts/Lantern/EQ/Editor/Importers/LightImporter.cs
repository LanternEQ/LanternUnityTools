using System;
using Lantern.Data;
using Lantern.EQ;
using UnityEngine;

namespace Lantern.Editor.Importers
{
    public static class LightImporter
    {
        public static void CreateLightInstances(string shortname, string lightInstanceListPath, Transform lightRoot)
        {
            ImportHelper.LoadTextAsset(lightInstanceListPath, out var listInstanceList);

            if (string.IsNullOrEmpty(listInstanceList))
            {
                Debug.LogError("Could not load lights instance asset: " + lightInstanceListPath);
                return;
            }

            var parsedLightLines = TextParser.ParseTextByDelimitedLines(listInstanceList, ',');

            for (var i = 0; i < parsedLightLines.Count; i++)
            {
                var lightInstance = parsedLightLines[i];
                GameObject lightObject = new GameObject("Light_" + i);
                lightObject.transform.position = new Vector3(Convert.ToSingle(lightInstance[0]),
                    Convert.ToSingle(lightInstance[1]), Convert.ToSingle(lightInstance[2]));
                Light light = lightObject.AddComponent<Light>();

                light.color = new Color(Convert.ToSingle(lightInstance[4]), Convert.ToSingle(lightInstance[5]),
                    Convert.ToSingle(lightInstance[6]));

                light.range = Convert.ToSingle(lightInstance[3]) * LanternConstants.WorldScale;

                light.intensity = 1f;

                if (lightInstance.Count > 7)
                {
                    light.intensity = Convert.ToSingle(lightInstance[7]);
                }

                lightObject.transform.parent = lightRoot.transform;
                light.lightmapBakeType = LightmapBakeType.Realtime;
                light.shadows = LightShadows.None;

                // Light should not affect skydome or geometry (unless during baking)
                light.cullingMask = ~((1 << LanternLayers.Zone) | (1 << LanternLayers.Skydome) |
                                      (1 << LanternLayers.ObjectsStaticLit));

                light.tag = LanternTags.StaticLight;
            }
        }
    }
}