using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public class ZoneViewerOptions : ScriptableObject
    {
        public bool LoadAllObjects = true;
        public bool LoadAllDoors = true;
        public float StartTime = 0.5f;
        public bool TickTime = false;
        public float SecondsPerDay = 60f;
    }
}
