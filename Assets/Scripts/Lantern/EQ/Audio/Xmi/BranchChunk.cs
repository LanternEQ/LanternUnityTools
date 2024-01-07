using System.Collections.Generic;
using System.IO;

namespace Lantern.EQ.Audio.Xmi
{
    public class BranchChunk : Chunk
    {
        //--Fields
        private Branch[] branches;

        //--Properties
        public IList<Branch> BranchPoints
        {
            get { return branches; }
        }

        //--Methods
        public BranchChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            int branchCount = reader.ReadUInt16();
            branches = new Branch[branchCount];
            for (int x = 0; x < branches.Length; x++)
            {
                branches[x] = new Branch(reader);
            }
        }

        //--Internal classes and structs
        public class Branch
        {
            //--Fields
            private int branchControllerIndex;
            private int branchControllerOffset;

            //--Properties
            public int ControllerIndex
            {
                get { return branchControllerIndex; }
            }
            public int ControllerOffset
            {
                get { return branchControllerOffset; }
            }

            //--Methods
            public Branch(BinaryReader reader)
            {
                branchControllerIndex = reader.ReadUInt16();
                branchControllerOffset = reader.ReadInt32();
            }
        }
    }
}
