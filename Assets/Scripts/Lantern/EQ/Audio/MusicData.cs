using System;

namespace Lantern.EQ.Audio
{
    [Serializable]
    public class MusicData
    {
        public int TrackIndexDay;
        public int TrackIndexNight;
        public int PlayCountDay;
        public int PlayCountNight;
        public int FadeOutMsDay;        // EQ only has a single fade for both day/night
        public int FadeOutMsNight;      // EQ only has a single fade for both day/night
    }
}
