using System.IO;

namespace Lantern.EQ.Audio.Xmi
{
    public class InfoChunk : Chunk
    {
        //--Fields
        private int xmidCount; // UWORD
        //--Properties
        public int XmidCount
        {
            get { return xmidCount; }
        }

        //--Methods
        public InfoChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            xmidCount = reader.ReadUInt16();
        }
    }
}
