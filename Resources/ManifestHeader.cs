using System;
using System.Collections.Immutable;

namespace Weary
{
    public sealed class ManifestHeader
    {
        public readonly string name;
        public readonly string author;
        public readonly string description;
        public readonly Version version;
        public readonly ImmutableArray<string> headerNames;
        public readonly string fileLocation;

        public ManifestHeader(string name, string author, string description, Version version, string[] headers, string location)
        {
            this.name = name;
            this.author = author;
            this.description = description;
            this.version = version;
            this.headerNames = headers.ToImmutableArray();
            this.fileLocation = location;
        }
    }
}