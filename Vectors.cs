using System;

namespace Weary
{
    public sealed class Vector2i
    {
        public static readonly Vector2i Zero = new Vector2i(0, 0);
        public static readonly Vector2i One = new Vector2i(1, 1);
        public static readonly Vector2i X = new Vector2i(1, 0);
        public static readonly Vector2i Y = new Vector2i(0, 1);

        public int x;
        public int y;

        public Vector2i(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int Length()
        { throw new NotImplementedException(); }

        public Vector2i Normalized()
        { throw new NotImplementedException(); }

        public int Distance(Vector2i v)
        { throw new NotImplementedException(); }

        public static Vector2i operator +(Vector2i v1, Vector2i v2)
        {
            return new Vector2i(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2i operator -(Vector2i v1, Vector2i v2)
        {
            return new Vector2i(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2i operator *(Vector2i v1, int scalar)
        {
            return new Vector2i(v1.x * scalar, v1.y * scalar);
        }

        public static Vector2i operator *(Vector2i v1, Vector2i v2)
        {
            return new Vector2i(v1.x * v2.x, v1.y * v2.y);
        }
    }

    public sealed class Vector2f
    {
        public static readonly Vector2f Zero = new Vector2f(0f, 0f);
        public static readonly Vector2f One = new Vector2f(1f, 1f);
        public static readonly Vector2f X = new Vector2f(1f, 0f);
        public static readonly Vector2f Y = new Vector2f(0f, 1f);

        public float x;
        public float y;

        public Vector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public int Length()
        { throw new NotImplementedException(); }

        public Vector2f Normalized()
        { throw new NotImplementedException(); }

        public int Distance(Vector2f v)
        { throw new NotImplementedException(); }

        public static Vector2f operator +(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2f operator -(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.x - v2.x, v1.y + v2.y);
        }

        public static Vector2f operator *(Vector2f v1, float scalar)
        {
            return new Vector2f(v1.x * scalar, v1.y * scalar);
        }

        public static Vector2f operator *(Vector2f v1, Vector2f v2)
        {
            return new Vector2f(v1.x * v2.x, v1.y * v2.y);
        }
    }
}