using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Mokus2D.Input;

using SDL3;

using Xunit;

namespace ContreJour.Desktop.Platform.Tests
{
    public class SdlInputStateTests
    {
        // Canvas 1600x1000 shown in an 800x500-point window at 2x: window point p is canvas 2p.
        private static readonly Letterbox Retina = new(new Vector2(1600, 1000), new Vector2(800, 500), new Vector2(1600, 1000));

        private readonly List<Vector2> _warps = [];

        private SdlInputState CreateInput()
        {
            return new SdlInputState(() => Retina, _warps.Add);
        }

        private static SDL.Event Motion(float x, float y, uint which = 1)
        {
            SDL.Event e = default;
            e.Motion.Type = SDL.EventType.MouseMotion;
            e.Motion.Which = which;
            e.Motion.X = x;
            e.Motion.Y = y;
            return e;
        }

        private static SDL.Event Button(byte button, bool down, uint which = 1)
        {
            SDL.Event e = default;
            e.Button.Type = down ? SDL.EventType.MouseButtonDown : SDL.EventType.MouseButtonUp;
            e.Button.Which = which;
            e.Button.Button = button;
            e.Button.Down = down;
            return e;
        }

        private static SDL.Event Wheel(float y, SDL.MouseWheelDirection direction = SDL.MouseWheelDirection.Normal)
        {
            SDL.Event e = default;
            e.Wheel.Type = SDL.EventType.MouseWheel;
            e.Wheel.Which = 1;
            e.Wheel.Y = y;
            e.Wheel.Direction = direction;
            return e;
        }

        private static SDL.Event KeyEvent(SDL.Keycode key, bool down, bool repeat = false, SDL.Keymod mod = SDL.Keymod.None)
        {
            SDL.Event e = default;
            e.Key.Type = down ? SDL.EventType.KeyDown : SDL.EventType.KeyUp;
            e.Key.Key = key;
            e.Key.Down = down;
            e.Key.Repeat = repeat;
            e.Key.Mod = mod;
            return e;
        }

        private static SDL.Event Finger(SDL.EventType type, ulong finger, float x, float y, ulong touch = 1)
        {
            SDL.Event e = default;
            e.TFinger.Type = type;
            e.TFinger.TouchID = touch;
            e.TFinger.FingerID = finger;
            e.TFinger.X = x;
            e.TFinger.Y = y;
            return e;
        }

        private static SDL.Event PadButton(uint pad, SDL.GamepadButton button, bool down)
        {
            SDL.Event e = default;
            e.GButton.Type = down ? SDL.EventType.GamepadButtonDown : SDL.EventType.GamepadButtonUp;
            e.GButton.Which = pad;
            e.GButton.Button = (byte)button;
            e.GButton.Down = down;
            return e;
        }

        private static SDL.Event WindowEvent(SDL.EventType type)
        {
            SDL.Event e = default;
            e.Window.Type = type;
            return e;
        }

        private static List<TouchPoint> Touches(SdlInputState input)
        {
            List<TouchPoint> touches = [];
            input.GetTouches(touches);
            return touches;
        }

        [Fact]
        public void MouseMotionMapsWindowPointsToCanvas()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Motion(100, 50));

