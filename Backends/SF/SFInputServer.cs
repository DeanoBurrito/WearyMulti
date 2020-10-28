using System;
using System.Collections.Generic;
using System.Diagnostics;
using SFML.Window;

namespace Weary
{
    public sealed class SFInputServer : InputServer
    {
        Dictionary<int, int> sfToWryMaps = new Dictionary<int, int>()
        {
            { (int)Keyboard.Key.A, (int)InputKey.A },
            { (int)Keyboard.Key.B, (int)InputKey.B },
            { (int)Keyboard.Key.C, (int)InputKey.C },
            { (int)Keyboard.Key.D, (int)InputKey.D },
            { (int)Keyboard.Key.E, (int)InputKey.E },
            { (int)Keyboard.Key.F, (int)InputKey.F },
            { (int)Keyboard.Key.G, (int)InputKey.G },
            { (int)Keyboard.Key.H, (int)InputKey.H },
            { (int)Keyboard.Key.I, (int)InputKey.I },
            { (int)Keyboard.Key.J, (int)InputKey.J },
            { (int)Keyboard.Key.K, (int)InputKey.K },
            { (int)Keyboard.Key.L, (int)InputKey.L },
            { (int)Keyboard.Key.M, (int)InputKey.M },
            { (int)Keyboard.Key.N, (int)InputKey.N },
            { (int)Keyboard.Key.O, (int)InputKey.O },
            { (int)Keyboard.Key.P, (int)InputKey.P },
            { (int)Keyboard.Key.Q, (int)InputKey.Q },
            { (int)Keyboard.Key.R, (int)InputKey.R },
            { (int)Keyboard.Key.S, (int)InputKey.S },
            { (int)Keyboard.Key.T, (int)InputKey.T },
            { (int)Keyboard.Key.U, (int)InputKey.U },
            { (int)Keyboard.Key.V, (int)InputKey.V },
            { (int)Keyboard.Key.W, (int)InputKey.W },
            { (int)Keyboard.Key.X, (int)InputKey.X },
            { (int)Keyboard.Key.Y, (int)InputKey.Y },
            { (int)Keyboard.Key.Z, (int)InputKey.Z },

            { (int)Keyboard.Key.Num0, (int)InputKey.Num0 },
            { (int)Keyboard.Key.Num1, (int)InputKey.Num1 },
            { (int)Keyboard.Key.Num2, (int)InputKey.Num2 },
            { (int)Keyboard.Key.Num3, (int)InputKey.Num3 },
            { (int)Keyboard.Key.Num4, (int)InputKey.Num4 },
            { (int)Keyboard.Key.Num5, (int)InputKey.Num5 },
            { (int)Keyboard.Key.Num6, (int)InputKey.Num6 },
            { (int)Keyboard.Key.Num7, (int)InputKey.Num7 },
            { (int)Keyboard.Key.Num8, (int)InputKey.Num8 },
            { (int)Keyboard.Key.Num9, (int)InputKey.Num9 },
            { (int)Keyboard.Key.Numpad0, (int)InputKey.Numpad0 },
            { (int)Keyboard.Key.Numpad1, (int)InputKey.Numpad1 },
            { (int)Keyboard.Key.Numpad2, (int)InputKey.Numpad2 },
            { (int)Keyboard.Key.Numpad3, (int)InputKey.Numpad3 },
            { (int)Keyboard.Key.Numpad4, (int)InputKey.Numpad4 },
            { (int)Keyboard.Key.Numpad5, (int)InputKey.Numpad5 },
            { (int)Keyboard.Key.Numpad6, (int)InputKey.Numpad6 },
            { (int)Keyboard.Key.Numpad7, (int)InputKey.Numpad7 },
            { (int)Keyboard.Key.Numpad8, (int)InputKey.Numpad8 },
            { (int)Keyboard.Key.Numpad9, (int)InputKey.Numpad9 },

            { (int)Keyboard.Key.F1, (int)InputKey.F1 },
            { (int)Keyboard.Key.F2, (int)InputKey.F2 },
            { (int)Keyboard.Key.F3, (int)InputKey.F3 },
            { (int)Keyboard.Key.F4, (int)InputKey.F4 },
            { (int)Keyboard.Key.F5, (int)InputKey.F5 },
            { (int)Keyboard.Key.F6, (int)InputKey.F6 },
            { (int)Keyboard.Key.F7, (int)InputKey.F7 },
            { (int)Keyboard.Key.F8, (int)InputKey.F8 },
            { (int)Keyboard.Key.F9, (int)InputKey.F9 },
            { (int)Keyboard.Key.F10, (int)InputKey.F10 },
            { (int)Keyboard.Key.F11, (int)InputKey.F11 },
            { (int)Keyboard.Key.F12, (int)InputKey.F12 },

            { (int)Keyboard.Key.LShift, (int)InputKey.ShiftLeft },
            { (int)Keyboard.Key.RShift, (int)InputKey.ShiftRight },
            { (int)Keyboard.Key.LAlt, (int)InputKey.AltLeft },
            { (int)Keyboard.Key.RAlt, (int)InputKey.AltRight },
            { (int)Keyboard.Key.LControl, (int)InputKey.CtrlLeft },
            { (int)Keyboard.Key.RControl, (int)InputKey.CtrlRight },

            { (int)Keyboard.Key.Space, (int)InputKey.Space },
            { (int)Keyboard.Key.Tab, (int)InputKey.Tab },
            { (int)Keyboard.Key.Escape, (int)InputKey.Escape },
            { (int)Keyboard.Key.Enter, (int)InputKey.Enter },
            { (int)Keyboard.Key.Backspace, (int)InputKey.Backspace },
            { (int)Keyboard.Key.Backslash, (int)InputKey.Backslash },
            { (int)Keyboard.Key.Slash, (int)InputKey.Forwardslash },
            { (int)Keyboard.Key.Semicolon, (int)InputKey.Semicolon },
            { (int)Keyboard.Key.Period, (int)InputKey.Dot },
            { (int)Keyboard.Key.Comma, (int)InputKey.Comma },
            { (int)Keyboard.Key.Quote, (int)InputKey.Quote },
            { (int)Keyboard.Key.LBracket, (int)InputKey.BracketLeft },
            { (int)Keyboard.Key.RBracket, (int)InputKey.BracketRight },
            { (int)Keyboard.Key.Tilde, (int)InputKey.Backtick },
            { (int)Keyboard.Key.Equal, (int)InputKey.Equals },
            { (int)Keyboard.Key.Hyphen, (int)InputKey.Hyphen },

            { (int)Keyboard.Key.Up, (int)InputKey.ArrowUp },
            { (int)Keyboard.Key.Down, (int)InputKey.ArrowDown },
            { (int)Keyboard.Key.Left, (int)InputKey.ArrowLeft },
            { (int)Keyboard.Key.Right, (int)InputKey.ArrowRight },
            { (int)Keyboard.Key.PageUp, (int)InputKey.PageUp },
            { (int)Keyboard.Key.PageDown, (int)InputKey.PageDown },
            { (int)Keyboard.Key.Home, (int)InputKey.Home },
            { (int)Keyboard.Key.End, (int)InputKey.End },
            { (int)Keyboard.Key.Insert, (int)InputKey.Insert },
            { (int)Keyboard.Key.Delete, (int)InputKey.Delete },

            { (int)Keyboard.Key.KeyCount + (int)Mouse.Button.Left, (int)InputKey.MouseLeft },
            { (int)Keyboard.Key.KeyCount + (int)Mouse.Button.Middle, (int)InputKey.MouseMiddle },
            { (int)Keyboard.Key.KeyCount + (int)Mouse.Button.Right, (int)InputKey.MouseRight },
        };

