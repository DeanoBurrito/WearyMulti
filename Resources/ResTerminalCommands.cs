using System;
using System.Collections.Generic;
using Weary.Debug;

namespace Weary.Resources
{
    internal class ResTerminalCommands
    {
        internal static void RegisterCommands()
        {
            DebugTerminal.RegisterCommand("lsmanifest", "Shows the current resource manager manifest.", LsManifest);
            DebugTerminal.RegisterCommand("load", "Forces a resource to be loaded.", Load);
            DebugTerminal.RegisterCommand("unload", "Unloads a resource. Note this is instant and WILL WORK ON CURRENTLY USED RESOURCES.", Unload);
            DebugTerminal.RegisterCommand("showloaders", "Lists all current resource parsers", ShowLoaders);
            DebugTerminal.RegisterCommand("showheader", "Shows a specific resource header (including rid if loaded).", ShowHeader);
            DebugTerminal.RegisterCommand("lsres", "Lists the currently loaded (or being loaded) resources.", LsRes);
        }

        public static void LsManifest(string[] args)
        {
            List<ResourceHeader> headers = ResourceManager.Global.GetHeaders();
            Log.WriteLine(headers.Count + " resource headers avalable.");
            foreach (ResourceHeader header in headers)
            {
                Log.WriteLine(header.ToString());
            }
        }

        public static void Load(string[] args)
        {
            if (args.Length != 1)
            {
                Log.WriteError("Expected resource name.");
                return;
            }

            ResourceManager.Global.GetResource(args[0]);
        }

        public static void Unload(string[] args)
        {
            if (args.Length != 1)
            {
                Log.WriteError("Expected id or name of resource");
                return;
            }
            ulong id;
            if (ulong.TryParse(args[0], out id))
                ResourceManager.Global.UnloadResource(id);
            else
                ResourceManager.Global.UnloadResource(args[0]);
        }

        public static void ShowLoaders(string[] args)
        {
            foreach (KeyValuePair<string, Func<ResourceManager, ResourceBase>> pair in ResourceManager.resLoaderMaps)
            {
                Log.WriteLine("| LOADER: " + pair.Key);
            }
        }

        public static void ShowHeader(string[] args)
        {
            if (args.Length != 1)
            {
                Log.WriteError("Expected resource name.");
                return;
            }
            ResourceHeader header = ResourceManager.Global.GetHeader(args[0]);
            if (header == null)
                return; //since we got null, an error will already have been emitted (hopefully)
            Log.WriteLine(header.ToString());
        }

        public static void LsRes(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}