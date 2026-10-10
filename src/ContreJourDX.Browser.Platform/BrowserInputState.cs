using System;
using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Game;
using Mokus2D.Input;

namespace ContreJourDX.Browser.Platform
{
    // Turns the page's pointer, key and wheel events into the held state the engine polls once per frame, as
    // the desktop host's SdlInputState does for SDL. Positions arrive in CSS pixels relative to the canvas and
    // are handed out on the game's canvas, through the host's letterbox. Mouse and pen drive the mouse; touch
    // drives touches, each under an id the engine has not seen before.
    public sealed class BrowserInputState(Func<Letterbox> letterbox) : IInputSource
    {
        private readonly Func<Letterbox> _letterbox = letterbox ?? throw new ArgumentNullException(nameof(letterbox));

        // Key ids rather than keys, so two ids that share a key cannot release each other.
        private readonly HashSet<int> _heldKeys = [];

        private readonly Dictionary<int, TouchPoint> _touches = [];

        private Vector2 _mousePosition;

        private bool _left;

        private bool _middle;

        private bool _right;

        private int _wheel;

        private int _nextTouchId = 1;

        public bool IsBackPressed => IsKeyDown(Key.Escape);

        public void HandlePointer(PointerPhase phase, PointerKind kind, int pointerId, Vector2 cssPosition, int buttons)
        {
            Vector2 position = _letterbox().WindowToLogical(cssPosition);
            if (kind == PointerKind.Touch)
            {
                HandleTouch(phase, pointerId, position);
                return;
            }
            _mousePosition = position;
            // DOM `buttons`: 1 primary, 2 secondary, 4 auxiliary, already updated for the event's own button.
            int held = phase == PointerPhase.Cancel ? 0 : buttons;
            _left = (held & 1) != 0;
            _right = (held & 2) != 0;
            _middle = (held & 4) != 0;
        }

        public void HandleKey(int id, bool down)
        {
            if (BrowserKeys.Map(id) == Key.None)
            {
                return;
            }
            _ = down ? _heldKeys.Add(id) : _heldKeys.Remove(id);
        }

        // In the desktop's units: 120 per notch, positive away from the player.
        public void AddWheel(int units)
        {
            _wheel += units;
        }

        // Releases while the page is away never arrive, so the host drops everything held.
        public void ReleaseAll()
        {
            _left = false;
            _middle = false;
            _right = false;
            _heldKeys.Clear();
            _touches.Clear();
        }

        public MouseSnapshot GetMouse()
        {
            return new MouseSnapshot(_mousePosition, _left, _middle, _right, _wheel);
        }

        // A page cannot move the pointer.
        public void SetMousePosition(int x, int y)
        {
        }

        public void GetPressedKeys(List<Key> into)
        {
            ArgumentNullException.ThrowIfNull(into);
            into.Clear();
            foreach (int id in _heldKeys)
            {
                Key key = BrowserKeys.Map(id);
                if (!into.Contains(key))
                {
                    into.Add(key);
                }
            }
        }

        public bool IsKeyDown(Key key)
        {
            foreach (int id in _heldKeys)
            {
                if (BrowserKeys.Map(id) == key)
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
            into.AddRange(_touches.Values);
        }

        private void HandleTouch(PointerPhase phase, int pointerId, Vector2 position)
        {
            if (phase is PointerPhase.Up or PointerPhase.Cancel)
            {
                _ = _touches.Remove(pointerId);
            }
            else if (_touches.TryGetValue(pointerId, out TouchPoint existing))
            {
                _touches[pointerId] = existing with { Position = position };
            }
            else if (phase == PointerPhase.Down)
            {
                _touches[pointerId] = new TouchPoint(_nextTouchId++, position);
            }
        }
    }
}
