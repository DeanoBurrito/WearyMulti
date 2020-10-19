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

        public ResourceBase GetResource()
        {
            return resman.GetResource(rid);
        }
    }
}