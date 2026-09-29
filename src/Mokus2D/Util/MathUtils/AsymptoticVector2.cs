using Microsoft.Xna.Framework;

using Mokus2D.Data;

namespace Mokus2D.Util.MathUtils
{
    public class AsymptoticVector2(RectangleFloat bounds, float offset) : IValueProcessor<Vector2>
    {
        private RectangleFloat Bounds = bounds;

        private readonly float Offset = offset;

        public Vector2 GetValue(Vector2 value)
        {
            Vector2 result = default;
            result.X = AsymptoticFloat.GetValue(value.X, Bounds.Left, Bounds.Right, Offset);
            result.Y = AsymptoticFloat.GetValue(value.X, Bounds.Top, Bounds.Bottom, Offset);
            return result;
        }
    }
}
