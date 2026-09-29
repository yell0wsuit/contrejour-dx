using System;
using System.Numerics;

namespace Mokus2D.Visual.Data
{
    public static class MatrixUtil
    {
        public static Matrix4x4 CalculateTransformMatrix(Vector2 position, float rotationRadians, Vector2 scaleVec)
        {
            return Matrix4x4.CreateScale(scaleVec.X, scaleVec.Y, 1f) * Matrix4x4.CreateRotationZ(rotationRadians) * Matrix4x4.CreateTranslation(position.X, position.Y, 0f);
        }

        public static Matrix4x4 Decompose2D(this Matrix4x4 matrix, out Vector3 scale, out Vector3 translation)
        {
            Matrix4x4 result = matrix;
            translation.X = result.M41;
            translation.Y = result.M42;
            translation.Z = result.M43;
            scale.X = (float)Math.Sqrt((result.M11 * result.M11) + (result.M12 * result.M12) + (result.M13 * result.M13));
            scale.Y = (float)Math.Sqrt((result.M21 * result.M21) + (result.M22 * result.M22) + (result.M23 * result.M23));
            scale.Z = (float)Math.Sqrt((result.M31 * result.M31) + (result.M32 * result.M32) + (result.M33 * result.M33));
            _ = new Matrix4x4(result.M11 / scale.X, result.M12 / scale.X, result.M13 / scale.X, 0f, result.M21 / scale.Y, result.M22 / scale.Y, result.M23 / scale.Y, 0f, result.M31 / scale.Z, result.M32 / scale.Z, result.M33 / scale.Z, 0f, 0f, 0f, 0f, 1f);
            float determinant = GetDeterminant(result);
            if (determinant < 0f)
            {
                scale.Y *= -1f;
            }
            return result;
        }

        private static float GetDeterminant(Matrix4x4 result)
        {
            return (result.M11 * ((result.M22 * result.M33) - (result.M23 * result.M32))) - (result.M12 * ((result.M21 * result.M33) - (result.M23 * result.M31))) + (result.M13 * ((result.M21 * result.M32) - (result.M22 * result.M31)));
        }
    }
}
