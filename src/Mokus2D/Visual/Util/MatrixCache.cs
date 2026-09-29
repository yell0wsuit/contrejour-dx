using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Util
{
    public static class MatrixCache
    {
        private static readonly Dictionary<Vector2, Matrix4x4> Cache = [];

        public static Matrix4x4 GetScreenMatrix(Vector2 screenSize)
        {
            if (!Cache.TryGetValue(screenSize, out Matrix4x4 value))
            {
                value = CreateScreenMatrix(screenSize);
                Cache[screenSize] = value;
            }
            return value;
        }

        private static Matrix4x4 CreateScreenMatrix(Vector2 size)
        {
            return XnaMath.Multiply(XnaMath.Multiply(Matrix4x4.CreateTranslation((0f - size.X) / 2f, (0f - size.Y) / 2f, 0f), XnaMath.CreateRotationX(XnaMath.ToRadians(180f))), Matrix4x4.CreateScale(2f / size.X, 2f / size.Y, 0f));
        }
    }
}
