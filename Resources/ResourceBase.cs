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

        protected abstract void Load(byte[] data);
        protected abstract void Unload();
        protected abstract byte[] Store();
        protected virtual void Edit(byte[] data, int offset = 0) 
        {}
    }
}