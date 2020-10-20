using System;

namespace Weary.Resources
{
    internal class DefaultLoaders
    {
        public static void RegisterDefaultLoaders()
        {
            ResourceManager.RegisterResourceLoader
            (
                new string[] { ".txt", ".cfg", ".json" }, 
                (ResourceManager rman) => { return new TextResource(rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[] { ".ttf", ".otf" },
                (ResourceManager rman) => { return new FontResource(rman); }
            );
        }
    }
}