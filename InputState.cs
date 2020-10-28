using System;
using System.Collections.Generic;

namespace Weary
{
    public sealed class InputState
    {
        bool[] currKeys;
        bool[] prevKeys;
        Dictionary<InputKey, int> heldKeys;
        Vector2f[] axisPositions;

        internal InputState(bool[] curr, bool[] prev, Dictionary<InputKey, int> held, Vector2f[] axis)
        {
            currKeys = curr;
            prevKeys = prev;
            heldKeys = held;
            axisPositions = axis;
        }

        public bool IsKeyDown(InputKey key)
        {
            return currKeys[(int)key];
        }

        public bool IsKeyPressed(InputKey key)
        {
            return currKeys[(int)key] && !prevKeys[(int)key];
        }

        public bool IsKeyReleased(InputKey key)
        {
            return !currKeys[(int)key] && prevKeys[(int)key];
        }

        public bool IsKeyHeld(InputKey key, int ms = 1000)
        {
            if (heldKeys.TryGetValue(key, out int currMs) && currMs >= ms)
                return true;
            return false;
        }

        public Vector2f GetMousePos()
        {
            if (axisPositions.Length < 1)
                return Vector2f.Zero; //uh, just in case I guess.
            return axisPositions[0];
        }

        public Vector2f GetAxis(int axisNum = 3)
        {
            if (axisNum >= axisPositions.Length)
                return Vector2f.Zero;
            
            return axisPositions[axisNum];
        }
    }
}