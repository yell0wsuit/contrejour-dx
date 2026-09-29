using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Util
{
    public static class MatrixCache
    {
        private static readonly Dictionary<Vector2, Matrix> Cache = [];

        public static Matrix GetScreenMatrix(Vector2 screenSize)
        {
            if (!Cache.TryGetValue(screenSize, out Matrix value))
            {
                value = CreateScreenMatrix(screenSize);
                Cache[screenSize] = value;
            }
            return value;
        }

        private static Matrix CreateScreenMatrix(Vector2 size)
        {
            return Matrix.CreateTranslation((0f - size.X) / 2f, (0f - size.Y) / 2f, 0f) * Matrix.CreateRotationX(MathHelper.ToRadians(180f)) * Matrix.CreateScale(2f / size.X, 2f / size.Y, 0f);
        }
    }
}
