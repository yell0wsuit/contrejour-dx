using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mokus2D.Util;

public static class XNAExtensions
{
    public static bool IsLandscape(this DisplayOrientation orientation)
    {
        return orientation == DisplayOrientation.LandscapeLeft || orientation == DisplayOrientation.LandscapeRight;
    }

    public static Vector2 Position(this MouseState state)
    {
        return new Vector2(state.X, state.Y);
    }
}
