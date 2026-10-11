using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Game;
using Mokus2D.Input;

using SDL3;

namespace ContreJourDX.Desktop.Platform
{
    // Turns SDL events into the held state the engine polls once per frame. Positions come in as
    // window points and are handed out on the game's canvas, through the host's letterbox.
    public sealed class SdlInputState : IInputSource
    {
        // XNA's ScrollWheelValue moves 120 per notch; SDL reports about 1.0 per notch.
        private const float WheelUnitsPerNotch = 120f;

        // SDL_TOUCH_MOUSEID and SDL_MOUSE_TOUCHID: SDL's synthetic copies of touch as mouse and of
        // mouse as touch. The originals are handled, so the copies would count twice.
        private const uint TouchMouseId = uint.MaxValue;

        private const ulong MouseTouchId = ulong.MaxValue;

        // The engine's Key values are XNA's, so each is mapped by name rather than arithmetic.
        private static readonly Dictionary<SDL.Keycode, Key> KeyMap = new()
        {
            [SDL.Keycode.Backspace] = Key.Back,
            [SDL.Keycode.Return] = Key.Enter,
            [SDL.Keycode.KpEnter] = Key.Enter,
            [SDL.Keycode.Capslock] = Key.CapsLock,
            [SDL.Keycode.Escape] = Key.Escape,
            [SDL.Keycode.Left] = Key.Left,
            [SDL.Keycode.Up] = Key.Up,
            [SDL.Keycode.Right] = Key.Right,
            [SDL.Keycode.Down] = Key.Down,
            [SDL.Keycode.Delete] = Key.Delete,
            [SDL.Keycode.A] = Key.A,
            [SDL.Keycode.D] = Key.D,
            [SDL.Keycode.S] = Key.S,
            [SDL.Keycode.W] = Key.W,
            [SDL.Keycode.F5] = Key.F5,
            [SDL.Keycode.LShift] = Key.LeftShift,
            [SDL.Keycode.RShift] = Key.RightShift,
        };

        private readonly Func<Letterbox> _letterbox;

        private readonly Action<Vector2> _warpMouse;

        private readonly HashSet<SDL.Keycode> _heldKeys = [];

        private readonly HashSet<uint> _padsHoldingBack = [];

        private readonly Dictionary<(ulong Touch, ulong Finger), TouchPoint> _fingers = [];

        private Vector2 _mousePosition;

        private bool _left;

        private bool _middle;

        private bool _right;

        private float _wheel;

        private int _nextFingerId = 1;

        public SdlInputState(Func<Letterbox> letterbox, Action<Vector2> warpMouse)
        {
            ArgumentNullException.ThrowIfNull(letterbox);
            ArgumentNullException.ThrowIfNull(warpMouse);
            _letterbox = letterbox;
            _warpMouse = warpMouse;
        }

        public bool IsBackPressed => _heldKeys.Contains(SDL.Keycode.Escape) || _padsHoldingBack.Count > 0;

        // F11 or Alt+Enter, pressed (not repeated): the host's full-screen toggle.
        public static bool IsFullScreenShortcut(in SDL.Event e)
        {
            return (SDL.EventType)e.Type == SDL.EventType.KeyDown && e.Key.Down && !e.Key.Repeat
                && (e.Key.Key == SDL.Keycode.F11
                    || (e.Key.Key == SDL.Keycode.Return && (e.Key.Mod & SDL.Keymod.Alt) != 0));
        }

