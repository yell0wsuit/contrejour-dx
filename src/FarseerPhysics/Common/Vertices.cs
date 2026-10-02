using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;

using FarseerPhysics.Collision;

namespace FarseerPhysics.Common
{
    [DebuggerDisplay("Count = {Count} Vertices = {ToString()}")]
    public class Vertices : List<Vector2>
    {
        internal bool AttachedToBody { get; set; }

        public List<Vertices> Holes { get; set; }

        public Vertices()
        {
        }

        public Vertices(int capacity)
            : base(capacity)
        {
        }

        public Vertices(IEnumerable<Vector2> vertices)
        {
            AddRange(vertices);
        }

        public int NextIndex(int index)
        {
            return index + 1 <= Count - 1 ? index + 1 : 0;
        }

        public Vector2 NextVertex(int index)
        {
            return base[NextIndex(index)];
        }

        public int PreviousIndex(int index)
        {
            return index - 1 >= 0 ? index - 1 : Count - 1;
        }

        public Vector2 PreviousVertex(int index)
        {
            return base[PreviousIndex(index)];
        }

        public float GetSignedArea()
        {
            if (Count < 3)
            {
                return 0f;
            }
            float num = 0f;
            for (int i = 0; i < Count; i++)
            {
                int index = (i + 1) % Count;
                Vector2 vector = base[i];
                Vector2 vector2 = base[index];
                num += vector.X * vector2.Y;
                num -= vector.Y * vector2.X;
            }
            return num / 2f;
        }

        public float GetArea()
        {
            float signedArea = GetSignedArea();
            return !(signedArea < 0f) ? signedArea : 0f - signedArea;
        }

        public Vector2 GetCentroid()
        {
            if (Count < 3)
            {
                return new Vector2(float.NaN, float.NaN);
            }
            Vector2 zero = Vector2.Zero;
            float num = 0f;
            for (int i = 0; i < Count; i++)
            {
                Vector2 vector = base[i];
                Vector2 vector2 = (i + 1 < Count) ? base[i + 1] : base[0];
                float num2 = 0.5f * ((vector.X * vector2.Y) - (vector.Y * vector2.X));
                num += num2;
                zero += num2 * (1f / 3f) * (vector + vector2);
            }
            return zero * (1f / num);
        }

        public AABB GetAABB()
        {
            Vector2 lowerBound = new(float.MaxValue, float.MaxValue);
            Vector2 upperBound = new(float.MinValue, float.MinValue);
            for (int i = 0; i < Count; i++)
            {
                if (base[i].X < lowerBound.X)
                {
                    lowerBound.X = base[i].X;
                }
                if (base[i].X > upperBound.X)
                {
                    upperBound.X = base[i].X;
                }
                if (base[i].Y < lowerBound.Y)
                {
                    lowerBound.Y = base[i].Y;
                }
                if (base[i].Y > upperBound.Y)
                {
                    upperBound.Y = base[i].Y;
                }
            }
            AABB result = default;
            result.LowerBound = lowerBound;
            result.UpperBound = upperBound;
            return result;
        }

        public void Translate(Vector2 value)
        {
            Translate(ref value);
        }

        public void Translate(ref Vector2 value)
        {
            for (int i = 0; i < Count; i++)
            {
                base[i] = Vector2.Add(base[i], value);
            }
            if (Holes == null || Holes.Count <= 0)
            {
                return;
            }
            foreach (Vertices hole in Holes)
            {
                hole.Translate(ref value);
            }
        }

        public void Scale(Vector2 value)
        {
            Scale(ref value);
        }

        public void Scale(ref Vector2 value)
        {
            for (int i = 0; i < Count; i++)
            {
                base[i] = Vector2.Multiply(base[i], value);
            }
            if (Holes == null || Holes.Count <= 0)
            {
                return;
            }
            foreach (Vertices hole in Holes)
            {
                hole.Scale(ref value);
            }
        }

        public void Rotate(float value)
        {
            float num = (float)Math.Cos(value);
            float num2 = (float)Math.Sin(value);
            for (int i = 0; i < Count; i++)
            {
                Vector2 vector = base[i];
                base[i] = new Vector2((vector.X * num) + (vector.Y * (0f - num2)), (vector.X * num2) + (vector.Y * num));
            }
            if (Holes == null || Holes.Count <= 0)
            {
                return;
            }
            foreach (Vertices hole in Holes)
            {
                hole.Rotate(value);
            }
        }

