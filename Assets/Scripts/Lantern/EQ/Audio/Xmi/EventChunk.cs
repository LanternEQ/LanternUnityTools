using System.IO;

namespace Lantern.EQ.Audio.Xmi
{
    public class EventChunk : Chunk
    {
        //--Fields
        private byte[] eventData;

        //--Properties
        public byte[] Data
        {
            get { return eventData; }
        }

        //--Methods
        public EventChunk(string id, int size, BinaryReader reader)
            : base(id, size)
        {
            eventData = reader.ReadBytes(size);
            // if (size % 2 == 1 && reader.PeekChar() == 0)
            //     reader.ReadByte();
        }
    }
}
