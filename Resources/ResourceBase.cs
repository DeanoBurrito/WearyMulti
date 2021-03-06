using System;

namespace Weary.Resources
{
    public abstract class ResourceBase
    {
        public readonly ulong rid;
        public int refCount { get; internal set; }
        protected ResourceManager resman;

        internal protected ResourceBase(ResourceManager resManager)
        {
            resman = resManager;
            rid = resman.ridGenerator.GenerateId();
            refCount = 0;
        }

        protected internal abstract void Load(byte[] data);
        protected internal abstract void Unload();
        protected internal abstract byte[] Store();
        protected internal virtual void Edit(byte[] data, int offset = 0) 
        {}

        public ResourceHeader GetHeader()
        {
            return resman.GetHeader(rid);
        }

        public ResourceRef GetRef()
        {
            return new ResourceRef(rid, resman);
        }
    }
}