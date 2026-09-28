using Microsoft.Xna.Framework;
using Mokus2D.Data;

namespace Mokus2D.Util.MathUtils;

public class AsymptoticVector2 : IValueProcessor<Vector2>
{
	public RectangleFloat Bounds;

	public float Offset;

	public AsymptoticVector2(RectangleFloat bounds, float offset)
	{
		Bounds = bounds;
		Offset = offset;
	}

	public Vector2 GetValue(Vector2 value)
	{
		Vector2 result = default(Vector2);
		result.X = AsymptoticFloat.GetValue(value.X, Bounds.Left, Bounds.Right, Offset);
		result.Y = AsymptoticFloat.GetValue(value.X, Bounds.Top, Bounds.Bottom, Offset);
		return result;
	}
}
