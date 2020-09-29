using System;

namespace Weary
{
    public struct DeltaTime
    {
        public readonly float seconds;
        public readonly int milliseconds;

        public DeltaTime(float seconds)
        {
            this.seconds = seconds;
            milliseconds = (int)(seconds * 1000f);
        }
    }
}