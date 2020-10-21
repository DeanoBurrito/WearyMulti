using System;
using Weary.Rendering;

namespace Weary.Resources
{
    public enum ShapeType : byte
    {
        Rect,
        Circle,
    }
    
    public class RectShapeResource : ShapeResource
    {
        public float width;
        public float height;
        
        internal protected RectShapeResource(ResourceManager resman) : base(ShapeType.Rect, resman)
        {}

        protected internal override void Load(byte[] data)
        {
            
        }

        protected internal override void Unload()
        {
            
        }

        protected internal override byte[] Store()
        {
            throw new NotImplementedException();
        }
    }

    public class CircleShapeResource : ShapeResource
    {
        public float radius;

        internal protected CircleShapeResource(ResourceManager resman) : base(ShapeType.Circle, resman)
        {}

        protected internal override void Load(byte[] data)
        {
            
        }

        protected internal override void Unload()
        {
            
        }

        protected internal override byte[] Store()
        {
            throw new NotImplementedException();
        }
    }
}