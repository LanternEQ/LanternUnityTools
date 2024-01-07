using System;
using System.Collections.Generic;
using System.IO;

namespace Lantern.EQ.Audio.Xmi
{
    public class CatChunk : Chunk
    {
        //--Fields
        private string catTypeId;
        private Chunk[] catSubChunks;
        //--Properties
        public string TypeId
        {
            get { return catTypeId; }
        }
        public Chunk[] SubChunks
        {
            get { return catSubChunks; }
        }
        //--Methods
        public CatChunk(string id, int size, BinaryReader reader, Func<BinaryReader, Chunk> catCallback)
            : base(id, size)
        {
            long readTo = reader.BaseStream.Position + size;
            catTypeId = new string(XmiHelper.Read8BitChars(reader, 4));
            List<Chunk> chunkList = new List<Chunk>();
            while (reader.BaseStream.Position < readTo)
            {
                Chunk chk = catCallback.Invoke(reader);
                chunkList.Add(chk);
            }
            catSubChunks = chunkList.ToArray();
        }
    }
}
