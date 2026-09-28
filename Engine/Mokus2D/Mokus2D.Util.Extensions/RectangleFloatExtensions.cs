using Microsoft.Xna.Framework;
using Mokus2D.Data;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util.Extensions;

public static class RectangleFloatExtensions
{
	public static Vector2 ClampPoint(this RectangleFloat rectangle, Vector2 point)
	{
		return point.Clamp(rectangle.LeftTop, rectangle.RightBottom);
	}
}
