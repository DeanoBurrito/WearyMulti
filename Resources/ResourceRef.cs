using System;

namespace Weary.Resources
{
    public sealed class ResourceRef
    {
        public readonly ulong rid;
        private ResourceManager resman;
        
        internal ResourceRef(ulong id, ResourceManager manager)
        {
            rid = id;
            resman = manager;
        }
        
        public ResourceRef(ResourceRef copy)
        {
            rid = copy.rid;
            resman = copy.resman;
        }

        public ResourceBase Get()
        {
            return resman.GetResource(rid);
        }

        public T Get<T>() where T : ResourceBase
        {
            ResourceBase rbase = Get();
            if (rbase is T)
                return (T)rbase;

            Log.WriteError("Could not convert resource to type: " + typeof(T));
            return default(T);
        }
    }
}