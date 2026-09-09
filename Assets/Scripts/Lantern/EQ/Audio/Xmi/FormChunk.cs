using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lantern.EQ.Audio.Xmi
{
    public class FormChunk : Chunk
    {
        //--Fields
        private string formTypeId;
        private Chunk[] formSubChunks;
        private Dictionary<long, int> branchLocations;
        //--Properties
        public string TypeId
        {
            get { return formTypeId; }
        }
        public Chunk[] SubChunks
        {
            get { return formSubChunks; }
        }
        public Dictionary<long, int> BranchLocations
        {
            get { return branchLocations; }
        }
        //--Methods
        public FormChunk(string id, int size, BinaryReader reader, Func<BinaryReader, Chunk> formCallback)
            : base(id, size)
        {
            long readTo = reader.BaseStream.Position + size;
            formTypeId = new string(XmiHelper.Read8BitChars(reader, 4));
            List<Chunk> chunkList = new List<Chunk>();
            while (reader.BaseStream.Position < readTo)
            {
                Chunk chk = formCallback.Invoke(reader);
                chunkList.Add(chk);
            }
            formSubChunks = chunkList.ToArray();
            branchLocations = GetBranchLocations();
        }

        // map byte offset => branch index
        private Dictionary<long, int> GetBranchLocations()
        {
            var branchLocations = new Dictionary<long, int>();
            var branchChunk = SubChunks.OfType<BranchChunk>().FirstOrDefault();
            if (branchChunk != null)
            {
                foreach (var branch in branchChunk.BranchPoints)
                {
                    branchLocations.Add(branch.ControllerOffset, branch.ControllerIndex);
                }
            }

            return branchLocations;
        }
    }
}
