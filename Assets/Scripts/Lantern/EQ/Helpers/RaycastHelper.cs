using System.Collections.Generic;
using Lantern.Data;
using UnityEngine;

namespace Lantern.EQ
{
    public static class RaycastHelper
    {
        private static readonly RaycastHit[] Hits = new RaycastHit[64];
        private static readonly float MaxDistance = 1000000f;

        public static bool TryGetSunlightValueEditor(Vector3 centerPosition, float radius,
            ZoneMeshSunlightValues sunlightValues, out float sunlightValue)
        {
            sunlightValue = 0f;

            if (sunlightValues == null)
            {
                return false;
            }

            List<Ray> rays = new List<Ray>();
            List<float> distances = new List<float>();

            // Ray from top down
            var topPosition = centerPosition;
            topPosition.y += radius;
            rays.Add(new Ray(topPosition, Vector3.down));
            distances.Add(radius * 2f);

            // North top
            var northTop = centerPosition;
            northTop.y += radius;
            northTop.x += radius;
            rays.Add(new Ray(northTop, Vector3.down));
            distances.Add(radius * 2f);

            // South top
            var southTop = centerPosition;
            southTop.y += radius;
            southTop.x -= radius;
            rays.Add(new Ray(southTop, Vector3.down));
            distances.Add(radius * 2f);

            // East top
            var eastTop = centerPosition;
            eastTop.y += radius;
            eastTop.z += radius;
            rays.Add(new Ray(eastTop, Vector3.down));
            distances.Add(radius * 2f);

            // West top
            var westTop = centerPosition;
            westTop.y += radius;
            westTop.z -= radius;
            rays.Add(new Ray(westTop, Vector3.down));
            distances.Add(radius * 2f);

            // Corner1 top (++)
            var corner1Top = centerPosition;
            corner1Top.y += radius;
            corner1Top.x += radius;
            corner1Top.z += radius;
            rays.Add(new Ray(corner1Top, Vector3.down));
            distances.Add(radius * 2f);

            // Corner2 top (-+)
            var corner2Top = centerPosition;
            corner2Top.y += radius;
            corner2Top.x -= radius;
            corner2Top.z += radius;
            rays.Add(new Ray(corner2Top, Vector3.down));
            distances.Add(radius * 2f);

            // Corner3 top (+-)
            var corner3Top = centerPosition;
            corner3Top.y += radius;
            corner3Top.x += radius;
            corner3Top.z -= radius;
            rays.Add(new Ray(corner3Top, Vector3.down));
            distances.Add(radius * 2f);

            // Corner4 top (+-)
            var corner4Top = centerPosition;
            corner4Top.y += radius;
            corner4Top.x -= radius;
            corner4Top.z -= radius;
            rays.Add(new Ray(corner4Top, Vector3.down));
            distances.Add(radius * 2f);

            // Ray from center down
            rays.Add(new Ray(centerPosition, Vector3.down));
            distances.Add(radius);

            // Ray from north side
            var northPos = centerPosition;
            northPos.x += radius;
            rays.Add(new Ray(northPos, centerPosition - northPos));
            distances.Add(radius * 2f);

            // Ray from south side
            var southPos = centerPosition;
            southPos.x -= radius;
            rays.Add(new Ray(southPos, centerPosition - southPos));
            distances.Add(radius * 2f);

            // Ray from east side
            var eastPos = centerPosition;
            eastPos.z += radius;
            rays.Add(new Ray(eastPos, centerPosition - eastPos));
            distances.Add(radius * 2f);

            // Ray from west side
            var westPos = centerPosition;
            westPos.z += radius;
            rays.Add(new Ray(westPos, centerPosition - westPos));
            distances.Add(radius * 2f);

            // Last chance
            var lastChance = centerPosition;
            lastChance.y += radius;
            rays.Add(new Ray(lastChance, Vector3.down));
            distances.Add(100000f);

            float baseMultiplier = 2f;
            for (int i = 0; i < 3; ++i)
            {
                rays.Add(rays[1]);
                rays.Add(rays[2]);
                rays.Add(rays[3]);
                rays.Add(rays[4]);
                rays.Add(rays[5]);
                distances.Add(radius * 2f * baseMultiplier);
                distances.Add(radius * 2f * baseMultiplier);
                distances.Add(radius * 2f * baseMultiplier);
                distances.Add(radius * 2f * baseMultiplier);
                distances.Add(radius * 2f * baseMultiplier);
                baseMultiplier += 1f;
            }

            while (rays.Count != 0)
            {
                var ray = rays[0];
                var distance = distances[0];

                if (Physics.RaycastNonAlloc(ray, Hits, distance, 1 << LanternLayers.Zone) > 0)
                {
                    int index = Hits[0].triangleIndex * 3;
                    int vertex1 = sunlightValues.GetVertex(index);
                    int vertex2 = sunlightValues.GetVertex(index + 1);
                    int vertex3 = sunlightValues.GetVertex(index + 2);

                    float color1 = sunlightValues.GetIntensityValue(vertex1);
                    float color2 = sunlightValues.GetIntensityValue(vertex2);
                    float color3 = sunlightValues.GetIntensityValue(vertex3);

                    float mixedColor = (color1 + color2 + color3) / 3f;

                    sunlightValue = mixedColor;
                    return true;
                }

                rays.RemoveAt(0);
                distances.RemoveAt(0);
            }

            return false;
        }

