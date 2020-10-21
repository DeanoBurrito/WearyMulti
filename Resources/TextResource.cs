using System;
using System.Text;

namespace Weary.Resources
{
    public sealed class TextResource : ResourceBase
    {
        public string resource = string.Empty;
        
        internal TextResource(ResourceManager manager) : base(manager)
        {}

        protected internal override void Load(byte[] data)
        {
            resource = Encoding.UTF8.GetString(data);
        }

        protected internal override void Unload()
        {
            resource = string.Empty;
        }

        protected internal override byte[] Store()
        {
            if (resource.Length == 0)
                return new byte[0];
            return Encoding.UTF8.GetBytes(resource);
        }
    }
}