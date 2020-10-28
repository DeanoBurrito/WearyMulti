using System;

namespace Weary
{
    public enum InputKey : int
    {
        InvalidKey = 0,

        //(Keyboard) Alphanumerics
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        Num0,
        Num1,
        Num2,
        Num3,
        Num4,
        Num5,
        Num6,
        Num7,
        Num8,
        Num9,
        Numpad0,
        Numpad1,
        Numpad2,
        Numpad3,
        Numpad4,
        Numpad5,
        Numpad6,
        Numpad7,
        Numpad8,
        Numpad9,

        //(Keyboard) Function keys
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        F11,
        F12,

        //(Keyboard) Modifiers
        ShiftLeft,
        ShiftRight,
        CtrlLeft,
        CtrlRight,
        AltLeft,
        AltRight,
        
        //(Keyboard) Utilities
        Tab,
        Escape,
        Space,
        Enter,
        Backspace,
        Backslash,
        Forwardslash,
        Semicolon,
        Dot,
        Comma,
        Quote,
        BracketLeft,
        BracketRight,
        Backtick,
        Equals,
        Hyphen,

        //(Keyboard) Navigation
        ArrowUp,
        ArrowDown,
        ArrowLeft,
        ArrowRight,
        PageUp,
        PageDown,
        Home,
        End,
        Insert,
        Delete,

        //(Mouse) buttons
        MouseLeft,
        MouseMiddle,
        MouseRight,

        //(Joystick) functions

        //(Gamepad) buttons

        Count
    }
}