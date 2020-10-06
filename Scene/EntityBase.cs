using System;
using SFML.System;

namespace Weary.Scene
{
    public abstract class EntityBase : SceneNode
    {
        public Vector2f position = new Vector2f(0f, 0f);
        public float rotation = 0f;
    }
}