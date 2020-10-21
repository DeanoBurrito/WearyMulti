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

        public abstract void DrawShape(RenderTarget target, ShapeBase shape, RenderParams renderParams);
        public abstract void DrawText(RenderTarget target, Font font, string text, uint fontSize, RenderParams renderParams);
        public abstract Vector2f GetTextBounds(Font font, string text, uint fontSize);
        
        public abstract void InitTexture(Texture texture, uint w, uint h);
        public abstract void DestroyTexture(Texture texture);
        
        public abstract void InitRenderTarget(RenderTarget target, uint w, uint h);
        public abstract void BindRenderTarget(RenderTarget target, Window window);
        public abstract void DestroyRenderTarget(RenderTarget target);
        public abstract void ClearRenderTarget(RenderTarget target, Color clearColor);
        public abstract void DisplayRenderTarget(RenderTarget target);
    }
}