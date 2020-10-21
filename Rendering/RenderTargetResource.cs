using System;
using System.IO;
using Weary.Resources;

namespace Weary.Rendering
{
    public sealed class RenderTargetResource : ResourceBase
    {
        public uint width { get; internal set; }
        public uint height { get; internal set; }

        private RenderServer renderServer;
        
        internal RenderTargetResource(RenderServer server, ResourceManager resman) : base(resman)
        {
            renderServer = server;
        }

        protected internal override void Load(byte[] data)
        {
            uint nWidth;
            uint nHeight;

            using (MemoryStream mem = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(mem))
            {
                nWidth = reader.ReadUInt32();
                nHeight = reader.ReadUInt32();
            }
            
            renderServer.InitRenderTarget(this, nWidth, nHeight);
        }

        protected internal override void Unload()
        {
            renderServer.DestroyRenderTarget(this);
        }

        protected internal override byte[] Store()
        {
            return new byte[0];
        }

        public void Clear(Color color)
        {
            renderServer.ClearRenderTarget(this, color);
        }

        public void Display()
        {
            renderServer.DisplayRenderTarget(this);
        }
    }
}