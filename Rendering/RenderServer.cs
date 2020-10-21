using System;
using Weary.Resources;

namespace Weary.Rendering
{
    public abstract class RenderServer
    {
        public static RenderServer Global { get; protected set; }
        
        public abstract void Init();
        public abstract void Deinit();
        public abstract void HandleEvents();

        public abstract void DrawShape(RenderTargetResource target, ShapeResource shape, RenderParams renderParams);
        public abstract void DrawText(RenderTargetResource target, FontResource font, string text, uint fontSize, RenderParams renderParams);
        public abstract Vector2f GetTextBounds(FontResource font, string text, uint fontSize);
        
        public abstract void InitTexture(TextureResource texture, uint w, uint h);
        public abstract void DestroyTexture(TextureResource texture);
        
        public abstract void InitRenderTarget(RenderTargetResource target, uint w, uint h);
        public abstract void BindRenderTarget(RenderTargetResource target, Window window);
        public abstract void DestroyRenderTarget(RenderTargetResource target);
        public abstract void ClearRenderTarget(RenderTargetResource target, Color clearColor);
        public abstract void DisplayRenderTarget(RenderTargetResource target);
    }
}