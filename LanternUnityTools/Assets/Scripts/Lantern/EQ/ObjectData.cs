using System;
using System.Collections.Generic;
using Lantern;
using Lantern.EQ;
using UnityEngine;

public class ObjectData : MonoBehaviour
{
    [Serializable]
    public class BoundingInfo
    {
        public Vector3 Center;
        public float Radius;
    }

    [SerializeField]
    private List<ObjectInstance> Objects = new List<ObjectInstance>();
    
    public void AddObjectInstance(string name, Vector3 position, Vector3 rotation, float scale, List<Color> colors, BoundingInfo bounds)
    {
        if (colors == null)
        {
            var zoneValues = FindObjectOfType<ZoneMeshSunlightValues>();
            colors = new List<Color>();
            var centeredPosition = position;
            centeredPosition.y += bounds.Center.y * scale;

            if (!RaycastHelper.TryGetSunlightValueEditor(centeredPosition, bounds.Radius * scale, zoneValues, out var sunlightA))
            {
                Debug.LogError("Unable to get light for: " + name + $" at {position}");
            }

            colors.Add(new Color(0f, 0f, 0f, sunlightA));
        }

        var adjustedBounds = new BoundingInfo
        {
            Center = Quaternion.Euler(rotation) * bounds.Center, 
            Radius = bounds.Radius * scale
        };

        Objects.Add(new ObjectInstance
        {
            Name = name,
            Position = position,
            Rotation = rotation,
            Scale = scale,
            Colors = colors,
            BoundingInfo = adjustedBounds
        });
    }

    public List<ObjectInstance> GetObjects()
    {
        return Objects;
    }
}
