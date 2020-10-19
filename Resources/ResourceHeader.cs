using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Weary.Resources
{
    public sealed class ResourceHeader
    {
        public readonly string resourceName;
        public readonly string filename;
        public readonly ulong fileStart;
        public readonly ulong fileLength;
        public readonly bool canUnload;
        public readonly ImmutableDictionary<string, string> customAttribs;
        
        internal bool loaded = false;
        internal ulong loadedId = 0;

        internal ResourceHeader(string name, string file, ulong startPos, ulong length, bool allowUnload, Dictionary<string, string> customData)
        {
            resourceName = name;
            filename = file;
            fileStart = startPos;
            fileLength = length;
            canUnload = allowUnload;

            customAttribs = customData == null ? ImmutableDictionary.Create<string, string>() : customData.ToImmutableDictionary();
        }

        public override string ToString()
        {
            return resourceName + ": file=" + filename + ", start=" + fileStart + ", length=" + fileLength + ", canUnload=" + canUnload + ", custom=(" + customAttribs.Count + " entries)";
        }
    }
}