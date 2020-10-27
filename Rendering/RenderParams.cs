using System;
using Weary.Resources;

namespace Weary.Rendering
{
    public class RenderParams
    {
        public Color tintColor = Color.White;
        public Vector2f position = Vector2f.Zero;
        public bool enabled = true;
        public Vector2i renderRectOffset = Vector2i.Zero;
        public RectangleShape renderRect = null;
    }
}