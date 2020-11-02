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

        public abstract string[] GetRenderDevices();
        public abstract void SelectRenderDevice(string devName);
        public abstract string GetServerInfo();

        //Drawing Commands
        public abstract void DrawShape(RenderTarget target, ShapeBase shape, RenderParams renderParams);
        public abstract void DrawText(RenderTarget target, Font font, string text, uint fontSize, RenderParams renderParams);
        public abstract void DrawTexture(RenderTarget target, Texture texture, RenderParams renderParams);
        
        //Textures
        public abstract void InitTexture(Texture texture, uint w, uint h);
        public abstract void InitTexture(Texture texture, byte[] data);
        public abstract void DestroyTexture(Texture texture);
        public abstract void SetTextureData(Texture texture, byte[] data);
        
        //Render targets
        public abstract void InitRenderTarget(RenderTarget target, uint w, uint h);
        public abstract void BindRenderTarget(RenderTarget target, Window window);
        public abstract void DestroyRenderTarget(RenderTarget target);
        public abstract void ClearRenderTarget(RenderTarget target, Color clearColor);
        public abstract void DisplayRenderTarget(RenderTarget target);
        public abstract Texture GetRenderTargetTexture(RenderTarget target);

        //Fonts
        public abstract void InitFont(Font font, byte[] data);
        public abstract void DestroyFont(Font font);
        public abstract Vector2f GetFontBounds(Font font, string text, uint fontSize);
    }
}