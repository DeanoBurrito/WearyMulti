using System;
using Weary.Resources;

namespace Weary.Rendering
{
    public sealed class Font : ResourceBase
    {
        internal Font(ResourceManager manager) : base(manager)
        { }

        protected internal override void Load(byte[] data)
        {
            RenderServer.Global.InitFont(this, data);
        }

        protected internal override void Unload()
        {
            RenderServer.Global.DestroyFont(this);
        }

        protected internal override byte[] Store()
        {
            return new byte[0];
        }
    }
}