        public void HandleEvent(in SDL.Event e)
        {
            // Deliberately not a switch: the populate-switch fixer rewrites one over this enum into
            // every one of its members.
            SDL.EventType type = (SDL.EventType)e.Type;
            if (type == SDL.EventType.MouseMotion)
            {
                if (e.Motion.Which != TouchMouseId)
                {
                    _mousePosition = ToCanvas(e.Motion.X, e.Motion.Y);
                }
            }
            else if (type is SDL.EventType.MouseButtonDown or SDL.EventType.MouseButtonUp)
            {
                if (e.Button.Which != TouchMouseId)
                {
                    _mousePosition = ToCanvas(e.Button.X, e.Button.Y);
                    SetButton(e.Button.Button, e.Button.Down);
                }
            }
            else if (type == SDL.EventType.MouseWheel)
            {
                if (e.Wheel.Which != TouchMouseId)
                {
                    float notches = e.Wheel.Direction == SDL.MouseWheelDirection.Flipped ? -e.Wheel.Y : e.Wheel.Y;
                    _wheel += notches * WheelUnitsPerNotch;
                }
            }
            else if (type is SDL.EventType.KeyDown or SDL.EventType.KeyUp)
            {
                _ = e.Key.Down ? _heldKeys.Add(e.Key.Key) : _heldKeys.Remove(e.Key.Key);
            }
            else if (type is SDL.EventType.FingerDown or SDL.EventType.FingerMotion or SDL.EventType.FingerUp or SDL.EventType.FingerCanceled)
            {
                HandleFinger(type, e.TFinger);
            }
            else if (type is SDL.EventType.GamepadButtonDown or SDL.EventType.GamepadButtonUp)
            {
                if (e.GButton.Button == (byte)SDL.GamepadButton.Back)
                {
                    _ = e.GButton.Down ? _padsHoldingBack.Add(e.GButton.Which) : _padsHoldingBack.Remove(e.GButton.Which);
                }
            }
            else if (type is SDL.EventType.WindowFocusLost or SDL.EventType.WindowMinimized)
            {
                // Releases that happen while another window has focus never arrive here.
                ReleaseAll();
            }
        }

        public MouseSnapshot GetMouse()
        {
            return new MouseSnapshot(_mousePosition, _left, _middle, _right, (int)_wheel);
        }

        public void SetMousePosition(int x, int y)
        {
            _warpMouse(_letterbox().LogicalToWindow(new Vector2(x, y)));
        }

        public void GetPressedKeys(List<Key> into)
        {
            ArgumentNullException.ThrowIfNull(into);
            into.Clear();
            foreach (SDL.Keycode key in _heldKeys)
            {
                if (KeyMap.TryGetValue(key, out Key mapped) && !into.Contains(mapped))
                {
                    into.Add(mapped);
                }
            }
        }

        public bool IsKeyDown(Key key)
        {
            foreach (SDL.Keycode held in _heldKeys)
            {
                if (KeyMap.TryGetValue(held, out Key mapped) && mapped == key)
                {
                    return true;
                }
            }
            return false;
        }

        public void GetTouches(List<TouchPoint> into)
        {
            ArgumentNullException.ThrowIfNull(into);
            into.Clear();
            into.AddRange(_fingers.Values);
        }

        private void HandleFinger(SDL.EventType type, in SDL.TouchFingerEvent finger)
        {
            if (finger.TouchID == MouseTouchId)
            {
                return;
            }
            (ulong, ulong) key = (finger.TouchID, finger.FingerID);
            // A touch the system takes over (an edge swipe, a removed device) ends canceled, not up.
            if (type is SDL.EventType.FingerUp or SDL.EventType.FingerCanceled)
            {
                _ = _fingers.Remove(key);
                return;
            }
            // Touch positions arrive normalized to the window.
            Vector2 windowSize = _letterbox().WindowSize;
            Vector2 position = ToCanvas(finger.X * windowSize.X, finger.Y * windowSize.Y);
            if (_fingers.TryGetValue(key, out TouchPoint existing))
            {
                _fingers[key] = existing with { Position = position };
            }
            else if (type == SDL.EventType.FingerDown)
            {
                _fingers[key] = new TouchPoint(_nextFingerId++, position);
            }
        }

        private void SetButton(byte button, bool down)
        {
            switch (button)
            {
                case 1:
                    _left = down;
                    break;
                case 2:
                    _middle = down;
                    break;
                case 3:
                    _right = down;
                    break;
                default:
                    break;
            }
        }

        // Releases while the window is away never arrive, so the host drops everything held.
        public void ReleaseAll()
        {
            _left = false;
            _middle = false;
            _right = false;
            _heldKeys.Clear();
            _padsHoldingBack.Clear();
            _fingers.Clear();
        }

        private Vector2 ToCanvas(float windowX, float windowY)
        {
            return _letterbox().WindowToLogical(new Vector2(windowX, windowY));
        }
    }
}
