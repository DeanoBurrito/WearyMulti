using System;

namespace Weary.Debug
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class DebugCommandLoaderAttribute : Attribute
    {

    }
}