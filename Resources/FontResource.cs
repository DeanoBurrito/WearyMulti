using System;
using System.IO;
using SFML.Graphics;

namespace Weary.Resources
{
    public sealed class FontResource : ResourceBase
    {
        public Font resource = null;

        public FontResource(ResourceManager manager) : base(manager)
        { }

        protected internal override void Load(byte[] data)
        {
            resource = new Font(data);
        }

        protected internal override void Unload()
        {
            if (resource == null)
                return;

            resource.Dispose();
            resource = null;
        }

        protected internal override byte[] Store()
        {
            if (resource == null)
                return new byte[0];

            ResourceHeader header = GetHeader();
            if (header == null || !File.Exists(header.filename))
                return new byte[0];

            byte[] data = new byte[0];
            try
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(header.filename)))
                {
                    if (header.fileStart > 0)
                        reader.BaseStream.Seek((long)header.fileStart, SeekOrigin.Begin);

                    int readLen = header.fileLength == 0 ? (int)reader.BaseStream.Length - (int)reader.BaseStream.Position : (int)header.fileLength;
                    data = reader.ReadBytes(readLen);
                }
            }
            catch (Exception e)
            {
                Log.WriteError("Unable to store FontResource " + header.resourceName + ", exception: " + e.ToString());
            }

            return data;
        }
    }
}