using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FarseerPhysics.Common
{
    public class Path
    {
        private readonly List<Vector2> ControlPoints;

        private float _deltaT;

        public bool Closed { get; set; }

        public Path()
        {
            ControlPoints = [];
        }

        public Path(Vector2[] vertices)
        {
            ControlPoints = new List<Vector2>(vertices.Length);
            for (int i = 0; i < vertices.Length; i++)
            {
                Add(vertices[i]);
            }
        }

        public Path(IList<Vector2> vertices)
        {
            ControlPoints = new List<Vector2>(vertices.Count);
            for (int i = 0; i < vertices.Count; i++)
            {
                Add(vertices[i]);
            }
        }

        public int NextIndex(int index)
        {
            return index == ControlPoints.Count - 1 ? 0 : index + 1;
        }

        public int PreviousIndex(int index)
        {
            return index == 0 ? ControlPoints.Count - 1 : index - 1;
        }

        public void Translate(ref Vector2 vector)
        {
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                ControlPoints[i] = Vector2.Add(ControlPoints[i], vector);
            }
        }

        public void Scale(ref Vector2 value)
        {
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                ControlPoints[i] = Vector2.Multiply(ControlPoints[i], value);
            }
        }

        public void Rotate(float value)
        {
            Matrix4x4 result = Matrix4x4.CreateRotationZ(value);
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                ControlPoints[i] = Vector2.Transform(ControlPoints[i], result);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            for (int i = 0; i < ControlPoints.Count; i++)
            {
                _ = stringBuilder.Append(ControlPoints[i].ToString());
                if (i < ControlPoints.Count - 1)
                {
                    _ = stringBuilder.Append(' ');
                }
            }
            return stringBuilder.ToString();
        }

        public Vertices GetVertices(int divisions)
        {
            Vertices vertices = [];
            float num = 1f / divisions;
            for (float num2 = 0f; num2 < 1f; num2 += num)
            {
                vertices.Add(GetPosition(num2));
            }
            return vertices;
        }

        public Vector2 GetPosition(float time)
        {
            if (ControlPoints.Count < 2)
            {
                throw new InvalidOperationException("You need at least 2 control points to calculate a position.");
            }
            Vector2 result;
            if (Closed)
            {
                Add(ControlPoints[0]);
                _deltaT = 1f / (ControlPoints.Count - 1);
                int num = (int)(time / _deltaT);
                int num2 = num - 1;
                if (num2 < 0)
                {
                    num2 += ControlPoints.Count - 1;
                }
                else if (num2 >= ControlPoints.Count - 1)
                {
                    num2 -= ControlPoints.Count - 1;
                }
                int num3 = num;
                if (num3 < 0)
                {
                    num3 += ControlPoints.Count - 1;
                }
                else if (num3 >= ControlPoints.Count - 1)
                {
                    num3 -= ControlPoints.Count - 1;
                }
                int num4 = num + 1;
                if (num4 < 0)
                {
                    num4 += ControlPoints.Count - 1;
                }
                else if (num4 >= ControlPoints.Count - 1)
                {
                    num4 -= ControlPoints.Count - 1;
                }
                int num5 = num + 2;
                if (num5 < 0)
                {
                    num5 += ControlPoints.Count - 1;
                }
                else if (num5 >= ControlPoints.Count - 1)
                {
                    num5 -= ControlPoints.Count - 1;
                }
                float amount = (time - (_deltaT * num)) / _deltaT;
                result = CatmullRom.Interpolate(ControlPoints[num2], ControlPoints[num3], ControlPoints[num4], ControlPoints[num5], amount);
                RemoveAt(ControlPoints.Count - 1);
            }
            else
            {
                int num6 = (int)(time / _deltaT);
                int num7 = num6 - 1;
                if (num7 < 0)
                {
                    num7 = 0;
                }
                else if (num7 >= ControlPoints.Count - 1)
                {
                    num7 = ControlPoints.Count - 1;
                }
                int num8 = num6;
                if (num8 < 0)
                {
                    num8 = 0;
                }
                else if (num8 >= ControlPoints.Count - 1)
                {
                    num8 = ControlPoints.Count - 1;
                }
                int num9 = num6 + 1;
                if (num9 < 0)
                {
                    num9 = 0;
                }
                else if (num9 >= ControlPoints.Count - 1)
                {
                    num9 = ControlPoints.Count - 1;
                }
                int num10 = num6 + 2;
                if (num10 < 0)
                {
                    num10 = 0;
                }
                else if (num10 >= ControlPoints.Count - 1)
                {
                    num10 = ControlPoints.Count - 1;
                }
                float amount2 = (time - (_deltaT * num6)) / _deltaT;
                result = CatmullRom.Interpolate(ControlPoints[num7], ControlPoints[num8], ControlPoints[num9], ControlPoints[num10], amount2);
            }
            return result;
        }

        public Vector2 GetPositionNormal(float time)
        {
            float time2 = time + 0.0001f;
            Vector2 value = GetPosition(time);
            Vector2 value2 = GetPosition(time2);
            Vector2 result = value - value2;
            Vector2 value3 = default;
            value3.X = 0f - result.Y;
            value3.Y = result.X;
            value3 = Vector2.Normalize(value3);
            return value3;
        }

        public void Add(Vector2 point)
        {
            ControlPoints.Add(point);
            _deltaT = 1f / (ControlPoints.Count - 1);
        }

        public void Remove(Vector2 point)
        {
            _ = ControlPoints.Remove(point);
            _deltaT = 1f / (ControlPoints.Count - 1);
        }

        public void RemoveAt(int index)
        {
            ControlPoints.RemoveAt(index);
            _deltaT = 1f / (ControlPoints.Count - 1);
        }

        public float GetLength()
        {
            List<Vector2> vertices = GetVertices(ControlPoints.Count * 25);
            float num = 0f;
            for (int i = 1; i < vertices.Count; i++)
            {
                num += Vector2.Distance(vertices[i - 1], vertices[i]);
            }
            if (Closed)
            {
                num += Vector2.Distance(vertices[ControlPoints.Count - 1], vertices[0]);
            }
            return num;
        }

        public List<Vector3> SubdivideEvenly(int divisions)
        {
            List<Vector3> list = [];
            float length = GetLength();
            float num = (length / divisions) + 0.001f;
            float num2 = 0f;
            Vector2 value = ControlPoints[0];
            Vector2 position = GetPosition(num2);
            while (num * 0.5f >= Vector2.Distance(value, position))
            {
                position = GetPosition(num2);
                num2 += 0.0001f;
                if (num2 >= 1f)
                {
                    break;
                }
            }
            value = position;
            for (int i = 1; i < divisions; i++)
            {
                Vector2 positionNormal = GetPositionNormal(num2);
                float z = (float)Math.Atan2(positionNormal.Y, positionNormal.X);
                list.Add(new Vector3(position, z));
                while (num >= Vector2.Distance(value, position))
                {
                    position = GetPosition(num2);
                    num2 += 1E-05f;
                    if (num2 >= 1f)
                    {
                        break;
                    }
                }
                if (num2 >= 1f)
                {
                    break;
                }
                value = position;
            }
            return list;
        }
    }
}
