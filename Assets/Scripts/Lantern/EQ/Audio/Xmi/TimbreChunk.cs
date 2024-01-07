using System.Collections.Generic;
using System.IO;

namespace Lantern.EQ.Audio.Xmi
{
    public class TimbreChunk : Chunk
    {
        //--Fields
        private Timbre[] timbres;

        //--Properties
        public IList<Timbre> TimbreList
        {
            get { return timbres; }
        }

        //--Methods
        public TimbreChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            int timbreCount = reader.ReadUInt16();
            timbres = new Timbre[timbreCount];
            for (int x = 0; x < timbres.Length; x++)
            {
                timbres[x] = new Timbre(reader);
            }
        }

        //--Internal classes and structs
        public class Timbre
        {
            //--Fields
            private int timbrePatchNumber;
            private int timbreBank;

            //--Properties
            public int PatchNumber
            {
                get { return timbrePatchNumber; }
            }
            public int Bank
            {
                get { return timbreBank; }
            }

            //--Methods
            public Timbre(BinaryReader reader)
            {
                timbrePatchNumber = reader.ReadByte();
                timbreBank = reader.ReadByte();
            }
        }
    }
}
