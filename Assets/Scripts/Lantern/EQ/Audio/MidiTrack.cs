using UnityEngine;

namespace Lantern.EQ.Audio
{
    [PreferBinarySerialization]
    public class MidiTrack : ScriptableObject
    {
        public string Name;
        public int TrackNumber;
        public byte[] Bytes;

        public static MidiTrack CreateMidiTrack(string name, int trackNumber, byte[] bytes)
        {
            var midiTrack = CreateInstance<MidiTrack>();
            midiTrack.Name = name;
            midiTrack.TrackNumber = trackNumber;
            midiTrack.Bytes = bytes;
            return midiTrack;
        }
    }
}
