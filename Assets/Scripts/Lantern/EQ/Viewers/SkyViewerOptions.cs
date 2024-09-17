using UnityEngine;

namespace Lantern.EQ.Viewers
{
    public class SkyViewerOptions : ScriptableObject
    {
        public int StartingSkyIndex = 1;
        public float StartingTime = 0.5f;
        public bool TickTime = true;
        public float SecondsPerDay = 60f;
        public bool ShowGroundPlane;
    }
}
