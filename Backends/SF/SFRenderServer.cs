using System;
using System.Collections.Generic;
using Weary.Rendering;
using Weary.Resources;

namespace Weary.Backends.SF
{
    public sealed class SFRenderServer : RenderServer
    {
        private List<(ResourceBase wryRes, object sfRes)> destructionPending = new List<(ResourceBase, object)>();

        private Dictionary<ulong, (Rendering.RenderTarget wryTarget, SFML.Graphics.RenderTarget sfTarget)> renderTargets = new Dictionary<ulong, (RenderTarget, SFML.Graphics.RenderTarget)>();
        private Dictionary<ulong, (Rendering.Texture wryTexture, SFML.Graphics.Texture sfTexture)> textures = new Dictionary<ulong, (Texture wryTexture, SFML.Graphics.Texture sfTexture)>();

        public override void Init()
        {
            Log.WriteLine("Initialising SFML-based render server.");
            if (Global == null)
                Global = this;
            else
                Log.WriteError("Render server is already running and marked as global. This instance will continue, this use case is unsupported.");
        }

        public override void Deinit()
        {
            Log.WriteLine("Deinitialising SFML-based render server.");
            if (Global == this)
                Global = null;

            foreach (var pair in renderTargets)
            {
                DestroyRenderTarget(pair.Value.wryTarget);
            }
            foreach (var pair in textures)
            {
                DestroyTexture(pair.Value.wryTexture);
            }

            HandleEvents();
        }

        public override void HandleEvents()
        {
            for (int i = 0; i < destructionPending.Count; i++)
            {
                if (destructionPending[i].wryRes is Rendering.RenderTarget wryRenderTarget)
                {
                    SFML.Graphics.RenderTarget sfRenderTarget = (SFML.Graphics.RenderTarget)destructionPending[i].sfRes;
                    Log.WriteLine("SFML rendertarget destroyed: rid=" + wryRenderTarget.rid);

                    //we're leaving render windows alone here, as those will be handled by the window server.
                    if (sfRenderTarget is SFML.Graphics.RenderTexture sfTex)
                        sfTex.Dispose();
                    sfTex = null;
                }
                else if (destructionPending[i].wryRes is Rendering.Texture wryTexture)
                {
                    SFML.Graphics.Texture sfTexture = (SFML.Graphics.Texture)destructionPending[i].sfRes;
                    Log.WriteLine("SFML texture destroyed: rid=" + wryTexture.rid);

                    sfTexture.Dispose();
                    sfTexture = null;
                }
            }
            destructionPending.Clear();
        }

        public override string[] GetRenderDevices()
        {
            Log.WriteError("SFML does not support explicitely listing or swapping render devices.");
            return new string[] 
            {
                "SFML does not support explicitely listing or swapping render devices."
            };
        }

        public override void SelectRenderDevice(string devName)
        {
            Log.WriteError("SFML does not support explicitely selecting render devices.");
        }

        public override string GetServerInfo()
        {
            return "Weary (SFML based) RenderServer. " + renderTargets.Count + " rendertargets, 0 textures.";
        }

        public override void DrawShape(Rendering.RenderTarget target, ShapeBase shape, RenderParams renderParams)
        {
            if (!renderParams.enabled)
                return;

            switch (shape.shapeType)
            {
                case ShapeType.Rect:
                    RenderRectShape(target, (RectangleShape)shape, renderParams);
                    break;
                case ShapeType.Circle:
                    RenderCircleShape(target, (CircleShape)shape, renderParams);
                    break;
            }
        }

        private void RenderRectShape(Rendering.RenderTarget target, RectangleShape shape, RenderParams renderParams)
        {
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;

            SFML.Graphics.RectangleShape sfRect = new SFML.Graphics.RectangleShape(new SFML.System.Vector2f(shape.width, shape.height));
            sfRect.Position = new SFML.System.Vector2f(renderParams.position.x, renderParams.position.y);
            sfRect.FillColor = GetSfmlColor(renderParams.tintColor);

            sfTarget.Draw(sfRect);
        }

