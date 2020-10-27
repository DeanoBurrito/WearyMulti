using System;
using System.IO;
using Weary.Resources;

namespace Weary.Rendering
{
    public sealed class RenderTarget : ResourceBase
    {
        public uint width { get; internal set; }
        public uint height { get; internal set; }

        private RenderServer renderServer;
        internal ulong textureRid;

        internal RenderTarget(RenderServer server, ResourceManager resman) : base(resman)
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

        public ResourceRef GetTexture()
        {
            return renderServer.GetRenderTargetTexture(this).GetRef();
        }

        public void DrawShape(ShapeBase shape, RenderParams renderParams)
        {
            renderServer.DrawShape(this, shape, renderParams);
        }

        public void DrawText(Font font, string text, uint fontSize, RenderParams renderParams)
        {
            renderServer.DrawText(this, font, text, fontSize, renderParams);
        }

        public void DrawTexture(Texture texture, RenderParams renderParams)
        {
            renderServer.DrawTexture(this, texture, renderParams);
        }
    }
}