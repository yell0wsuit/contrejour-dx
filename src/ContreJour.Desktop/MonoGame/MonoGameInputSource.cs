using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

using Mokus2D.Input;

namespace ContreJour.Desktop.MonoGame
{
    internal sealed class MonoGameInputSource : IInputSource
    {
        public bool IsBackPressed => GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;

        public MouseSnapshot GetMouse()
        {
            MouseState state = Mouse.GetState();
            return new MouseSnapshot(
                new System.Numerics.Vector2(state.X, state.Y),
                state.LeftButton == ButtonState.Pressed,
                state.MiddleButton == ButtonState.Pressed,
                state.RightButton == ButtonState.Pressed,
                state.ScrollWheelValue);
        }

        public void SetMousePosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
        }

        public void GetPressedKeys(List<Key> into)
        {
            into.Clear();
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                into.Add((Key)key);
            }
        }

        public bool IsKeyDown(Key key)
        {
            return Keyboard.GetState().IsKeyDown((Keys)key);
        }

        public void GetTouches(List<TouchPoint> into)
        {
            into.Clear();
            foreach (TouchLocation touch in TouchPanel.GetState())
            {
                if (touch.State is TouchLocationState.Pressed or TouchLocationState.Moved)
                {
                    into.Add(new TouchPoint(touch.Id, touch.Position.ToNumerics()));
                }
            }
        }
    }
}
