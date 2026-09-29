using System;
using System.Numerics;

using Mokus2D.Util.Data;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Util.Extensions
{
    public static class VectorExtensions
    {

        public static Vector2 ChangeY(this Vector2 source, float y)
        {
            return new Vector2(source.X, y);
        }

        public static Vector2 AddY(this Vector2 source, float value)
        {
            return new Vector2(source.X, source.Y + value);
        }

        public static Vector2 ChangeX(this Vector2 source, float x)
        {
            return new Vector2(x, source.Y);
        }

        public static Vector2 Size(this Rectangle value)
        {
            return new Vector2(value.Width, value.Height);
        }

        public static Vector2 LeftTop(this Rectangle value)
        {
            return new Vector2(value.X, value.Y);
        }

        public static Vector2 LeftBottom(this Rectangle value)
        {
            return new Vector2(value.X, value.Y + value.Height);
        }

        public static Vector2 RightBottom(this Rectangle value)
        {
            return value.LeftTop() + value.Size();
        }

        public static Vector2 RightTop(this Rectangle value)
        {
            return new Vector2(value.X + value.Width, value.Y);
        }

        public static Rectangle ToRectangle(this Vector2 leftTop, Vector2 size)
        {
            return new Rectangle((int)leftTop.X, (int)leftTop.Y, (int)size.X, (int)size.Y);
        }

        public static float Min(this Vector2 vector)
        {
            return Math.Min(vector.X, vector.Y);
        }

        public static Vector2 Abs(this Vector2 vector)
        {
            return new Vector2(vector.X.Abs(), vector.Y.Abs());
        }

        public static float Atan2(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.X, vector.Y);
        }

        public static Vector2 ToIntVector(this Vector2 vector)
        {
            return new Vector2((int)vector.X, (int)vector.Y);
        }

        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            return VectorUtil.Rotate(vector, angle);
        }

        public static Vector2 Rotate90(this Vector2 vector)
        {
            Vector2 point = vector;
            VectorUtil.Rotate90(ref point);
            return point;
        }

        public static Vector2 Signs(this Vector2 source)
        {
            return new Vector2(source.X.Sign(), source.Y.Sign());
        }

        public static Vector2 StepTo(this Vector2 source, Vector2 target, Vector2 step)
        {
            return new Vector2(source.X.StepTo(target.X, step.X), source.Y.StepTo(target.Y, step.Y));
        }

        public static Vector2 Normalize(this Vector2 source, float length)
        {
            source = XnaMath.Normalize(source);
            return source * length;
        }

        public static Vector2 StepTo(this Vector2 source, Vector2 target, float step)
        {
            Vector2 vector = target - source;
            if (vector.Length() <= step)
            {
                return target;
            }
            vector *= step / vector.Length();
            source += vector;
            return source;
        }

        public static Vector2 Middle(this Vector2 source, Vector2 target)
        {
            return VectorUtil.Center(source, target);
        }
    }
}
