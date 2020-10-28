using System;
using Weary.Rendering;

namespace Weary.Resources
{
    internal static class DefaultLoaders
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
                typeof(Font),
                (ResourceManager rman) => { return new Font(rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[] { ".png", ".jpg" },
                typeof(Texture),
                (ResourceManager rman) => { return new Texture(RenderServer.Global, rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[0],
                typeof(RenderTarget),
                (ResourceManager rman) => { return new RenderTarget(RenderServer.Global, rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[0],
                typeof(RectangleShape),
                (ResourceManager rman) => { return new RectangleShape(rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[0],
                typeof(CircleShape),
                (ResourceManager rman) => { return new CircleShape(rman); }
            );
        }
    }
}