            Assert.Equal(new Vector2(200, 100), input.GetMouse().Position);
        }

        [Fact]
        public void MouseButtonsTrackLeftMiddleRight()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Button(1, true));
            input.HandleEvent(Button(2, true));
            input.HandleEvent(Button(3, true));
            MouseSnapshot held = input.GetMouse();
            input.HandleEvent(Button(1, false));
            input.HandleEvent(Button(2, false));
            input.HandleEvent(Button(3, false));
            MouseSnapshot released = input.GetMouse();

            Assert.True(held.Left && held.Middle && held.Right);
            Assert.False(released.Left || released.Middle || released.Right);
        }

        [Fact]
        public void MouseButtonTakesItsPositionFromTheEvent()
        {
            SdlInputState input = CreateInput();
            SDL.Event down = Button(1, true);
            down.Button.X = 10;
            down.Button.Y = 20;

            input.HandleEvent(down);

            Assert.Equal(new Vector2(20, 40), input.GetMouse().Position);
        }

        [Fact]
        public void SyntheticTouchMouseIsIgnored()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Motion(100, 50, which: uint.MaxValue));
            input.HandleEvent(Button(1, true, which: uint.MaxValue));

            Assert.Equal(new MouseSnapshot(Vector2.Zero, false, false, false, 0), input.GetMouse());
        }

        [Fact]
        public void WheelAccumulatesAtXnaScale()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Wheel(1f));
            input.HandleEvent(Wheel(0.5f));
            int afterHalf = input.GetMouse().ScrollWheelValue;
            input.HandleEvent(Wheel(0.5f));

            Assert.Equal(180, afterHalf);
            Assert.Equal(240, input.GetMouse().ScrollWheelValue);
        }

        [Fact]
        public void WheelFractionsBelowOneUnitAreKept()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Wheel(0.004f));
            input.HandleEvent(Wheel(0.004f));
            int afterTwo = input.GetMouse().ScrollWheelValue;
            input.HandleEvent(Wheel(0.004f));

            Assert.Equal(0, afterTwo);
            Assert.Equal(1, input.GetMouse().ScrollWheelValue);
        }

        [Fact]
        public void WheelFlippedDirectionIsNegated()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Wheel(1f, SDL.MouseWheelDirection.Flipped));

            Assert.Equal(-120, input.GetMouse().ScrollWheelValue);
        }

        [Fact]
        public void KeysAreHeldUntilReleased()
        {
            SdlInputState input = CreateInput();
            List<Key> pressed = [];

            input.HandleEvent(KeyEvent(SDL.Keycode.A, true));
            input.HandleEvent(KeyEvent(SDL.Keycode.LShift, true));
            input.GetPressedKeys(pressed);
            bool heldA = input.IsKeyDown(Key.A);
            input.HandleEvent(KeyEvent(SDL.Keycode.A, false));

            Assert.True(heldA);
            Assert.Equal([Key.A, Key.LeftShift], pressed.Order());
            Assert.False(input.IsKeyDown(Key.A));
            Assert.True(input.IsKeyDown(Key.LeftShift));
        }

        [Fact]
        public void KeysUnmappedAreIgnored()
        {
            SdlInputState input = CreateInput();
            List<Key> pressed = [Key.W];

            input.HandleEvent(KeyEvent(SDL.Keycode.F11, true));
            input.GetPressedKeys(pressed);

            Assert.Empty(pressed);
        }

        [Fact]
        public void EscapeIsBackWhileHeld()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(KeyEvent(SDL.Keycode.Escape, true));
            bool held = input.IsBackPressed;
            input.HandleEvent(KeyEvent(SDL.Keycode.Escape, false));

            Assert.True(held);
            Assert.False(input.IsBackPressed);
        }

        [Fact]
        public void GamepadBackIsBackWhileAnyPadHoldsIt()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(PadButton(7, SDL.GamepadButton.Back, true));
            input.HandleEvent(PadButton(9, SDL.GamepadButton.Back, true));
            input.HandleEvent(PadButton(7, SDL.GamepadButton.Back, false));
            bool oneHeld = input.IsBackPressed;
            input.HandleEvent(PadButton(9, SDL.GamepadButton.Back, false));

            Assert.True(oneHeld);
            Assert.False(input.IsBackPressed);
        }

        [Fact]
        public void GamepadOtherButtonsAreNotBack()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(PadButton(7, SDL.GamepadButton.South, true));

            Assert.False(input.IsBackPressed);
        }

        [Fact]
        public void FingersGetStableIdsAndCanvasPositions()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Finger(SDL.EventType.FingerDown, 100, 0.5f, 0.25f));
            input.HandleEvent(Finger(SDL.EventType.FingerDown, 200, 0.1f, 0.1f));
            input.HandleEvent(Finger(SDL.EventType.FingerMotion, 100, 0.25f, 0.5f));
            List<TouchPoint> both = Touches(input);
            input.HandleEvent(Finger(SDL.EventType.FingerUp, 100, 0.25f, 0.5f));
            List<TouchPoint> one = Touches(input);

            // (0.25, 0.5) of an 800x500-point window is point (200, 250), canvas (400, 500).
            Assert.Equal(new[] { new TouchPoint(1, new Vector2(400, 500)), new TouchPoint(2, new Vector2(160, 100)) }, both);
            Assert.Equal(new[] { new TouchPoint(2, new Vector2(160, 100)) }, one);
        }

        [Fact]
        public void FingersIdsAreNotReusedWhileOthersAreDown()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Finger(SDL.EventType.FingerDown, 100, 0f, 0f));
            input.HandleEvent(Finger(SDL.EventType.FingerUp, 100, 0f, 0f));
            input.HandleEvent(Finger(SDL.EventType.FingerDown, 100, 0f, 0f));

            Assert.Equal(2, Assert.Single(Touches(input)).Id);
        }

        [Fact]
        public void FingerCanceledReleasesTheTouch()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Finger(SDL.EventType.FingerDown, 100, 0.5f, 0.5f));
            input.HandleEvent(Finger(SDL.EventType.FingerCanceled, 100, 0.5f, 0.5f));

            Assert.Empty(Touches(input));
        }

        [Fact]
        public void SyntheticMouseTouchIsIgnored()
        {
            SdlInputState input = CreateInput();

            input.HandleEvent(Finger(SDL.EventType.FingerDown, 1, 0.5f, 0.5f, touch: ulong.MaxValue));

            Assert.Empty(Touches(input));
        }

        [Fact]
        public void FocusLostClearsHeldState()
        {
            SdlInputState input = CreateInput();
            input.HandleEvent(Button(1, true));
            input.HandleEvent(KeyEvent(SDL.Keycode.Escape, true));
            input.HandleEvent(PadButton(7, SDL.GamepadButton.Back, true));
            input.HandleEvent(Finger(SDL.EventType.FingerDown, 100, 0.5f, 0.5f));
            List<Key> pressed = [];

            input.HandleEvent(WindowEvent(SDL.EventType.WindowFocusLost));
            input.GetPressedKeys(pressed);

            Assert.False(input.GetMouse().Left);
            Assert.False(input.IsBackPressed);
            Assert.Empty(pressed);
            Assert.Empty(Touches(input));
        }

        [Fact]
        public void MinimizedClearsHeldState()
        {
            SdlInputState input = CreateInput();
            input.HandleEvent(Button(1, true));

            input.HandleEvent(WindowEvent(SDL.EventType.WindowMinimized));

            Assert.False(input.GetMouse().Left);
        }

        [Fact]
        public void SetMousePositionWarpsToWindowPoints()
        {
            SdlInputState input = CreateInput();

            input.SetMousePosition(200, 100);

            Assert.Equal(new[] { new Vector2(100, 50) }, _warps);
        }

        [Fact]
        public void IsFullScreenShortcutF11Press()
        {
            Assert.True(SdlInputState.IsFullScreenShortcut(KeyEvent(SDL.Keycode.F11, true)));
        }

        [Fact]
        public void IsFullScreenShortcutAltEnterPress()
        {
            Assert.True(SdlInputState.IsFullScreenShortcut(KeyEvent(SDL.Keycode.Return, true, mod: SDL.Keymod.LAlt)));
        }

        [Fact]
        public void IsFullScreenShortcutIgnoresRepeatReleaseAndPlainEnter()
        {
            Assert.False(SdlInputState.IsFullScreenShortcut(KeyEvent(SDL.Keycode.F11, true, repeat: true)));
            Assert.False(SdlInputState.IsFullScreenShortcut(KeyEvent(SDL.Keycode.F11, false)));
            Assert.False(SdlInputState.IsFullScreenShortcut(KeyEvent(SDL.Keycode.Return, true)));
        }
    }
}
