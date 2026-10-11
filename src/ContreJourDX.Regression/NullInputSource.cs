using System.Collections.Generic;

using Mokus2D.Input;

namespace ContreJourDX.Regression
{
    // No keyboard, mouse, touch or back button: the harness plays with input disabled.
    internal sealed class NullInputSource : IInputSource
    {
        public bool IsBackPressed => false;

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
        }

        public bool IsKeyDown(Key key)
        {
            return false;
        }

        public void GetTouches(List<TouchPoint> into)
        {
            into.Clear();
        }
    }
}
