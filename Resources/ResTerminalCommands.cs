using System;
using System.Collections.Generic;
using System.Linq;
using Weary.Debug;

namespace Weary.Resources
{
    internal class ResTerminalCommands
    {
        [DebugCommandLoader]
        internal static void RegisterCommands()
        {
            DebugTerminal.RegisterCommand("lsmanifest", "Shows the current resource manager manifest.", LsManifest);
            DebugTerminal.RegisterCommand("savemanifest", "Saves the current manifest to a file, and writes all used resources to a binary.", SaveManifest);
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

        public static void SaveManifest(string[] args)
        {
            List<ResourceHeader> headers = ResourceManager.Global.GetHeaders();
            List<string> headerNames = new List<string>();
            foreach (ResourceHeader h in headers)
                headerNames.Add(h.resourceName);
            
            ManifestHeader header = new ManifestHeader(
                "DebugManifest", 
                "Debug Terminal", 
                "Created via the debug terminal command 'savemanifest'", 
                new Version(1, 0, 0), 
                ResourceManager.Global.GetHeaders().Select(h => h.resourceName).ToArray(),
                "_Data/DebugManifest.json");
            ResourceManager.Global.SaveManifestText(header, true);
            Log.WriteLine("Current manifest saved to _Data/DebugManifest.json");
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
            foreach (KeyValuePair<string, Type> pair in ResourceManager.resLoaderMaps)
            {
                Log.WriteLine("| LOADER: " + pair.Key + " for type " + pair.Value.Name + " (" + pair.Value.FullName + ")");
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