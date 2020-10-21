using System;

namespace Weary.Resources
{
    public abstract class ShapeBase : ResourceBase
    {
        public readonly ShapeType shapeType;
        
        internal protected ShapeBase(ShapeType type, ResourceManager resman) : base(resman)
        {
            shapeType = type;
        }
    }
}