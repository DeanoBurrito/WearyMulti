using System;

namespace Weary.Rendering
{
    public abstract class WindowServer
    {
        public static WindowServer Global { get; protected set; }

        public abstract void Init();
        public abstract void Deinit();
        public abstract Window CreateWindow(uint width, uint height, bool allowResize, string title);
        public abstract void DestroyWindow(Window window);
        public abstract void SetWindowSize(Window window, uint nWidth, uint nHeight);
        public abstract void SetWindowPosition(Window window, int x, int y);
        public abstract void SetWindowDesktop(Window window, int desktop);
        public abstract void SetWindowTitle(Window window, string title);
        public abstract void SetAllowResize(Window window, bool enabled);
        public abstract void HandleEvents();
    }
}