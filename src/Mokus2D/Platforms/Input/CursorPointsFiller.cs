using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.PlatformSupport.Input;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Platforms.Input
{
    public static class CursorPointsFiller
    {
        private static Vector2? _mousePosition;

        private static readonly List<TouchPoint> Touches = [];

        public static void FillPoints(List<CursorPoint> cursorPoints)
        {
            MouseSnapshot mouse = Mokus2DGame.Input.GetMouse();
            Vector2 mousePosition = AdjustMouseSpeed(mouse.Position);
            bool flag = false;
            Mokus2DGame.Input.GetTouches(Touches);
            foreach (TouchPoint item in Touches)
            {
                cursorPoints.Add(new CursorPoint(item.Position, item.Id, TouchType.Touch));
                if (item.Position == mouse.Position)
                {
                    flag = true;
                }
            }
            bool mouseButtonsSwapped = GetMouseButtonsSwapped();
            bool leftDown = mouseButtonsSwapped ? mouse.Right : mouse.Left;
            bool rightDown = mouseButtonsSwapped ? mouse.Left : mouse.Right;
            if (leftDown && !flag)
            {
                cursorPoints.Add(new CursorPoint(mousePosition, -1, TouchType.LeftMouseButton));
            }
            if (rightDown)
            {
                cursorPoints.Add(new CursorPoint(mousePosition, -2, TouchType.RightMouseButton));
            }
            if (mouse.Middle)
            {
                cursorPoints.Add(new CursorPoint(mousePosition, -3, TouchType.MiddleMouseButton));
            }
        }

        private static Vector2 AdjustMouseSpeed(Vector2 mousePosition)
        {
            if (Mokus2DGame.Config.MouseSpeed.FuzzyEquals(1f, 0.05f))
            {
                return mousePosition;
            }
            if (_mousePosition.HasValue && Mokus2DGame.Instance.IsFullScreen)
            {
                Vector2 vector = mousePosition - _mousePosition.Value;
                if (vector.Length() > 2f)
                {
                    vector *= Mokus2DGame.Config.MouseSpeed;
                    mousePosition = _mousePosition.Value + vector;
                    Mokus2DGame.Input.SetMousePosition((int)mousePosition.X, (int)mousePosition.Y);
                }
            }
            _mousePosition = mousePosition;
            return mousePosition;
        }

        private static bool GetMouseButtonsSwapped()
        {
            // Desktop MonoGame already reports the logical (post-swap) buttons.
            return false;
        }
    }
}
