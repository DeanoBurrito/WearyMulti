using System;
using Weary.Rendering;

namespace Weary.Resources
{
    internal class DefaultLoaders
    {
        public static void RegisterDefaultLoaders()
        {
            ResourceManager.RegisterResourceLoader
            (
                new string[] { ".txt", ".cfg", ".json" }, 
                typeof(TextResource),
                (ResourceManager rman) => { return new TextResource(rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[] { ".ttf", ".otf" },
                typeof(FontResource),
                (ResourceManager rman) => { return new FontResource(rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[] { ".png", ".jpg" },
                typeof(TextureResource),
                (ResourceManager rman) => { return new TextureResource(RenderServer.Global, rman); }
            );
        }
    }
}