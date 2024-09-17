using System.Collections.Generic;
using UnityEngine;

namespace Lantern.EQ.Audio
{
    public class MidiTrackCollection : ScriptableObject
    {
        public List<MidiTrack> MidiTracks = new List<MidiTrack>();
    }
}
