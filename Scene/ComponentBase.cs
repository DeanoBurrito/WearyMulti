using System;

namespace Weary.Scene
{
    public abstract class ComponentBase : SceneNode
    {
        public readonly int componentType; //cast this to an enum (positive if user-defined, negative if internal enum)

        public ComponentBase(ulong id, int compType) : base(isComp: true)
        {
            componentType = compType;
            name = "__COMP__" + componentType.ToString();
        }

        public virtual string GetDebugString() => string.Empty;
    }
}