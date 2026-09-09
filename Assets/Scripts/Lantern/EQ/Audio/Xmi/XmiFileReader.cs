using System;
using System.Collections.Generic;
using System.IO;

namespace Lantern.EQ.Audio.Xmi
{
    public sealed class XmiFileReader : IDisposable
    {
        //--Fields
        private BinaryReader reader;
        //--Properties

        //--Methods
        public XmiFileReader(string fileName)
        {
            // if (!Path.GetExtension(fileName).ToLower().Equals(".xmi") || !CrossPlatformHelper.ResourceExists(fileName))
            //     throw new InvalidDataException("Invalid xmi file : " + fileName);
            // reader = new BinaryReader(CrossPlatformHelper.OpenResource(fileName));

            var stream = File.Open(fileName, FileMode.Open);
            reader = new BinaryReader(stream);
        }
        public XmiFileReader(Stream stream)
        {
            reader = new BinaryReader(stream);
        }

        public XmiFile ReadXmiFile()
        {
            return new XmiFile(XmiFileReader.ReadAllChunks(reader));
        }
        public Chunk[] ReadAllChunks()
        {
            return XmiFileReader.ReadAllChunks(reader);
        }
        public Chunk ReadNextChunk()
        {
            return XmiFileReader.ReadNextChunk(reader);
        }
        public void Close()
        {
            Dispose();
        }
        public void Dispose()
        {
            if (reader == null)
                return;
            reader.Dispose();
            reader = null;
        }

        internal static Chunk[] ReadAllChunks(BinaryReader reader)
        {
            List<Chunk> chunks = new List<Chunk>();

            while(reader.BaseStream.Position < reader.BaseStream.Length)
            {
                Chunk chunk = ReadNextChunk(reader);
                if (chunk != null)
                    chunks.Add(chunk);
            }

            return chunks.ToArray();
        }
        internal static Chunk ReadNextChunk(BinaryReader reader)
        {
            string id = new string(XmiHelper.Read8BitChars(reader, 4));
            int size = XmiHelper.ReadInt32BE(reader);
            switch (id.ToLower())
            {
                case "form":
                    return new FormChunk(id, size, reader, new Func<BinaryReader,Chunk>(ReadNextChunk));
                case "info":
                    return new InfoChunk(id, size, reader);
                case "cat ":
                    return new CatChunk(id, size, reader, new Func<BinaryReader,Chunk>(ReadNextChunk));
                case "timb":
                    return new TimbreChunk(id, size, reader);
                case "rbrn":
                    return new BranchChunk(id, size, reader);
                case "evnt":
                    return new EventChunk(id, size, reader);

                default:
                    return new UnknownChunk(id, size, reader);
            }
        }
        internal static XmiFile ReadXmiFile(BinaryReader reader)
        {
            return new XmiFile(ReadAllChunks(reader));
        }
    }
}
