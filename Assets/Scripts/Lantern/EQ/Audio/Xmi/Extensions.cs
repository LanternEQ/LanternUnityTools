using Melanchall.DryWetMidi.Core;

namespace Lantern.EQ.Audio.Xmi
{
    public static class Extensions
    {
        // Special xmidi VLQ
        public static long ReadXmiVlqLongNumber(this MidiReader reader)
        {
            long result = 0;
            byte b;
            bool validXmiVlqByte;

            do
            {
                b = reader.ReadByte();
                validXmiVlqByte = (b & 0x80) == 0;

                if (validXmiVlqByte)
                    result += b;
            }
            while (validXmiVlqByte);
            reader.Position--;

            return result;
        }

        public static int ReadXmiVlqNumber(this MidiReader reader)
        {
            return (int)reader.ReadXmiVlqLongNumber();
        }
    }
}
