using System.Collections.Generic;

namespace Mokus2D.Input
{
    /// <summary>Current keyboard, mouse, touch and back-button state, as reported by the platform host.</summary>
    public interface IInputSource
    {
        /// <summary>True while the platform back button (gamepad Back) is held.</summary>
        bool IsBackPressed { get; }

        MouseSnapshot GetMouse();

        void SetMousePosition(int x, int y);

        /// <summary>Clears <paramref name="into"/> and fills it with the keys currently held.</summary>
        void GetPressedKeys(List<Key> into);

        bool IsKeyDown(Key key);

        /// <summary>Clears <paramref name="into"/> and fills it with the touches currently down or moving.</summary>
        void GetTouches(List<TouchPoint> into);
    }
}
