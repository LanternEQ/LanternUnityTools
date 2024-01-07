﻿using System.IO;

namespace Infrastructure.EQ.MeltySynth
{
    internal sealed class PresetInfo
    {
        private string name;
        private int patchNumber;
        private int bankNumber;
        private int zoneStartIndex;
        private int zoneEndIndex;
        private int library;
        private int genre;
        private int morphology;

        private PresetInfo(BinaryReader reader)
        {
            name = reader.ReadFixedLengthString(20);
            patchNumber = reader.ReadUInt16();
            bankNumber = reader.ReadUInt16();
            zoneStartIndex = reader.ReadUInt16();
            library = reader.ReadInt32();
            genre = reader.ReadInt32();
            morphology = reader.ReadInt32();
        }

        internal static PresetInfo[] ReadFromChunk(BinaryReader reader, int size)
        {
            if (size % 38 != 0)
            {
                throw new InvalidDataException("The preset list is invalid.");
            }

            var count = size / 38;

            var presets = new PresetInfo[count];

            for (var i = 0; i < count; i++)
            {
                presets[i] = new PresetInfo(reader);
            }

            for (var i = 0; i < count - 1; i++)
            {
                presets[i].zoneEndIndex = presets[i + 1].zoneStartIndex - 1;
            }

            return presets;
        }

        public string Name => name;
        public int PatchNumber => patchNumber;
        public int BankNumber => bankNumber;
        public int ZoneStartIndex => zoneStartIndex;
        public int ZoneEndIndex => zoneEndIndex;
        public int Library => library;
        public int Genre => genre;
        public int Morphology => morphology;
    }
}