        public bool IsConvex()
        {
            if (Count < 3)
            {
                return false;
            }
            if (Count == 3)
            {
                return true;
            }
            for (int i = 0; i < Count; i++)
            {
                int num = (i + 1 < Count) ? (i + 1) : 0;
                Vector2 vector = base[num] - base[i];
                for (int j = 0; j < Count; j++)
                {
                    if (j != i && j != num)
                    {
                        Vector2 vector2 = base[j] - base[i];
                        float num2 = (vector.X * vector2.Y) - (vector.Y * vector2.X);
                        if (num2 <= 0f)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool IsCounterClockWise()
        {
            return Count >= 3 && GetSignedArea() > 0f;
        }

        public void ForceCounterClockWise()
        {
            if (Count >= 3 && !IsCounterClockWise())
            {
                Reverse();
            }
        }

        public bool IsSimple()
        {
            if (Count < 3)
            {
                return false;
            }
            for (int i = 0; i < Count; i++)
            {
                Vector2 a = base[i];
                Vector2 a2 = NextVertex(i);
                for (int j = i + 1; j < Count; j++)
                {
                    Vector2 b = base[j];
                    Vector2 b2 = NextVertex(j);
                    if (LineTools.LineIntersect2(ref a, ref a2, ref b, ref b2, out Vector2 _))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public PolygonError CheckPolygon()
        {
            if (Count < 3 || Count > Settings.MaxPolygonVertices)
            {
                return PolygonError.InvalidAmountOfVertices;
            }
            if (!IsSimple())
            {
                return PolygonError.NotSimple;
            }
            if (GetArea() <= 1.1920929E-07f)
            {
                return PolygonError.AreaTooSmall;
            }
            if (!IsConvex())
            {
                return PolygonError.NotConvex;
            }
            for (int i = 0; i < Count; i++)
            {
                int index = (i + 1 < Count) ? (i + 1) : 0;
                if ((base[index] - base[i]).LengthSquared() <= 1.4210855E-14f)
                {
                    return PolygonError.SideTooSmall;
                }
            }
            return !IsCounterClockWise() ? PolygonError.NotCounterClockWise : PolygonError.NoError;
        }

        public void ProjectToAxis(ref Vector2 axis, out float min, out float max)
        {
            max = min = Vector2.Dot(axis, base[0]);
            for (int i = 0; i < Count; i++)
            {
                float num = Vector2.Dot(base[i], axis);
                if (num < min)
                {
                    min = num;
                }
                else if (num > max)
                {
                    max = num;
                }
            }
        }

        public int PointInPolygon(ref Vector2 point)
        {
            int num = 0;
            for (int i = 0; i < Count; i++)
            {
                Vector2 a = base[i];
                Vector2 b = base[NextIndex(i)];
                Vector2 value = b - a;
                float num2 = MathUtils.Area(ref a, ref b, ref point);
                if (num2 == 0f && Vector2.Dot(point - a, value) >= 0f && Vector2.Dot(point - b, value) <= 0f)
                {
                    return 0;
                }
                if (a.Y <= point.Y)
                {
                    if (b.Y > point.Y && num2 > 0f)
                    {
                        num++;
                    }
                }
                else if (b.Y <= point.Y && num2 < 0f)
                {
                    num--;
                }
            }
            return num != 0 ? 1 : -1;
        }

        public bool PointInPolygonAngle(ref Vector2 point)
        {
            double num = 0.0;
            for (int i = 0; i < Count; i++)
            {
                Vector2 p = base[i] - point;
                Vector2 p2 = base[NextIndex(i)] - point;
                num += MathUtils.VectorAngle(ref p, ref p2);
            }
            return Math.Abs(num) >= Math.PI;
        }

        public void Transform(ref Matrix4x4 transform)
        {
            for (int i = 0; i < Count; i++)
            {
                base[i] = Vector2.Transform(base[i], transform);
            }
            if (Holes != null && Holes.Count > 0)
            {
                for (int j = 0; j < Holes.Count; j++)
                {
                    Vector2[] array = [.. Holes[j]];
                    for (int k = 0; k < array.Length; k++)
                    {
                        array[k] = Vector2.Transform(array[k], transform);
                    }
                    Holes[j] = [.. array];
                }
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < Count; i++)
            {
                _ = stringBuilder.Append(base[i].ToString());
                if (i < Count - 1)
                {
                    _ = stringBuilder.Append(' ');
                }
            }
            return stringBuilder.ToString();
        }
    }
}
