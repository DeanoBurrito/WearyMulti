using System;

namespace Weary.Debug
{
    public sealed class DebugCommand
    {
        public readonly string identifier;
        public readonly Action<string[]> callback;

        internal DebugCommand(string cmdName, Action<string[]> action)
        {
            identifier = cmdName;
            callback = action;
        }
    }
}