using System;

namespace Weary.Resources
{
    public class CircleShape : ShapeBase
    {
        public float radius;

        internal protected CircleShape(ResourceManager resman) : base(ShapeType.Circle, resman)
        {}

        protected internal override void Load(byte[] data)
        {
            if (data.Length == sizeof(float))
            {
                radius = BitConverter.ToSingle(data, 0);
            }
        }

        protected internal override void Unload()
        {}

        protected internal override byte[] Store()
        {
            return BitConverter.GetBytes(radius);
        }
    }
}