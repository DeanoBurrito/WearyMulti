using System;

namespace Weary.Rendering
{
    public struct Color
    {   
        public static readonly Color Black = new Color(0f, 0f, 0f);
        public static readonly Color White = new Color(1f, 1f, 1f);
        public static readonly Color Red = new Color(1f, 0f, 0f);
        public static readonly Color Green = new Color(0f, 1f, 0f);
        public static readonly Color Blue = new Color(0f, 0f, 1f);
        public static readonly Color Grey = new Color(0.5f, 0.5f, 0.5f);
        public static readonly Color TransparentBlack = new Color(0f, 0f, 0f, 0f);
        public static readonly Color TransparentWhite = new Color(1f, 1f, 1f, 0f);
        
        public float r;
        public float g;
        public float b;
        public float a;

        public Color(Color copy)
        {
            r = copy.r;
            g = copy.g;
            b = copy.b;
            a = copy.a;
        }

        public Color(Color copy, float a)
        {
            r = copy.r;
            g = copy.g;
            b = copy.b;
            this.a = a;
        }

        public Color(float x)
        {
            r = g = b = a = x;
        }

        public Color(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            a = 1f;
        }

        public Color (float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
    }
}