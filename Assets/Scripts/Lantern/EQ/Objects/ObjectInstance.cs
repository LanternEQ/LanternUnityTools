using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lantern.EQ.Objects
{
    /// <summary>
    /// Representation of a single object in a zone.
    /// Contains all info needed to spawn in the object.
    /// </summary>
    [Serializable]
    public class ObjectInstance
    {
        [NonSerialized]
        public int Id;
        public GameObject GameObject;
        public string Name;
        public Vector3 Position;
        public Vector3 Rotation;

        public float Scale;
        public List<Color> Colors;
        public ObjectData.ObjectBoundingInfo _objectBoundingInfo;
        [NonSerialized]
        public bool PendingSpawn;
    }
}
