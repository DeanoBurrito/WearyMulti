using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Weary.Resources
{
    public sealed class ResourceManager
    {
        public static ResourceManager Global
        { get; private set; }

        internal static void Init()
        {
            ResTerminalCommands.RegisterCommands();
            Global = new ResourceManager();
            Global.LoadManifestText("_Data/Manifest.json");
        }

        internal UuidManager ridGenerator;

        Dictionary<ulong, ResourceBase> resources;
        Dictionary<string, ResourceHeader> headers;

        public ResourceManager()
        {
            ridGenerator = new UuidManager();
            resources = new Dictionary<ulong, ResourceBase>();
            headers = new Dictionary<string, ResourceHeader>();
        }

        public ResourceBase GetResource(ulong rid)
        {
            throw new NotImplementedException();
        }

        public List<ResourceHeader> GetHeaders()
        {
            //return a copy so the headers cant be meddled with
            ResourceHeader[] hArr = new ResourceHeader[headers.Count];
            headers.Values.CopyTo(hArr, 0);
            return new List<ResourceHeader>(hArr);
        }

        public void LoadManifestText(string filename)
        {
            if (!File.Exists(filename))
            {
                Log.WriteError("Cannot load manifest, file does not exist: " + filename);
                return;
            }

            try
            {
                string manifestText = File.ReadAllText(filename);
                ProcessManifest(manifestText);
            }
            catch (Exception e)
            {
                Log.WriteError("Error during manifest file read: " + e.ToString());
            }
        }

        public void LoadManifestBinary(string filename, int readOffset = 0)
        {
            if (!File.Exists(filename))
            {
                Log.WriteError("Cannot load manifest, file does not exist: " + filename);
                return;
            }

            try
            {
                string manifestText;
                using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    if (readOffset > 0)
                        reader.BaseStream.Seek(readOffset, SeekOrigin.Begin);
                    manifestText = reader.ReadString();
                }
                ProcessManifest(manifestText);
            }
            catch (Exception e)
            {
                Log.WriteError("Error during manifest file read: " + e.ToString());
            }
        }

        private void ProcessManifest(string data)
        {
            JsonDocument jdoc = JsonDocument.Parse(data);
            JsonElement jroot = jdoc.RootElement;

            //TODO: this is hacky, and should use TryGetProperty() instead of GetProperty. We dont know whats in the file we just opened.
            string packageName = jroot.GetProperty("Name").GetString() ?? "No package name";
            string packageDesc = jroot.GetProperty("Description").GetString() ?? "No description";
            string packageAuthor = jroot.GetProperty("Author").GetString() ?? "No author";
            Version packageVersion = Version.Parse(jroot.GetProperty("Version").GetString()) ?? new Version(69, 420);
            Log.WriteLine($"Loading manifest: {packageName} by {packageAuthor} (v {packageVersion.ToString(3)}), {packageDesc}");

            JsonElement resElement = jroot.GetProperty("Resources");
            foreach (JsonElement ele in resElement.EnumerateArray())
            {
                ProcessManifestEntry(ele);
            }

            Log.WriteLine("Manifest " + packageName + " loaded. " + headers.Count + " headers processed.");
        }

        private void ProcessManifestEntry(JsonElement entry)
        {
            string name = "<name missing>";
            string filename;
            ulong fileStart;
            ulong fileLen;
            bool canUnload;
            Dictionary<string, string> attribs = null;

            //Weirdly written, but essentially each if statement is getting the json property, then trying to read it (checking for non-empty strings).
            //If either of those operations fail, it'll jump to the detail missing block, otherwise process as normal.
            if (!(entry.TryGetProperty("Name", out JsonElement nameProp) && (name = nameProp.GetString()).Length > 0))
                goto CRITICAL_DETAIL_MISSING;
            if (!(entry.TryGetProperty("Filename", out JsonElement filenameProp) && (filename = filenameProp.GetString()).Length > 0))
                goto CRITICAL_DETAIL_MISSING;
            if (!(entry.TryGetProperty("FileStart", out JsonElement fileStartProp) && fileStartProp.TryGetUInt64(out fileStart)))
                goto CRITICAL_DETAIL_MISSING;
            if (!(entry.TryGetProperty("FileLength", out JsonElement fileLenProp) && fileLenProp.TryGetUInt64(out fileLen)))
                goto CRITICAL_DETAIL_MISSING;
            if (!entry.TryGetProperty("CanUnload", out JsonElement canUnloadProp))
                goto CRITICAL_DETAIL_MISSING;
            else
                canUnload = canUnloadProp.GetBoolean();

            //custom attribs is completely optional
            if (entry.TryGetProperty("CustomAttribs", out JsonElement customAttribsProp))
            {
                attribs = new Dictionary<string, string>();
                foreach (JsonElement attrib in customAttribsProp.EnumerateArray())
                {
                    if (attrib.TryGetProperty("Key", out JsonElement keyProp) && attrib.TryGetProperty("Value", out JsonElement valueProp))
                    {
                        attribs.Add(keyProp.GetString(), valueProp.GetString());
                    }
                }
            }

            ResourceHeader header = new ResourceHeader(name, filename, fileStart, fileLen, canUnload, attribs);
            headers.Add(header.resourceName, header);
            return;

        CRITICAL_DETAIL_MISSING:
            Log.WriteError($"Missing critical element for manifest entry: " + name + ". Entry is being dropped (but will remain in file).");
            return;
        }

        public void SaveManifestText(string outputName)
        { throw new NotImplementedException(); }

        public void SaveManifestBinary(string outputName)
        { throw new NotImplementedException(); }
    }
}