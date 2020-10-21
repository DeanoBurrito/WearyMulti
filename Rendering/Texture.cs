using System;
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
            renderServer.InitTexture(this, 0, 0);
            throw new NotImplementedException();
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