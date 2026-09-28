using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mokus2D.Util;

public static class XNAExtensions
{
	public static bool IsLandscape(this DisplayOrientation orientation)
	{
		if (orientation != DisplayOrientation.LandscapeLeft)
		{
			return orientation == DisplayOrientation.LandscapeRight;
		}
		return true;
	}

	public static Vector2 Position(this MouseState state)
	{
		return new Vector2(state.X, state.Y);
	}
}
