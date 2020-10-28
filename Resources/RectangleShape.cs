using System;

namespace Weary.Resources
{
    public class RectangleShape : ShapeBase
    {
        public float width;
        public float height;
        
        internal protected RectangleShape(ResourceManager resman) : base(ShapeType.Rect, resman)
        {}

        protected internal override void Load(byte[] data)
        {
            if (data.Length == sizeof(float) * 2)
            {
                width = BitConverter.ToSingle(data, 0);
                height = BitConverter.ToSingle(data, 4);
            }
        }

        protected internal override void Unload()
        {}

        protected internal override byte[] Store()
        {
            int sizeOfFloat = sizeof(float);
            byte[] arr = new byte[sizeof(float) * 2];
            Array.Copy(BitConverter.GetBytes(width), arr, sizeOfFloat);
            Array.Copy(BitConverter.GetBytes(height), 0, arr, sizeOfFloat, sizeOfFloat);
            return arr;
        }
    }
}