using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Game;
using Mokus2D.Input;

using Xunit;

namespace ContreJour.Browser.Platform.Tests
{
    public class BrowserInputStateTests
    {
        // A 1600x1000 canvas in an 800x500 CSS page with a 2x backing store: one CSS pixel is two canvas units.
        private static readonly Letterbox Retina = new(new Vector2(1600, 1000), new Vector2(800, 500), new Vector2(1600, 1000));

        private static BrowserInputState Create()
        {
            return new BrowserInputState(() => Retina);
        }

        [Fact]
        public void HoverMovesTheMouseWithoutPressing()
        {
            BrowserInputState input = Create();

            input.HandlePointer(PointerPhase.Move, PointerKind.Mouse, 1, new Vector2(100, 50), 0);

            MouseSnapshot mouse = input.GetMouse();
            Assert.Equal(new Vector2(200, 100), mouse.Position);
            Assert.False(mouse.Left);
        }

        [Fact]
        public void ButtonsFollowTheDomBitmask()
        {
            BrowserInputState input = Create();

            input.HandlePointer(PointerPhase.Down, PointerKind.Mouse, 1, new Vector2(10, 10), 1 | 2 | 4);
            MouseSnapshot held = input.GetMouse();
            input.HandlePointer(PointerPhase.Up, PointerKind.Mouse, 1, new Vector2(10, 10), 2);
            MouseSnapshot released = input.GetMouse();

            Assert.True(held.Left && held.Right && held.Middle);
            Assert.False(released.Left);
            Assert.True(released.Right);
        }

        [Fact]
        public void PenActsAsTheMouse()
        {
            BrowserInputState input = Create();

            input.HandlePointer(PointerPhase.Down, PointerKind.Pen, 9, new Vector2(10, 20), 1);

            Assert.True(input.GetMouse().Left);
            List<TouchPoint> touches = [];
            input.GetTouches(touches);
            Assert.Empty(touches);
        }

        [Fact]
        public void LiftingOneTouchKeepsTheOther()
        {
            BrowserInputState input = Create();
            List<TouchPoint> touches = [];

            input.HandlePointer(PointerPhase.Down, PointerKind.Touch, 11, new Vector2(10, 10), 1);
            input.HandlePointer(PointerPhase.Down, PointerKind.Touch, 12, new Vector2(20, 20), 1);
            input.GetTouches(touches);
            int second = touches.Find(t => t.Position == new Vector2(40, 40)).Id;
            input.HandlePointer(PointerPhase.Up, PointerKind.Touch, 11, new Vector2(10, 10), 0);
            input.HandlePointer(PointerPhase.Move, PointerKind.Touch, 12, new Vector2(30, 30), 1);
            input.GetTouches(touches);

            TouchPoint remaining = Assert.Single(touches);
            Assert.Equal(second, remaining.Id);
            Assert.Equal(new Vector2(60, 60), remaining.Position);

            input.HandlePointer(PointerPhase.Cancel, PointerKind.Touch, 12, new Vector2(30, 30), 0);
            input.GetTouches(touches);
            Assert.Empty(touches);
        }

        [Fact]
        public void TouchIdsAreNeverReused()
        {
            BrowserInputState input = Create();
            List<TouchPoint> touches = [];

            input.HandlePointer(PointerPhase.Down, PointerKind.Touch, 5, Vector2.Zero, 1);
            input.GetTouches(touches);
            int first = touches[0].Id;
            input.HandlePointer(PointerPhase.Up, PointerKind.Touch, 5, Vector2.Zero, 0);
            input.HandlePointer(PointerPhase.Down, PointerKind.Touch, 5, Vector2.Zero, 1);
            input.GetTouches(touches);

            Assert.NotEqual(first, touches[0].Id);
        }

        [Fact]
        public void AMoveForAnUnknownTouchIsIgnored()
        {
            BrowserInputState input = Create();
            List<TouchPoint> touches = [];

            input.HandlePointer(PointerPhase.Move, PointerKind.Touch, 3, Vector2.Zero, 0);
            input.GetTouches(touches);

            Assert.Empty(touches);
        }

        [Fact]
        public void WheelAccumulates()
        {
            BrowserInputState input = Create();

            input.AddWheel(120);
            input.AddWheel(-40);

            Assert.Equal(80, input.GetMouse().ScrollWheelValue);
        }

        [Fact]
        public void QHoldsBack()
        {
            BrowserInputState input = Create();

            input.HandleKey(2, down: true);

            Assert.True(input.IsBackPressed);
            Assert.True(input.IsKeyDown(Key.Escape));
            input.HandleKey(2, down: false);
            Assert.False(input.IsBackPressed);
        }

        [Fact]
        public void PressedKeysAreDistinctAndUnknownIdsIgnored()
        {
            BrowserInputState input = Create();
            List<Key> keys = [];

            input.HandleKey(1, down: true);
            input.HandleKey(2, down: true);
            input.HandleKey(7, down: true);
            input.HandleKey(99, down: true);
            input.GetPressedKeys(keys);

            Assert.Equal(2, keys.Count);
            Assert.Contains(Key.Escape, keys);
            Assert.Contains(Key.Left, keys);
        }

        [Fact]
        public void ReleaseAllDropsEverythingHeld()
        {
            BrowserInputState input = Create();
            List<TouchPoint> touches = [];
            List<Key> keys = [];

            input.HandlePointer(PointerPhase.Down, PointerKind.Mouse, 1, new Vector2(10, 10), 1);
            input.HandlePointer(PointerPhase.Down, PointerKind.Touch, 2, new Vector2(10, 10), 1);
            input.HandleKey(7, down: true);
            input.ReleaseAll();
            input.GetTouches(touches);
            input.GetPressedKeys(keys);

            Assert.False(input.GetMouse().Left);
            Assert.Empty(touches);
            Assert.Empty(keys);
            Assert.False(input.IsBackPressed);
        }

        [Fact]
        public void SetMousePositionIsIgnored()
        {
            BrowserInputState input = Create();
            input.HandlePointer(PointerPhase.Move, PointerKind.Mouse, 1, new Vector2(100, 50), 0);

            input.SetMousePosition(5, 5);

            Assert.Equal(new Vector2(200, 100), input.GetMouse().Position);
        }
    }
}