        public static bool TryGetSunlightValueRuntime(Vector3 origin, ZoneMeshSunlightValues sunlightValues,
            out float sunlightValue)
        {
            if (TryRaycastSingleNonAlloc(origin, Vector3.down, 1000f,
                1 << LanternLayers.ZoneRaycast, out var hit) && hit.HasValue)
            {
                int index = Hits[0].triangleIndex * 3;
                int vertex1 = sunlightValues.GetVertex(index);
                int vertex2 = sunlightValues.GetVertex(index + 1);
                int vertex3 = sunlightValues.GetVertex(index + 2);

                float color1 = sunlightValues.GetIntensityValue(vertex1);
                float color2 = sunlightValues.GetIntensityValue(vertex2);
                float color3 = sunlightValues.GetIntensityValue(vertex3);

                float mixedColor = (color1 + color2 + color3) / 3f;

                sunlightValue = mixedColor;
                return true;
            }
            
            sunlightValue = 0f;
            return false;
        }
        
        public static bool TryGetGroundHeight(Vector3 origin, out float height, bool includeNonSolidSurfaces = false)
        {
            if (TryRaycastSingleNonAlloc(origin, Vector3.down, 1000f,
                1 << LanternLayers.ObjectsStaticLit | 1 << LanternLayers.ObjectsDynamicLit |
                1 << (includeNonSolidSurfaces ? LanternLayers.ZoneRaycast : LanternLayers.Zone), out var hit) && hit.HasValue)
            {
                height = hit.Value.point.y;
                return true;
            }

            height = 0f;
            return false;
        }
        
        public static bool TryGetGroundHeightDebug(Vector3 position, out float height)
        {
            // The number added to the y-pos cannot be too large or NPCs will path onto walls when walking thru gates.
            // If the value is too low, they will not follow elevated terrain (like in Paineel newbie area) very good either.
            RaycastHit hit;
            // TODO: Most likely needs to use the capsule height of the NPC to determine the proper starting height of the cast.
            if (Physics.Raycast(position, Vector3.down, out hit, 1000f, 
                1 << LanternLayers.ObjectsStaticLit | 1 << LanternLayers.ObjectsDynamicLit | 1 << LanternLayers.ZoneRaycast))
            {
                Debug.LogError("Raycast hit: " + hit.collider.name + " at: " + hit.point.y);
                height = hit.point.y;
                return true;
            }

            height = 0f;
            return false;
        }

        /// <summary>
        /// Wrapper for the non allocating raycast function that returns the closest hit
        /// Silly that Unity doesn't provide this
        /// </summary>
        private static bool TryRaycastSingleNonAlloc(Vector3 origin, Vector3 direction, float distance, int mask, out RaycastHit? hit)
        {
            int hits = Physics.RaycastNonAlloc(origin, direction, Hits, distance, mask);
            hit = null;
            
            if (hits == 0)
            {
                return false;
            }

            float closest = float.PositiveInfinity;
            for (int i = 0; i < hits; ++i)
            {
                if (Hits[i].distance < closest)
                {
                    closest = Hits[i].distance;
                    hit = Hits[i];
                }
            }

            return true;
        }
    }
}