        private void RenderCircleShape(Rendering.RenderTarget target, CircleShape shape, RenderParams renderParams)
        { 
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;
            
            SFML.Graphics.CircleShape sfCircle = new SFML.Graphics.CircleShape(shape.radius);
            sfCircle.Position = new SFML.System.Vector2f(renderParams.position.x, renderParams.position.y);
            sfCircle.FillColor = GetSfmlColor(renderParams.tintColor);

            sfTarget.Draw(sfCircle);
        } 

        public override void DrawText(Rendering.RenderTarget target, Font font, string text, uint fontSize, RenderParams renderParams)
        {
            if (!renderParams.enabled)
                return;
            
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;

            SFML.Graphics.Text sfText = new SFML.Graphics.Text(text, font.resource, fontSize);
            sfText.Position = new SFML.System.Vector2f(renderParams.position.x, renderParams.position.y);
            sfText.FillColor = GetSfmlColor(renderParams.tintColor);

            sfTarget.Draw(sfText);
            sfText.Dispose();
        }

        public override void DrawTexture(RenderTarget target, Texture texture, RenderParams renderParams)
        {
            if (!renderParams.enabled)
                return;
            
            SFML.Graphics.Texture sfTexture = GetValidTexture(texture);
            if (sfTexture == null)
                return;
            
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;

            SFML.Graphics.Sprite sfSprite = new SFML.Graphics.Sprite(sfTexture);
            if (renderParams.renderRect != null)
            {
                sfSprite.TextureRect = 
                    new SFML.Graphics.IntRect(renderParams.renderRectOffset.x, renderParams.renderRectOffset.y, 
                    (int)renderParams.renderRect.width, (int)renderParams.renderRect.height);
            }
            sfSprite.Position = new SFML.System.Vector2f(renderParams.position.x, renderParams.position.y);
            sfSprite.Color = GetSfmlColor(renderParams.tintColor);
            
            sfTarget.Draw(sfSprite);
        }

        public override Vector2f GetTextBounds(Font font, string text, uint fontSize)
        {
            SFML.Graphics.Text sfText = new SFML.Graphics.Text(text, font.resource, fontSize);
            SFML.Graphics.FloatRect bounds = sfText.GetLocalBounds();
            sfText.Dispose();

            return new Vector2f(bounds.Width, bounds.Height);
        }

        public override void InitTexture(Texture texture, uint w, uint h)
        { 
            if (textures.ContainsKey(texture.rid))
            {
                Log.WriteError("Invalid texture to init: texture is already bound to existing data. Please destroy the current one before reuse.");
                return;
            }

            SFML.Graphics.Texture sfTexture = new SFML.Graphics.Texture(w, h);
            texture.width = sfTexture.Size.X;
            texture.height = sfTexture.Size.Y;
            textures.Add(texture.rid, (texture, sfTexture));

            Log.WriteLine("New SFML texture initialized: w=" + w + ", h=" + h + ", rid=" + texture.rid);
        }

        public override void DestroyTexture(Texture texture)
        { 
            SFML.Graphics.Texture sfTexture = GetValidTexture(texture);
            if (sfTexture == null)
                return;

            Log.WriteLine("SFML texture queued for destruction. rid=" + texture.rid);
            textures.Remove(texture.rid);
            destructionPending.Add((texture, sfTexture));
        }

        public override void SetTextureData(Texture texture, byte[] data)
        {
            SFML.Graphics.Texture sfTexture = GetValidTexture(texture);
            if (sfTexture == null)
                return;

            sfTexture.Update(data);
        }

