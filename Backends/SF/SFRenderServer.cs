using System;
using System.Collections.Generic;
using Weary.Rendering;
using Weary.Resources;
using SFML.Graphics;

namespace Weary.Backends.SF
{
    public sealed class SFRenderServer : RenderServer
    {
        private List<(ResourceBase wryRes, object sfRes)> destructionPending = new List<(ResourceBase, object)>();

        private Dictionary<ulong, (RenderTargetResource wryTarget, RenderTarget sfTarget)> renderTargets = new Dictionary<ulong, (RenderTargetResource, RenderTarget)>();
        
        public override void Init()
        {
            Log.WriteLine("Initialising SFML-based render server.");
        }

        public override void Deinit()
        {
            Log.WriteLine("Deinitialising SFML-based render server.");
        }

        public override void HandleEvents()
        {
            for (int i = 0; i < destructionPending.Count; i++)
            {
                if (destructionPending[i].wryRes is RenderTargetResource wryRenderTarget)
                {
                    RenderTarget sfRenderTarget = (RenderTarget)destructionPending[i].sfRes;
                    Log.WriteLine("SFML rendertarget destroyed: rid=" + wryRenderTarget.rid);
                    //TODO: implement destroying rendertargets
                }
            }
            destructionPending.Clear();
        }

        public override void DrawShape(RenderTargetResource target, ShapeResource shape, RenderParams renderParams)
        {
            if (!renderParams.enabled)
                return;
            
            switch (shape.shapeType)
            {
                case ShapeType.Rect:
                    RenderRectShape(target, (RectShapeResource)shape, renderParams);
                    break;
                case ShapeType.Circle:
                    RenderCircleShape(target, (CircleShapeResource)shape, renderParams);
                    break;
            }
        }

        private void RenderRectShape(RenderTargetResource target, RectShapeResource shape, RenderParams renderParams)
        {
            RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;
            
            RectangleShape sfRect = new RectangleShape(new SFML.System.Vector2f(shape.width, shape.height));
            sfRect.Position = new SFML.System.Vector2f(renderParams.position.x, renderParams.position.y);
            sfRect.FillColor = GetSfmlColor(renderParams.tintColor);

            sfTarget.Draw(sfRect);
        }

        private void RenderCircleShape(RenderTargetResource targt, CircleShapeResource shape, RenderParams renderParams)
        {}

        public override void InitTexture(TextureResource texture, uint w, uint h)
        {}

        public override void DestroyTexture(TextureResource texture)
        {}

        public override void InitRenderTarget(RenderTargetResource target, uint w, uint h)
        {
            if (renderTargets.ContainsKey(target.rid))
            {
                Log.WriteError("Invalid rendertarget for init: target is already bound. Please destroy target the target first then re-initialize.");
                return;
            }

            RenderTexture sfTarget = new RenderTexture(w, h);
            renderTargets.Add(target.rid, (target, sfTarget));

            Log.WriteLine("New SFML rendertarget initialized: w=" + w + ", h=" + h + ", rid=" + target.rid);
        }

        public override void BindRenderTarget(RenderTargetResource target, Window window)
        {
            if (WindowServer.Global is SFWindowServer sfWindowServer)
            {
                RenderWindow sfRenderWindow = sfWindowServer.GetRenderWindow(window);
                renderTargets.Add(target.rid, (target, sfRenderWindow));
                
                Log.WriteLine("New SFML rendertarget bound to window (id=" + window.windowId + ", title=" + window.title + "), rid=" + target.rid);
            }
            else
                Log.WriteError("Unable to bind SFML render target to window, window server is not SFML-based (mismatching data).");
        }

        public override void DestroyRenderTarget(RenderTargetResource target)
        {
            RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;
            
            Log.WriteLine("SFML rendertarget queued for destruction. rid=" + target.rid);
            renderTargets.Remove(target.rid);
            destructionPending.Add((target, sfTarget));
        }

        public override void ClearRenderTarget(RenderTargetResource target, Weary.Rendering.Color clearColor)
        {
            RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;
            
            sfTarget.Clear(GetSfmlColor(clearColor));
        }

        public override void DisplayRenderTarget(RenderTargetResource target)
        {
            RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;

            if (sfTarget is RenderTexture sfRenderTexture)
                sfRenderTexture.Display();
            else if (sfTarget is RenderWindow sfRenderWindow)
                sfRenderWindow.Display();
            else
                Log.WriteError("SFML RenderTarget does not support display() call (if you see this, panic). rid=" + target.rid);
        }

        private RenderTarget GetValidRenderTarget(RenderTargetResource target)
        {
            if (target == null)
            {
                Log.WriteError("Invalid rendertarget for operation: target is null.");
                return null;
            }
            else if (!renderTargets.ContainsKey(target.rid))
            {
                Log.WriteError("Invalid rendertarget for operation: target is not owned by this render server.");
                return null;
            }
            return renderTargets[target.rid].sfTarget;
        }

        private SFML.Graphics.Color GetSfmlColor(Weary.Rendering.Color col)
        {
            float dr = MathF.Max(0f, MathF.Min(col.r * 255f, 255f));
            float dg = MathF.Max(0f, MathF.Min(col.g * 255f, 255f));
            float db = MathF.Max(0f, MathF.Min(col.b * 255f, 255f));
            float da = MathF.Max(0f, MathF.Min(col.a * 255f, 255f));
            return new SFML.Graphics.Color((byte)dr, (byte)dg, (byte)db, (byte)da);
        }
    }
}