using System;

namespace Weary.Scene
{
    public enum NodeUpdatePolicy : byte
    {
        FollowParent = 0,
        AlwaysExecute = 1,
        NeverExecute = 2,
    }
}