        bool[] currKeyStates;
        bool[] prevKeyStates;
        Vector2f[] axis;
        Dictionary<Keyboard.Key, int> heldTimes = new Dictionary<Keyboard.Key, int>();

        Stopwatch precisionClock;
        
        public override void Init()
        {
            Log.WriteLine("Initialising SFML-based input server.");
            if (Global == null)
                Global = this;
            else
                Log.WriteError("Render server is already running and marked as global. This instance will continue, this use case is unsupported.");

            int totalCount = (int)Keyboard.Key.KeyCount + (int)Mouse.Button.ButtonCount;
            currKeyStates = new bool[totalCount];
            prevKeyStates = new bool[totalCount];
            axis = new Vector2f[3];
            
            precisionClock = new Stopwatch();
            precisionClock.Start();

            HandleEvents(); //initial scan to populate arrays with data (even if its junk right now)
        }

        public override void Deinit()
        {
            Log.WriteLine("Deinitialising SFML-based input server.");
            if (Global == this)
                Global = null;

            //mark any remaining resources for destruction here.

            HandleEvents();
        }

        public override void HandleEvents()
        {
            precisionClock.Stop();
            int deltaMs = (int)precisionClock.ElapsedMilliseconds;
            precisionClock.Reset();
            precisionClock.Start();

            Array.Copy(currKeyStates, prevKeyStates, currKeyStates.Length);

            for (int i = 0; i < currKeyStates.Length; i++)
            {
                if (i < (int)Keyboard.Key.KeyCount)
                    currKeyStates[i] = Keyboard.IsKeyPressed((Keyboard.Key)i);
                else if (i == (int)Keyboard.Key.KeyCount)
                    continue;
                else
                {
                    int mIdx = i - (int)Keyboard.Key.KeyCount;
                    currKeyStates[i] = Mouse.IsButtonPressed((Mouse.Button)mIdx);
                }

                Keyboard.Key sfKey = (Keyboard.Key)i;
                if (currKeyStates[i])
                {
                    if (!heldTimes.ContainsKey(sfKey))
                        heldTimes.Add(sfKey, 0);
                    heldTimes[sfKey] += deltaMs;
                }
                else if (heldTimes.ContainsKey(sfKey))
                    heldTimes.Remove(sfKey);
            }

            SFML.System.Vector2i mpos = Mouse.GetPosition();
            axis[0] = new Vector2f(mpos.X, mpos.Y);
            axis[1] = Vector2f.Zero;
            axis[2] = Vector2f.Zero;
        }

        public override string GetServerInfo()
        {
            return "Weary (SFML based) InputServer.";
        }

        public override InputState GetInputState()
        {
            bool[] stateCurr = new bool[(int)InputKey.Count];
            bool[] statePrev = new bool[(int)InputKey.Count];
            Dictionary<InputKey, int> held = new Dictionary<InputKey, int>();
            for (int i = 0; i < (int)InputKey.Count; i++)
            {
                stateCurr[i] = statePrev[i] = false; //set all values to false by default
            }

            //apply actual values
            for (int i = 0; i < (int)currKeyStates.Length; i++)
            {
                if (currKeyStates[i])
                    stateCurr[sfToWryMaps[i]] = true;
            }
            for (int i = 0; i < (int)prevKeyStates.Length; i++)
            {
                if (prevKeyStates[i])
                    statePrev[sfToWryMaps[i]] = true;
            }

            foreach (var pair in heldTimes)
            {
                InputKey wryKey = (InputKey)sfToWryMaps[(int)pair.Key];
                held.Add(wryKey, pair.Value);
            }

            return new InputState(stateCurr, statePrev, held, axis);
        }
    }
}