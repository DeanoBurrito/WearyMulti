using System;
using System.IO;
using Weary.Resources;

namespace Weary.Rendering
{
    public class Texture : ResourceBase
    {
        protected RenderServer renderServer;
        public uint width { get; internal set; }
        public uint height { get; internal set; }
        
        internal protected Texture(RenderServer server, ResourceManager resman) : base(resman)
        {
            renderServer = server;
        }

        protected internal override void Load(byte[] data)
        {
            uint nWidth;
            uint nHeight;
            if (data.Length == 0)
            {
                nWidth = nHeight = 1;
            }
            else
            {
                using (MemoryStream mem = new MemoryStream(data))
                using (BinaryReader reader = new BinaryReader(mem))
                {
                    nWidth = reader.ReadUInt32();
                    nHeight = reader.ReadUInt32();
                }
            }
            renderServer.InitTexture(this, nWidth, nHeight);
        }

        protected internal override void Unload()
        {
            renderServer.DestroyTexture(this);
        }

        protected internal override byte[] Store()
        {
            throw new NotImplementedException();
        }
    }
}