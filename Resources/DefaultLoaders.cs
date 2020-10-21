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
            ResourceManager.RegisterResourceLoader
            (
                new string[0],
                typeof(RenderTargetResource),
                (ResourceManager rman) => { return new RenderTargetResource(RenderServer.Global, rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[0],
                typeof(RectShapeResource),
                (ResourceManager rman) => { return new RectShapeResource(rman); }
            );
            ResourceManager.RegisterResourceLoader
            (
                new string[0],
                typeof(CircleShapeResource),
                (ResourceManager rman) => { return new CircleShapeResource(rman); }
            );
        }
    }
}