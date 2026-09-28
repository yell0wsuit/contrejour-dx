using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.UI.Layout;

public static class LayoutOrientationExtensions
{
    public static float GetValue(this Vector2 value, LayoutOrientation orientation)
    {
        return orientation != LayoutOrientation.Horizontal ? value.Y : value.X;
    }

    public static Vector2 StepTo(this Vector2 value, float target, float step, LayoutOrientation orientation)
    {
        float value2 = value.GetValue(orientation);
        value2 = value2.StepTo(target, step);
        return value.Change(value2, orientation);
    }

    public static Vector2 Change(this Vector2 vector, float value, LayoutOrientation orientation)
    {
        return orientation != LayoutOrientation.Horizontal ? vector.ChangeY(value) : vector.ChangeX(value);
    }

    public static Vector2 Change(this Vector2 vector, Vector2 value, LayoutOrientation orientation)
    {
        return orientation != LayoutOrientation.Horizontal ? vector.ChangeY(value.Y) : vector.ChangeX(value.X);
    }
}
