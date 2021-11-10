using UnityEngine;

namespace Lantern.Helpers
{
    public static class BoundsHelper
    {
        public static ObjectData.BoundingInfo CalculateSpherePositionAndRadiusForModel(GameObject model)
        {
            if (model == null)
            {
                return default;
            }
        
            var meshRenderers = model.GetComponentsInChildren<Renderer>();

            float maxX = 0f, maxY = 0f, maxZ = 0f;
            float minX = 0f, minY = 0f, minZ = 0f;
            float maxDist = 0f;
        
            foreach (var renderer in meshRenderers)
            {
                var min = renderer.bounds.min;
                var max = renderer.bounds.max;

                if (max.x > maxX)
                {
                    maxX = max.x;
                }
            
                if (min.x < minX)
                {
                    minX = min.x;
                }

                if (max.y > maxY)
                {
                    maxY = max.y;
                }
            
                if (min.y < minY)
                {
                    minY = min.y;
                }
            
                if (max.z > maxZ)
                {
                    maxZ = max.z;
                }
            
                if (min.z < minZ)
                {
                    minZ = min.z;
                }
            }

            var center = new Vector3((maxX + minX) / 2f, (maxY + minY) / 2f, (maxZ + minZ) / 2f);
        
            foreach (var renderer in meshRenderers)
            {
                var min = renderer.bounds.min;
                var max = renderer.bounds.max;
                float dist1 = Vector3.Distance(min, center);
                float dist2 = Vector3.Distance(max , center);

                if (dist1 > maxDist)
                {
                    maxDist = dist1;
                }

                if (dist2 > maxDist)
                {
                    maxDist = dist2;
                }
            }

            return new ObjectData.BoundingInfo {Center = center, Radius = maxDist};
        }
    }
}
