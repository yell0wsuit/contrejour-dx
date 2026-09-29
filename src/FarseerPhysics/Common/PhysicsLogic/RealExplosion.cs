using System;
using System.Collections.Generic;
using System.Linq;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PhysicsLogic
{
    public sealed class RealExplosion(World world) : PhysicsLogic(world, PhysicsLogicType.Explosion)
    {
        private readonly float EdgeRatio = 0.025f;

        public bool IgnoreWhenInsideShape { get; set; }

        private readonly float MaxAngle = (float)Math.PI / 15f;

        private readonly int MaxShapes = 100;

        private readonly int MinRays = 5;

        private readonly List<ShapeData> _data = [];

        private readonly RayDataComparer _rdc = new();

        public Dictionary<Fixture, Vector2> Activate(Vector2 pos, float radius, float maxForce)
        {
            AABB aabb = default;
            aabb.LowerBound = pos + new Vector2(0f - radius, 0f - radius);
            aabb.UpperBound = pos + new Vector2(radius, radius);
            Fixture[] shapes = new Fixture[MaxShapes];
            Fixture[] containedShapes = new Fixture[5];
            bool exit = false;
            int shapeCount = 0;
            int containedShapeCount = 0;
            World.QueryAABB(delegate (Fixture fixture4)
            {
                if (fixture4.TestPoint(ref pos))
                {
                    if (IgnoreWhenInsideShape)
                    {
                        exit = true;
                        return false;
                    }
                    containedShapes[containedShapeCount++] = fixture4;
                }
                else
                {
                    shapes[shapeCount++] = fixture4;
                }
                return true;
            }, ref aabb);
            if (exit)
            {
                return [];
            }
            Dictionary<Fixture, Vector2> dictionary = new(shapeCount + containedShapeCount);
            float[] array = new float[shapeCount * 2];
            int num = 0;
            for (int num2 = 0; num2 < shapeCount; num2++)
            {
                PolygonShape polygonShape;
                if (shapes[num2].Shape is CircleShape circleShape)
                {
                    Vertices vertices = [];
                    Vector2 item = Vector2.Zero + new Vector2(circleShape.Radius, 0f);
                    vertices.Add(item);
                    item = Vector2.Zero + new Vector2(0f, circleShape.Radius);
                    vertices.Add(item);
                    item = Vector2.Zero + new Vector2(0f - circleShape.Radius, circleShape.Radius);
                    vertices.Add(item);
                    item = Vector2.Zero + new Vector2(0f, 0f - circleShape.Radius);
                    vertices.Add(item);
                    polygonShape = new PolygonShape(vertices, 0f);
                }
                else
                {
                    polygonShape = shapes[num2].Shape as PolygonShape;
                }
                if (shapes[num2].Body.BodyType != BodyType.Dynamic || polygonShape == null)
                {
                    continue;
                }
                Vector2 vector = shapes[num2].Body.GetWorldPoint(polygonShape.MassData.Centroid) - pos;
                float num3 = (float)Math.Atan2(vector.Y, vector.X);
                float num4 = float.MaxValue;
                float num5 = float.MinValue;
                float num6 = 0f;
                float num7 = 0f;
                for (int num8 = 0; num8 < polygonShape.Vertices.Count; num8++)
                {
                    Vector2 vector2 = shapes[num2].Body.GetWorldPoint(polygonShape.Vertices[num8]) - pos;
                    float num9 = (float)Math.Atan2(vector2.Y, vector2.X);
                    float num10 = num9 - num3;
                    num10 = (num10 - (float)Math.PI) % ((float)Math.PI * 2f);
                    if (num10 < 0f)
                    {
                        num10 += (float)Math.PI * 2f;
                    }
                    num10 -= (float)Math.PI;
                    if (!(Math.Abs(num10) > (float)Math.PI))
                    {
                        if (num10 > num5)
                        {
                            num5 = num10;
                            num7 = num9;
                        }
                        if (num10 < num4)
                        {
                            num4 = num10;
                            num6 = num9;
                        }
                    }
                }
                array[num] = num6;
                num++;
                array[num] = num7;
                num++;
            }
            Array.Sort(array, 0, num, _rdc);
            _data.Clear();
            bool flag = true;
            ShapeData item2 = default;
            for (int num11 = 0; num11 < num; num11++)
            {
                Fixture fixture = null;
                int num12 = (num11 != num - 1) ? (num11 + 1) : 0;
                if (array[num11] == array[num12])
                {
                    continue;
                }
                float num13 = (num11 != num - 1) ? (array[num11 + 1] + array[num11]) : (array[0] + ((float)Math.PI * 2f) + array[num11]);
                num13 /= 2f;
                Vector2 point = pos;
                Vector2 point2 = (radius * new Vector2((float)Math.Cos(num13), (float)Math.Sin(num13))) + pos;
                bool hitClosest = false;
                World.RayCast(delegate (Fixture f, Vector2 p, Vector2 n, float fr)
                {
                    Body body = f.Body;
                    if (!IsActiveOn(body))
                    {
                        return 0f;
                    }
                    hitClosest = true;
                    fixture = f;
                    return fr;
                }, point, point2);
                if (hitClosest && fixture.Body.BodyType == BodyType.Dynamic)
                {
                    if (_data.Count > 0 && _data[^1].Body == fixture.Body && !flag)
                    {
                        int index = _data.Count - 1;
                        ShapeData value = _data[index];
                        value.Max = array[num12];
                        _data[index] = value;
                    }
                    else
                    {
                        item2.Body = fixture.Body;
                        item2.Min = array[num11];
                        item2.Max = array[num12];
                        _data.Add(item2);
                    }
                    if (_data.Count > 1 && num11 == num - 1 && _data.Last().Body == _data.First().Body && _data.Last().Max == _data.First().Min)
                    {
                        ShapeData value2 = _data[0];
                        value2.Min = _data.Last().Min;
                        _data.RemoveAt(_data.Count - 1);
                        _data[0] = value2;
                        while (_data.First().Min >= _data.First().Max)
                        {
                            value2.Min -= (float)Math.PI * 2f;
                            _data[0] = value2;
                        }
                    }
                    int index2 = _data.Count - 1;
                    ShapeData value3 = _data[index2];
                    while (_data.Count > 0 && _data.Last().Min >= _data.Last().Max)
                    {
                        value3.Min = _data.Last().Min - ((float)Math.PI * 2f);
                        _data[index2] = value3;
                    }
                    flag = false;
                }
                else
                {
                    flag = true;
                }
            }
            RayCastInput input = default;
            for (int num14 = 0; num14 < _data.Count; num14++)
            {
                if (!IsActiveOn(_data[num14].Body))
                {
                    continue;
                }
                float num15 = _data[num14].Max - _data[num14].Min;
                float num16 = MathHelper.Min((float)Math.PI / 90f, EdgeRatio * num15);
                int num17 = (int)Math.Ceiling((num15 - (2f * num16) - ((MinRays - 1) * MaxAngle)) / MaxAngle);
                if (num17 < 0)
                {
                    num17 = 0;
                }
                float num18 = (num15 - (num16 * 2f)) / (MinRays + (float)num17 - 1f);
                for (float num19 = _data[num14].Min + num16; num19 < _data[num14].Max || MathUtils.FloatEquals(num19, _data[num14].Max, 0.0001f); num19 += num18)
                {
                    Vector2 vector3 = pos;
                    Vector2 vector4 = pos + (radius * new Vector2((float)Math.Cos(num19), (float)Math.Sin(num19)));
                    Vector2 point3 = Vector2.Zero;
                    float num20 = float.MaxValue;
                    List<Fixture> fixtureList = _data[num14].Body.FixtureList;
                    for (int num21 = 0; num21 < fixtureList.Count; num21++)
                    {
                        Fixture fixture2 = fixtureList[num21];
                        input.Point1 = vector3;
                        input.Point2 = vector4;
                        input.MaxFraction = 50f;
                        if (fixture2.RayCast(out RayCastOutput output, ref input, 0) && num20 > output.Fraction)
                        {
                            num20 = output.Fraction;
                            point3 = (output.Fraction * vector4) + ((1f - output.Fraction) * vector3);
                        }
                        float num22 = num15 / (MinRays + num17) * maxForce * 180f / (float)Math.PI * (1f - Math.Min(1f, num20));
                        Vector2 impulse = Vector2.Dot(num22 * new Vector2((float)Math.Cos(num19), (float)Math.Sin(num19)), -output.Normal) * new Vector2((float)Math.Cos(num19), (float)Math.Sin(num19));
                        _data[num14].Body.ApplyLinearImpulse(ref impulse, ref point3);
                        if (!dictionary.TryAdd(fixture2, impulse))
                        {
                            dictionary[fixture2] += impulse;
                        }
                        if (num20 > 1f)
                        {
                            point3 = vector4;
                        }
                    }
                }
            }
            for (int num23 = 0; num23 < containedShapeCount; num23++)
            {
                Fixture fixture3 = containedShapes[num23];
                if (IsActiveOn(fixture3.Body))
                {
                    float num24 = MinRays * maxForce * 180f / (float)Math.PI;
                    Vector2 point4;
                    if (fixture3.Shape is CircleShape circleShape2)
                    {
                        point4 = fixture3.Body.GetWorldPoint(circleShape2.Position);
                    }
                    else
                    {
                        PolygonShape polygonShape2 = fixture3.Shape as PolygonShape;
                        point4 = fixture3.Body.GetWorldPoint(polygonShape2.MassData.Centroid);
                    }
                    Vector2 impulse2 = num24 * (point4 - pos);
                    fixture3.Body.ApplyLinearImpulse(ref impulse2, ref point4);
                    _ = dictionary.TryAdd(fixture3, impulse2);
                }
            }
            return dictionary;
        }
    }
}
