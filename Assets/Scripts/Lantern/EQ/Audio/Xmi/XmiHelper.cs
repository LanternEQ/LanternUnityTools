using System;
using System.IO;

namespace Lantern.EQ.Audio.Xmi
{
    public static class XmiHelper
    {
        public static char[] Read8BitChars(BinaryReader reader, int length)
        {
            char[] chars = new char[length];
            for (int x = 0; x < chars.Length; x++)
                chars[x] = (char)reader.ReadByte();
            return chars;
        }

        public static int ReadInt32(byte[] input, int index)
        {
            if (BitConverter.IsLittleEndian)
                return input[index] | (input[index + 1] << 8) | (input[index + 2] << 16) | (input[index + 3] << 24);
            return (input[index] << 24) | (input[index + 1] << 16) | (input[index + 2] << 8) | input[index + 3];
        }

        public static int ReadInt32BE(BinaryReader reader)
        {
            var bytes = reader.ReadBytes(4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return ReadInt32(bytes, 0);
        }
    }
}
