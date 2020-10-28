using System;

namespace Weary
{
    public abstract class InputServer
    {
        public static InputServer Global { get; protected set; }

        public abstract void Init();
        public abstract void Deinit();
        public abstract void HandleEvents();
        public abstract string GetServerInfo();

        public abstract InputState GetInputState();
    }
}