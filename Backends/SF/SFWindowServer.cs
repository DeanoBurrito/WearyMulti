using System;
using System.Collections.Generic;
using Weary.Rendering;
using SFML.System;
using SFML.Graphics;

namespace Weary.Backends.SF
{
    public sealed class SFWindowServer : WindowServer
    {
        Dictionary<ulong, (Window wryWindow, RenderWindow sfWindow)> windows = new Dictionary<ulong, (Window, RenderWindow)>();
        List<(Window wryWindow, RenderWindow sfWindow)> destructionPending = new List<(Window wryWindow, RenderWindow sfWindow)>();
        UuidManager idManager = new UuidManager();

        public RenderWindow GetRenderWindow(Window window)
        {
            return GetValidWindow(window);
        }

        public override void Init()
        {
            Log.WriteLine("SFML Window server initializing.");
            if (Global == null)
                Global = this;
            else
                Log.WriteError("Window server is already running and marked as global. This instance will continue, this use case is unsupported.");
        }

        public override void Deinit()
        {
            if (Global == this)
                Global = null;

            Log.WriteLine("Window server is cleaning up " + windows.Count + " windows before shutdown.");
            foreach ((Window sysWindow, RenderWindow sfWindow) window in windows.Values)
            {
                DestroyWindow(window.sysWindow);
            }
            
            HandleEvents();
            Log.WriteLine("SFML Window server shutting down.");
        }

        public override Window CreateWindow(uint width, uint height, bool allowResize, string title)
        {
            ulong newId = idManager.GenerateId();
            SFML.Window.Styles sfStyle;
            if (width == SFML.Window.VideoMode.DesktopMode.Width && height == SFML.Window.VideoMode.DesktopMode.Height)
                sfStyle = SFML.Window.Styles.Fullscreen;
            else
                sfStyle = SFML.Window.Styles.Close;
            SFML.Window.VideoMode videoMode = new SFML.Window.VideoMode(width, height);
            RenderWindow sfWindow = new RenderWindow(videoMode, title, sfStyle);

            Window wryWindow = new Window(newId, width, height, title, sfStyle == SFML.Window.Styles.Fullscreen);
            wryWindow.allowResize = allowResize;
            windows.Add(newId, (wryWindow, sfWindow));

            sfWindow.Closed += (object sender, EventArgs e) => { DestroyWindow(wryWindow); };

            Log.WriteLine("New SFML window created: " + wryWindow.ToString());
            return wryWindow;
        }

        private RenderWindow GetValidWindow(Window window)
        {
            if (window == null)
            {
                Log.WriteError("Invalid window for operation: window is null.");
                return null;
            }
            else if (!windows.ContainsKey(window.windowId) || windows[window.windowId].wryWindow != window)
            {
                Log.WriteError("Inalid window for operation: window is not owned by this server.");
                return null;
            }
            return windows[window.windowId].sfWindow;
        }

        public override void DestroyWindow(Window window)
        {
            RenderWindow sfWindow = GetValidWindow(window);
            if (sfWindow == null)
                return;

            destructionPending.Add(windows[window.windowId]);
            windows.Remove(window.windowId);
            Log.WriteLine("SFML window pending destruction: " + window.ToString());
        }

        public override void SetWindowSize(Window window, uint nWidth, uint nHeight)
        {
            RenderWindow sfWindow = GetValidWindow(window);
            if (sfWindow == null)
                return;

            sfWindow.Size = new Vector2u(nWidth, nHeight);
            window.width = nWidth;
            window.height = nHeight;
            window.TriggerOnResized(nWidth, nHeight);
        }

        public override void SetWindowPosition(Window window, int x, int y)
        {
            RenderWindow sfWindow = GetValidWindow(window);
            if (sfWindow == null)
                return;

            sfWindow.Position = new SFML.System.Vector2i(x, y);
            window.x = x;
            window.y = y;
            window.TriggerOnMoved(x, y);
        }

        public override void SetWindowDesktop(Window window, int desktop)
        {
            Log.WriteError("Explicitely setting desktops in SFML is not supported :(.");
        }

        public override void SetWindowTitle(Window window, string title)
        {
            RenderWindow sfWindow = GetValidWindow(window);
            if (sfWindow == null)
                return;

            sfWindow.SetTitle(title);
            window.title = title;
            window.TriggerOnTitleChanged(title);
        }

        public override void SetAllowResize(Window window, bool enabled)
        {
            Log.WriteError("Changing window styles is not supported with SFML.");
        }

        public override void HandleEvents()
        {
            foreach (var win in windows.Values)
            {
                win.sfWindow.DispatchEvents();
                MarshallToWearyWindow(win.sfWindow, win.wryWindow);
            }

            //any windows marked as closed now get handled properly, without modifying the enumeration
            for (int i = 0; i < destructionPending.Count; i++)
            {
                RenderWindow sfWindow = destructionPending[i].sfWindow;
                Window wryWindow = destructionPending[i].wryWindow;
                wryWindow.TriggerOnDestroy();

                Log.WriteLine("SFML window being destroyed: " + wryWindow.ToString());
                sfWindow.Close();
                sfWindow.Dispose();
                sfWindow = null;

                idManager.FreeId(wryWindow.windowId);
                wryWindow = null;
            }
            destructionPending.Clear();
        }

        private void MarshallToWearyWindow(RenderWindow sfWindow, Window wryWindow)
        {
            wryWindow.width = sfWindow.Size.X;
            wryWindow.height = sfWindow.Size.Y;
            wryWindow.x = sfWindow.Position.X;
            wryWindow.y = sfWindow.Position.Y;
        }
    }
}