using Microsoft.Xna.Framework;

namespace Mokus2D.Util.Extensions
{
    public static class PointExtensions
    {

        public static bool Between(this Point point, Point leftTop, Point rightBottom)
        {
            return point.X >= leftTop.X && point.Y >= leftTop.Y && point.X <= rightBottom.X && point.Y <= rightBottom.Y;
        }
    }
}
