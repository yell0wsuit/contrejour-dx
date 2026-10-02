using System.Collections.Generic;

using Mokus2D.Input;

using Xunit;

namespace Mokus2D.Tests
{
    public class KeysControllerTests
    {
        private sealed class HeldInput : IInputSource
        {
            public HashSet<Key> Held { get; } = [];

            public bool IsBackPressed { get; set; }

            public MouseSnapshot GetMouse()
            {
                return default;
            }

            public void SetMousePosition(int x, int y)
            {
            }

            public void GetPressedKeys(List<Key> into)
            {
                into.Clear();
                into.AddRange(Held);
            }

            public bool IsKeyDown(Key key)
            {
                return Held.Contains(key);
            }

            public void GetTouches(List<TouchPoint> into)
            {
                into.Clear();
            }
        }

        private readonly HeldInput _input = new();

        private readonly KeysController _keys;

        public KeysControllerTests()
        {
            _keys = new KeysController(() => _input);
        }

        [Fact]
        public void KeyListenerFiresOncePerPress()
        {
            int presses = 0;
            _keys.AddKeyListener(Key.F5, () => presses++);

            _ = _input.Held.Add(Key.F5);
            _keys.Update(1f / 60);
            _keys.Update(1f / 60);
            Assert.Equal(1, presses);

            _ = _input.Held.Remove(Key.F5);
            _keys.Update(1f / 60);
            _ = _input.Held.Add(Key.F5);
            _keys.Update(1f / 60);
            Assert.Equal(2, presses);
        }

        [Fact]
        public void KeyListenerIgnoresOtherKeys()
        {
            int presses = 0;
            _keys.AddKeyListener(Key.Left, () => presses++);

            _ = _input.Held.Add(Key.Right);
            _keys.Update(1f / 60);

            Assert.Equal(0, presses);
        }

        [Fact]
        public void RemovedKeyListenerDoesNotFire()
        {
            int presses = 0;
            void Listener()
            {
                presses++;
            }
            _keys.AddKeyListener(Key.Enter, Listener);
            _keys.RemoveKeyListener(Key.Enter, Listener);

            _ = _input.Held.Add(Key.Enter);
            _keys.Update(1f / 60);

            Assert.Equal(0, presses);
        }

        [Fact]
        public void KeyHeldBeforeListenerIsAddedDoesNotFire()
        {
            _ = _input.Held.Add(Key.Enter);
            _keys.Update(1f / 60);
            int presses = 0;
            _keys.AddKeyListener(Key.Enter, () => presses++);

            _keys.Update(1f / 60);

            Assert.Equal(0, presses);
        }

        [Fact]
        public void ListenerAddedDuringPressWaitsForTheNextOne()
        {
            int presses = 0;
            _keys.AddKeyListener(Key.Enter, () => _keys.AddKeyListener(Key.Enter, () => presses++));

            _ = _input.Held.Add(Key.Enter);
            _keys.Update(1f / 60);

            Assert.Equal(0, presses);
        }

        [Fact]
        public void BackListenerFiresOncePerPress()
        {
            int presses = 0;
            _keys.AddBackKeyListener(() => presses++);

            _input.IsBackPressed = true;
            _keys.Update(1f / 60);
            _keys.Update(1f / 60);

            Assert.Equal(1, presses);
        }
    }
}
