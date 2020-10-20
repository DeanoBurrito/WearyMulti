using System;
using Weary.Resources;

namespace Weary.Rendering
{
    public sealed class Window
    {
        public readonly ulong windowId;
        public uint width { get; internal set;}
        public uint height { get; internal set;}
        public int x { get; internal set;} = 0;
        public int y { get; internal set;} = 0;
        public int desktop  { get; internal set;} = 0;
        public string title { get; internal set;}
        public bool fullscreen { get; internal set;}
        public bool allowResize { get; internal set;} = true;

        public event Action<object> onDestroy;
        public event Action<object, uint, uint> onResized;
        public event Action<object, int, int> onMoved;
        public event Action<object, string> onTitleChanged;

        public Window(ulong id, uint w, uint h, string t, bool f)
        {
            windowId = id;
            width = w;
            height = h;
            title = t;
            fullscreen = f;
        }

        internal void TriggerOnDestroy() => onDestroy?.Invoke(this);
        internal void TriggerOnResized(uint x, uint y) => onResized?.Invoke(this, x, y);
        internal void TriggerOnMoved(int x, int y) => onMoved?.Invoke(this, x, y);
        internal void TriggerOnTitleChanged(string t) => onTitleChanged(this, t);

        public override string ToString()
        {
            return "id=" + windowId + ", w=" + width + ", h=" + height + ", x=" + x + ", y=" + y + ", desktop=" + desktop + 
                ", title=" + title + ", isFullscreen=" + fullscreen + ", allowResize=" + allowResize;
        }
    }
}