        public override void InitRenderTarget(Rendering.RenderTarget target, uint w, uint h)
        {
            if (renderTargets.ContainsKey(target.rid))
            {
                Log.WriteError("Invalid rendertarget for init: target is already bound. Please destroy target the target first then re-initialize.");
                return;
            }

            SFML.Graphics.RenderTexture sfTarget = new SFML.Graphics.RenderTexture(w, h);
            target.width = sfTarget.Size.X;
            target.height = sfTarget.Size.Y;
            renderTargets.Add(target.rid, (target, sfTarget));

            Texture wryTexture = ResourceManager.Global.CreateResource<Texture>("BlindRenderTextures/" + target.rid.ToString() + "_SFML");
            wryTexture.width = sfTarget.Size.X;
            wryTexture.height = sfTarget.Size.Y;
            target.textureRid = wryTexture.rid;
            Log.WriteLine("Texture (rid=" + wryTexture.rid + ") bound to rendertarget, resized to: w=" + wryTexture.width + ", h=" + wryTexture.height);

            //update where the wryTexture maps to
            destructionPending.Add((null, textures[wryTexture.rid].sfTexture));
            textures.Remove(wryTexture.rid);
            textures.Add(wryTexture.rid, (wryTexture, sfTarget.Texture));

            Log.WriteLine("New SFML rendertarget initialized: w=" + w + ", h=" + h + ", rid=" + target.rid);
        }

        public override void BindRenderTarget(Rendering.RenderTarget target, Window window)
        {
            if (WindowServer.Global is SFWindowServer sfWindowServer)
            {
                SFML.Graphics.RenderWindow sfRenderWindow = sfWindowServer.GetRenderWindow(window);
                target.width = sfRenderWindow.Size.X;
                target.height = sfRenderWindow.Size.Y;

                //destroy original sfTarget (since we're replacing it), but keep original wryRef. Replace in-place.
                destructionPending.Add((null, renderTargets[target.rid].sfTarget));
                renderTargets.Remove(target.rid);
                renderTargets.Add(target.rid, (target, sfRenderWindow));

                Log.WriteLine("New SFML rendertarget bound to window (id=" + window.windowId + ", title=" + window.title + "), rid=" + target.rid);
            }
            else
                Log.WriteError("Unable to bind SFML render target to window, window server is not SFML-based (mismatching data).");
        }

        public override void DestroyRenderTarget(Rendering.RenderTarget target)
        {
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;

            Log.WriteLine("SFML rendertarget queued for destruction. rid=" + target.rid);
            renderTargets.Remove(target.rid);
            destructionPending.Add((target, sfTarget));
        }

        public override void ClearRenderTarget(Rendering.RenderTarget target, Weary.Rendering.Color clearColor)
        {
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;

            sfTarget.Clear(GetSfmlColor(clearColor));
        }

        public override void DisplayRenderTarget(Rendering.RenderTarget target)
        {
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return;

            if (sfTarget is SFML.Graphics.RenderTexture sfRenderTexture)
                sfRenderTexture.Display();
            else if (sfTarget is SFML.Graphics.RenderWindow sfRenderWindow)
                sfRenderWindow.Display();
            else
                Log.WriteError("SFML RenderTarget does not support display() call (if you see this, panic). rid=" + target.rid);
        }

        public override Texture GetRenderTargetTexture(RenderTarget target)
        {
            SFML.Graphics.RenderTarget sfTarget = GetValidRenderTarget(target);
            if (sfTarget == null)
                return null;

            if (sfTarget is SFML.Graphics.RenderTexture sfTexture)
            {
                return textures[target.textureRid].wryTexture;
            }
            else
            {
                Log.WriteError("SFML RenderTarget is not a renderTexture, cannot obtain the displayed texture.");
                return null;
            }
        }

        private SFML.Graphics.Texture GetValidTexture(Rendering.Texture texture)
        {
            if (texture == null)
            {
                Log.WriteError("Invalid texture for operation: texture is null.");
                return null;
            }
            else if (!textures.ContainsKey(texture.rid))
            {
                Log.WriteError("Invalid texture for operation: texture is not owned by this render server.");
                return null;
            }
            return textures[texture.rid].sfTexture;
        }

        private SFML.Graphics.RenderTarget GetValidRenderTarget(Rendering.RenderTarget target)
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