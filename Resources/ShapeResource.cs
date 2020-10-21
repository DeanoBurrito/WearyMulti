using System;
using System.IO;

namespace Weary.Resources
{
    public abstract class ShapeResource : ResourceBase
    {
        public readonly ShapeType shapeType;
        
        internal protected ShapeResource(ShapeType type, ResourceManager resman) : base(resman)
        {
            shapeType = type;
        }
    }
}