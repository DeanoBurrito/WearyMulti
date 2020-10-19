using System;
using System.Collections.Generic;
using Weary.Debug;

namespace Weary.Resources
{
    internal class ResTerminalCommands
    {
        internal static void RegisterCommands()
        {
            DebugTerminal.RegisterCommand("showmanifest", "Shows the current resource manager manifest.", ShowManifest);
        }

        public static void ShowManifest(string[] args)
        {
            List<ResourceHeader> headers = ResourceManager.Global.GetHeaders();
            Log.WriteLine(headers.Count + " resource headers avalable.");
            foreach (ResourceHeader header in headers)
            {
                Log.WriteLine(header.ToString());
            }
        }
    }
}