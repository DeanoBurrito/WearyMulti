using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace Weary.Resources
{
    public sealed class ResourceManager
    {
        public static ResourceManager Global
        { get; private set; }

        internal static Dictionary<string, Func<ResourceManager, ResourceBase>> resLoaderMaps = new Dictionary<string, Func<ResourceManager, ResourceBase>>();

        internal static void Init()
        {
            ResTerminalCommands.RegisterCommands();
            DefaultLoaders.RegisterDefaultLoaders();

            Global = new ResourceManager();
            Global.LoadManifestText("_Data/Manifest.json");
        }

        public static void RegisterResourceLoader(string[] fileExts, Func<ResourceManager, ResourceBase> creatorDelegate)
        {
            foreach (string ext in fileExts)
            {
                if (!ext.StartsWith('.'))
                {
                    Log.WriteError("Cannot attach resource with invalid file extension (not bigging with dot): " + ext);
                    continue;
                }
                if (resLoaderMaps.ContainsKey(ext))
                {
                    Log.WriteError("Loader already present for resoruces with extention " + ext + ", skipping");
                    continue;
                }

                resLoaderMaps.Add(ext, creatorDelegate);
            }
        }

        internal UuidManager ridGenerator;

        Dictionary<ulong, ResourceBase> resources;
        Dictionary<string, ResourceHeader> headers;
        Dictionary<ulong, string> ridToHeaderMap;

        public ResourceManager()
        {
            ridGenerator = new UuidManager();
            resources = new Dictionary<ulong, ResourceBase>();
            headers = new Dictionary<string, ResourceHeader>();
            ridToHeaderMap = new Dictionary<ulong, string>();
        }

        public ResourceBase GetResource(ulong rid)
        {
            if (resources.ContainsKey(rid))
                return resources[rid];

            Log.WriteError("Cannot get resource with id=" + rid + ", rid is invalid.");
            return null;
        }

        public ResourceBase GetResource(string resourceName)
        {
            if (!headers.ContainsKey(resourceName))
            {
                Log.WriteError("Cannot get resource with name=" + resourceName + ", does not exist.");
                return null;
            }

            ResourceHeader header = headers[resourceName];
            if (header.loaded)
                return GetResource(header.loadedId);

            LoadResource(resourceName);
            if (header.loaded)
                return GetResource(header.loadedId);
            return null; //If we reach here, LoadResource() should have emitted an error anyway.
        }

        public void PreloadResources(string[] resourceList)
        { throw new NotImplementedException(); } //TODO: have this run on a background thread, and return a task-like object that can querry the loading status

        public void UnloadResource(string resourceName)
        {
            if (!headers.ContainsKey(resourceName))
            {
                Log.WriteError("Cannot unload resource, no resource exists with name=" + resourceName);
                return;
            }
            ResourceHeader header = headers[resourceName];
            if (!header.loaded)
            {
                Log.WriteError("Cannot unload resource, it's currently not loaded. Name=" + resourceName);
                return;
            }

            ResourceBase res = GetResource(header.loadedId);
            resources.Remove(res.rid);
            ridGenerator.FreeId(res.rid);
            res.Unload();
            res = null;

            header.loadedId = 0;
            header.loaded = false;
            Log.WriteLine("Unloaded resource: " + header.resourceName);
        }

        public void UnloadResource(ulong rid)
        {
            if (ridToHeaderMap.ContainsKey(rid))
                UnloadResource(ridToHeaderMap[rid]);
            else
                Log.WriteError("Cannot unload resource, nothing loaded with rid=" + rid);
        }

        private void LoadResource(string resourceName)
        {
            if (!headers.ContainsKey(resourceName))
                Log.WriteError("Cannot get resource with name=" + resourceName + ", does not exist.");

            ResourceHeader header = headers[resourceName];
            if (!File.Exists(header.filename))
                Log.WriteError("Cannot get resource with name=" + resourceName + ", specified file does not exist.");

            try
            {
                byte[] rawData;
                using (FileStream fstream = File.OpenRead(header.filename))
                using (BinaryReader reader = new BinaryReader(fstream))
                {
                    if (header.fileStart > 0)
                        reader.BaseStream.Seek((long)header.fileStart, SeekOrigin.Begin);

                    int readLen = header.fileLength == 0 ? (int)reader.BaseStream.Length - (int)reader.BaseStream.Position : (int)header.fileLength;
                    rawData = reader.ReadBytes(readLen);
                }

                if (!resLoaderMaps.ContainsKey(header.loaderExt))
                    throw new Exception("Loader not found for extension: " + header.loaderExt + ". Unable to load resource: " + resourceName);

                ResourceBase resource = resLoaderMaps[header.loaderExt].Invoke(this);
                resource.Load(rawData);
                resources.Add(resource.rid, resource);
                ridToHeaderMap.Add(resource.rid, header.resourceName);

                header.loaded = true;
                header.loadedId = resource.rid;
                Log.WriteLine("Loaded resource: " + resourceName);
            }
            catch (Exception e)
            {
                Log.WriteError("Error occured during resource load: " + e.ToString());
            }
        }

        public ResourceRef GetRef(string resourceName)
        {
            ResourceBase resource = GetResource(resourceName);

            if (resource != null)
                return new ResourceRef(resource.rid, this);
            else
                return null;
        }

        public ResourceHeader GetHeader(ulong rid)
        {
            if (resources.ContainsKey(rid))
                return GetHeader(ridToHeaderMap[rid]);

            Log.WriteError("Cannot get resource header, nothing loaded with rid=" + rid);
            return null;
        }

        public ResourceHeader GetHeader(string resourceName)
        {
            if (headers.ContainsKey(resourceName))
                return headers[resourceName];

            Log.WriteError("Cannot get resource header, no manifest entry for resource: " + resourceName);
            return null;
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

                string relativePath = Path.GetRelativePath(Environment.CurrentDirectory, filename);
                relativePath = Path.GetDirectoryName(relativePath);
                ProcessManifest(manifestText, relativePath);
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
                using (FileStream fstream = File.Open(filename, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fstream))
                {
                    if (readOffset > 0)
                        reader.BaseStream.Seek(readOffset, SeekOrigin.Begin);
                    manifestText = reader.ReadString();
                }

                string relativePath = Path.GetRelativePath(Environment.CurrentDirectory, filename);
                relativePath = Path.GetDirectoryName(relativePath);
                ProcessManifest(manifestText, relativePath);
            }
            catch (Exception e)
            {
                Log.WriteError("Error during manifest file read: " + e.ToString());
            }
        }

        private void ProcessManifest(string data, string relativePath)
        {
            JsonDocument jdoc = JsonDocument.Parse(data);
            JsonElement jroot = jdoc.RootElement;

            string packageName = string.Empty;
            string packageDesc = string.Empty;
            string packageAuthor = string.Empty;
            Version packageVersion = new Version(0, 0, 0);
            if (jroot.TryGetProperty("Name", out JsonElement nameProp))
                packageName = nameProp.GetString();
            if (jroot.TryGetProperty("Description", out JsonElement descProp))
                packageDesc = descProp.GetString();
            if (jroot.TryGetProperty("Author", out JsonElement authorProp))
                packageAuthor = descProp.GetString();
            if (jroot.TryGetProperty("Version", out JsonElement versionProp))
                packageVersion = new Version(versionProp.GetString());
            Log.WriteLine($"Loading manifest: {packageName} by {packageAuthor} (v {packageVersion.ToString(3)}), {packageDesc}");

            JsonElement resElement = jroot.GetProperty("Resources");
            foreach (JsonElement ele in resElement.EnumerateArray())
            {
                ProcessManifestEntry(ele, relativePath);
            }

            Log.WriteLine("Manifest " + packageName + " loaded. " + headers.Count + " headers processed.");
        }

        private void ProcessManifestEntry(JsonElement entry, string relativePath)
        {
            string name = "<name missing>";
            string filename;
            ulong fileStart;
            ulong fileLen;
            bool canUnload;
            string loaderExt;
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
            if (!(entry.TryGetProperty("LoaderExtension", out JsonElement loadExtProp) && (loaderExt = loadExtProp.GetString()).Length > 0))
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

            if (headers.ContainsKey(name))
            {
                Log.WriteError($"Cannot added manifest entry {name}, an entry already exists with this name.");
                return;
            }

            filename = Path.Combine(relativePath, filename);
            ResourceHeader header = new ResourceHeader(name, filename, fileStart, fileLen, canUnload, loaderExt, attribs);
            headers.Add(header.resourceName, header);
            return;

        CRITICAL_DETAIL_MISSING:
            Log.WriteError($"Missing critical element for manifest entry: " + name + ". Entry is being dropped (but will remain in file).");
            return;
        }

        public void SaveManifestText(string outputName, string[] resourceList, bool enableOverwrite = false)
        {
            if (File.Exists(outputName))
            {
                if (enableOverwrite)
                {
                    Log.WriteLine("Manifest file " + outputName + " already exists. Overwriting with new.");
                    File.Delete(outputName);
                }
                else
                {
                    Log.WriteError("Manifest file " + outputName + " already exists, and enableOverwrite is not enabled. Aborting write operation.");
                    return;
                }
            }

            if (resourceList == null)
                resourceList = headers.Keys.ToArray();

            try
            {
                using (FileStream fstream = File.OpenWrite(outputName))
                using (Utf8JsonWriter writer = new Utf8JsonWriter(fstream))
                {
                    WriteManifest(writer, resourceList);
                }
            }
            catch (Exception e)
            {
                Log.WriteError("Error occured during manifest write, exception: " + e.ToString());
            }
        }

        public void SaveManifestBinary(string outputName, string[] resourceList, bool enableOverwrite = false)
        {
            if (File.Exists(outputName))
            {
                if (enableOverwrite)
                {
                    Log.WriteLine("Manifest file " + outputName + " already exists. Overwriting with new.");
                    File.Delete(outputName);
                }
                else
                {
                    Log.WriteError("Manifest file " + outputName + " already exists, and enableOverwrite is not enabled. Aborting write operation.");
                    return;
                }
            }

            if (resourceList == null)
                resourceList = headers.Keys.ToArray();
            
            try
            {
                using (MemoryStream memFile = new MemoryStream())
                {
                    using (Utf8JsonWriter writer = new Utf8JsonWriter(memFile))
                    {
                        WriteManifest(writer, resourceList);
                    }

                    File.WriteAllBytes(outputName, memFile.ToArray());
                }

            }
            catch (Exception e)
            {
                Log.WriteError("Error occured during manifest write, exception: " + e.ToString());
            }
        }

        private void WriteManifest(Utf8JsonWriter writer, string[] headerNames)
        { 
            writer.WriteStartObject();

            //TODO: implement a way to get these values programmatically
            writer.WriteString("Name", "Coming soon!");
            writer.WriteString("Description", "Coming soon!");
            writer.WriteString("Author", "Coming soon!");
            writer.WriteString("Version", "0.0.0");

            writer.WriteStartArray("Resources");
            foreach (string headerStr in headerNames)
            {
                if (!headers.ContainsKey(headerStr))
                    continue;
                
                WriteManifestEntry(writer, headers[headerStr]);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }

        private void WriteManifestEntry(Utf8JsonWriter writer, ResourceHeader header)
        {
            writer.WriteStartObject();

            writer.WriteString("Name", header.resourceName);
            writer.WriteString("Filename", header.filename);
            writer.WriteNumber("FileStart", header.fileStart);
            writer.WriteNumber("FileLength", header.fileLength);
            writer.WriteBoolean("CanUnload", header.canUnload);
            writer.WriteString("LoaderExtension", header.loaderExt);

            writer.WriteStartArray("CustomAttribs");
            foreach (KeyValuePair<string, string> custom in header.customAttribs)
            {
                writer.WriteStartObject();
                writer.WriteString("Key", custom.Key);
                writer.WriteString("Value", custom.Value);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}