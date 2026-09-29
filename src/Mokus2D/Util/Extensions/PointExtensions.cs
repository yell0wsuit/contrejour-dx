using Microsoft.Xna.Framework;

namespace Mokus2D.Util.Extensions
{
    public static class PointExtensions
    {
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }

        public static bool Between(this Point point, Point leftTop, Point rightBottom)
        {
            return point.X >= leftTop.X && point.Y >= leftTop.Y && point.X <= rightBottom.X && point.Y <= rightBottom.Y;
        }
    }
}
