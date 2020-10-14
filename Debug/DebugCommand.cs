using System;

namespace Weary.Debug
{
    public sealed class DebugCommand
    {
        public readonly string identifier;
        public readonly string help;
        public readonly Action<string[]> callback;

        internal DebugCommand(string cmdName, string helpText, Action<string[]> action)
        {
            identifier = cmdName;
            help = helpText;
            callback = action;
        }
    }
}