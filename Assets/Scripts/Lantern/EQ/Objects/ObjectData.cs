using System;
using System.Collections.Generic;
using Lantern.EQ.Helpers;
using Lantern.EQ.Lighting;
using UnityEngine;

namespace Lantern.EQ.Objects
{
    /// <summary>
    /// Holds information about the objects that are spawned in the world
    /// It is used in Lantern's object streaming system where only objects
    /// that are near the player are spawned in
    /// </summary>
    public class ObjectData : MonoBehaviour
    {
        [Serializable]
        public class ObjectBoundingInfo
        {
            public Vector3 Center;
            public float Radius;
        }

        [SerializeField]
        private List<ObjectInstance> _objects = new List<ObjectInstance>();

        #if UNITY_EDITOR
        public void AddObjectInstance(string name, Vector3 position, Vector3 rotation, float scale, List<Color> colors, ObjectBoundingInfo bounds)
        {
            if (colors == null)
            {
                var zoneValues = FindObjectOfType<ZoneAmbientLightValues>();

                if (zoneValues != null)
                {
                    colors = new List<Color>();
                    var centeredPosition = position;
                    centeredPosition.y += bounds.Center.y * scale;

                    if (!RaycastHelper.TryGetSunlightValueEditor(centeredPosition, bounds.Radius * scale, zoneValues, out var sunlightA))
                    {
                        Debug.LogError("Unable to get light for: " + name + $" at {position}");
                    }

                    colors.Add(new Color(0f, 0f, 0f, sunlightA));
                }
                else
                {
                    Debug.LogError("Cannot calculate dynamic object lighting. ZoneAmbientLightValues are missing");
                }
            }

            var adjustedBounds = new ObjectBoundingInfo
            {
                Center = Quaternion.Euler(rotation) * bounds.Center,
                Radius = bounds.Radius * scale
            };

            _objects.Add(new ObjectInstance
            {
                Name = name,
                Position = position,
                Rotation = rotation,
                Scale = scale,
                Colors = colors,
                _objectBoundingInfo = adjustedBounds
            });
        }
        #endif

        public List<ObjectInstance> GetObjects()
        {
            return _objects;
        }
    }
}
