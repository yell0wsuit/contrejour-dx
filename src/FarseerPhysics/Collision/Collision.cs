using System;
using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace FarseerPhysics.Collision
{
    public static class Collision
    {
        private sealed class EPCollider
        {
            private readonly TempPolygon _polygonB = new();

            private Transform _xf;

            private Vector2 _centroidB;

            private Vector2 _v0;

            private Vector2 _v1;

            private Vector2 _v2;

            private Vector2 _v3;

            private Vector2 _normal0;

            private Vector2 _normal1;

            private Vector2 _normal2;

            private Vector2 _normal;

            private Vector2 _lowerLimit;

            private Vector2 _upperLimit;

            private float _radius;

            private bool _front;

            public void Collide(ref Manifold manifold, EdgeShape edgeA, ref Transform xfA, PolygonShape polygonB, ref Transform xfB)
            {
                _xf = MathUtils.MulT(xfA, xfB);
                _centroidB = MathUtils.Mul(ref _xf, polygonB.MassData.Centroid);
                _v0 = edgeA.Vertex0;
                _v1 = edgeA._vertex1;
                _v2 = edgeA._vertex2;
                _v3 = edgeA.Vertex3;
                bool hasVertex = edgeA.HasVertex0;
                bool hasVertex2 = edgeA.HasVertex3;
                Vector2 vector = _v2 - _v1;
                vector = Vector2.Normalize(vector);
                _normal1 = new Vector2(vector.Y, 0f - vector.X);
                float num = Vector2.Dot(_normal1, _centroidB - _v1);
                float num2 = 0f;
                float num3 = 0f;
                bool flag = false;
                bool flag2 = false;
                if (hasVertex)
                {
                    Vector2 a = _v1 - _v0;
                    a = Vector2.Normalize(a);
                    _normal0 = new Vector2(a.Y, 0f - a.X);
                    flag = MathUtils.Cross(a, vector) >= 0f;
                    num2 = Vector2.Dot(_normal0, _centroidB - _v0);
                }
                if (hasVertex2)
                {
                    Vector2 b = _v3 - _v2;
                    b = Vector2.Normalize(b);
                    _normal2 = new Vector2(b.Y, 0f - b.X);
                    flag2 = MathUtils.Cross(vector, b) > 0f;
                    num3 = Vector2.Dot(_normal2, _centroidB - _v2);
                }
                if (hasVertex && hasVertex2)
                {
                    if (flag && flag2)
                    {
                        _front = num2 >= 0f || num >= 0f || num3 >= 0f;
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = _normal0;
                            _upperLimit = _normal2;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = -_normal1;
                            _upperLimit = -_normal1;
                        }
                    }
                    else if (flag)
                    {
                        _front = num2 >= 0f || (num >= 0f && num3 >= 0f);
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = _normal0;
                            _upperLimit = _normal1;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = -_normal2;
                            _upperLimit = -_normal1;
                        }
                    }
                    else if (flag2)
                    {
                        _front = num3 >= 0f || (num2 >= 0f && num >= 0f);
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = _normal1;
                            _upperLimit = _normal2;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = -_normal1;
                            _upperLimit = -_normal0;
                        }
                    }
                    else
                    {
                        _front = num2 >= 0f && num >= 0f && num3 >= 0f;
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = _normal1;
                            _upperLimit = _normal1;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = -_normal2;
                            _upperLimit = -_normal0;
                        }
                    }
                }
                else if (hasVertex)
                {
                    if (flag)
                    {
                        _front = num2 >= 0f || num >= 0f;
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = _normal0;
                            _upperLimit = -_normal1;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = _normal1;
                            _upperLimit = -_normal1;
                        }
                    }
                    else
                    {
                        _front = num2 >= 0f && num >= 0f;
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = _normal1;
                            _upperLimit = -_normal1;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = _normal1;
                            _upperLimit = -_normal0;
                        }
                    }
                }
                else if (hasVertex2)
                {
                    if (flag2)
                    {
                        _front = num >= 0f || num3 >= 0f;
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = -_normal1;
                            _upperLimit = _normal2;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = -_normal1;
                            _upperLimit = _normal1;
                        }
                    }
                    else
                    {
                        _front = num >= 0f && num3 >= 0f;
                        if (_front)
                        {
                            _normal = _normal1;
                            _lowerLimit = -_normal1;
                            _upperLimit = _normal1;
                        }
                        else
                        {
                            _normal = -_normal1;
                            _lowerLimit = -_normal2;
                            _upperLimit = _normal1;
                        }
                    }
                }
                else
                {
                    _front = num >= 0f;
                    if (_front)
                    {
                        _normal = _normal1;
                        _lowerLimit = -_normal1;
                        _upperLimit = -_normal1;
                    }
                    else
                    {
                        _normal = -_normal1;
                        _lowerLimit = _normal1;
                        _upperLimit = _normal1;
                    }
                }
                _polygonB.Count = polygonB.Vertices.Count;
                for (int i = 0; i < polygonB.Vertices.Count; i++)
                {
                    ref Vector2 reference = ref _polygonB.Vertices[i];
                    reference = MathUtils.Mul(ref _xf, polygonB.Vertices[i]);
                    ref Vector2 reference2 = ref _polygonB.Normals[i];
                    reference2 = MathUtils.Mul(_xf.q, polygonB.Normals[i]);
                }
                _radius = 0.02f;
                manifold.PointCount = 0;
                EPAxis ePAxis = ComputeEdgeSeparation();
                if (ePAxis.Type == EPAxisType.Unknown || ePAxis.Separation > _radius)
                {
                    return;
                }
                EPAxis ePAxis2 = ComputePolygonSeparation();
                if (ePAxis2.Type != EPAxisType.Unknown && ePAxis2.Separation > _radius)
                {
                    return;
                }
                EPAxis ePAxis3 = (ePAxis2.Type == EPAxisType.Unknown) ? ePAxis : ((!(ePAxis2.Separation > (0.98f * ePAxis.Separation) + 0.001f)) ? ePAxis : ePAxis2);
                FixedArray2<ClipVertex> vIn = default;
                ReferenceFace referenceFace = default;
                if (ePAxis3.Type == EPAxisType.EdgeA)
                {
                    manifold.Type = ManifoldType.FaceA;
                    int num4 = 0;
                    float num5 = Vector2.Dot(_normal, _polygonB.Normals[0]);
                    for (int j = 1; j < _polygonB.Count; j++)
                    {
                        float num6 = Vector2.Dot(_normal, _polygonB.Normals[j]);
                        if (num6 < num5)
                        {
                            num5 = num6;
                            num4 = j;
                        }
                    }
                    int num7 = num4;
                    int num8 = (num7 + 1 < _polygonB.Count) ? (num7 + 1) : 0;
                    ClipVertex value = vIn[0];
                    value.V = _polygonB.Vertices[num7];
                    value.ID.Features.IndexA = 0;
                    value.ID.Features.IndexB = (byte)num7;
                    value.ID.Features.TypeA = 1;
                    value.ID.Features.TypeB = 0;
                    vIn[0] = value;
                    ClipVertex value2 = vIn[1];
                    value2.V = _polygonB.Vertices[num8];
                    value2.ID.Features.IndexA = 0;
                    value2.ID.Features.IndexB = (byte)num8;
                    value2.ID.Features.TypeA = 1;
                    value2.ID.Features.TypeB = 0;
                    vIn[1] = value2;
                    if (_front)
                    {
                        referenceFace.i1 = 0;
                        referenceFace.i2 = 1;
                        referenceFace.v1 = _v1;
                        referenceFace.v2 = _v2;
                        referenceFace.normal = _normal1;
                    }
                    else
                    {
                        referenceFace.i1 = 1;
                        referenceFace.i2 = 0;
                        referenceFace.v1 = _v2;
                        referenceFace.v2 = _v1;
                        referenceFace.normal = -_normal1;
                    }
                }
                else
                {
                    manifold.Type = ManifoldType.FaceB;
                    ClipVertex value3 = vIn[0];
                    value3.V = _v1;
                    value3.ID.Features.IndexA = 0;
                    value3.ID.Features.IndexB = (byte)ePAxis3.Index;
                    value3.ID.Features.TypeA = 0;
                    value3.ID.Features.TypeB = 1;
                    vIn[0] = value3;
                    ClipVertex value4 = vIn[1];
                    value4.V = _v2;
                    value4.ID.Features.IndexA = 0;
                    value4.ID.Features.IndexB = (byte)ePAxis3.Index;
                    value4.ID.Features.TypeA = 0;
                    value4.ID.Features.TypeB = 1;
                    vIn[1] = value4;
                    referenceFace.i1 = ePAxis3.Index;
                    referenceFace.i2 = (referenceFace.i1 + 1 < _polygonB.Count) ? (referenceFace.i1 + 1) : 0;
                    referenceFace.v1 = _polygonB.Vertices[referenceFace.i1];
                    referenceFace.v2 = _polygonB.Vertices[referenceFace.i2];
                    referenceFace.normal = _polygonB.Normals[referenceFace.i1];
                }
                referenceFace.sideNormal1 = new Vector2(referenceFace.normal.Y, 0f - referenceFace.normal.X);
                referenceFace.sideNormal2 = -referenceFace.sideNormal1;
                referenceFace.sideOffset1 = Vector2.Dot(referenceFace.sideNormal1, referenceFace.v1);
                referenceFace.sideOffset2 = Vector2.Dot(referenceFace.sideNormal2, referenceFace.v2);
                int num9 = ClipSegmentToLine(out FixedArray2<ClipVertex> vOut, ref vIn, referenceFace.sideNormal1, referenceFace.sideOffset1, referenceFace.i1);
                if (num9 < 2)
                {
                    return;
                }
                num9 = ClipSegmentToLine(out FixedArray2<ClipVertex> vOut2, ref vOut, referenceFace.sideNormal2, referenceFace.sideOffset2, referenceFace.i2);
                if (num9 < 2)
                {
                    return;
                }
                if (ePAxis3.Type == EPAxisType.EdgeA)
                {
                    manifold.LocalNormal = referenceFace.normal;
                    manifold.LocalPoint = referenceFace.v1;
                }
                else
                {
                    manifold.LocalNormal = polygonB.Normals[referenceFace.i1];
                    manifold.LocalPoint = polygonB.Vertices[referenceFace.i1];
                }
                int num10 = 0;
                for (int k = 0; k < 2; k++)
                {
                    float num11 = Vector2.Dot(referenceFace.normal, vOut2[k].V - referenceFace.v1);
                    if (num11 <= _radius)
                    {
                        ManifoldPoint value5 = manifold.Points[num10];
                        if (ePAxis3.Type == EPAxisType.EdgeA)
                        {
                            value5.LocalPoint = MathUtils.MulT(ref _xf, vOut2[k].V);
                            value5.Id = vOut2[k].ID;
                        }
                        else
                        {
                            value5.LocalPoint = vOut2[k].V;
                            value5.Id.Features.TypeA = vOut2[k].ID.Features.TypeB;
                            value5.Id.Features.TypeB = vOut2[k].ID.Features.TypeA;
                            value5.Id.Features.IndexA = vOut2[k].ID.Features.IndexB;
                            value5.Id.Features.IndexB = vOut2[k].ID.Features.IndexA;
                        }
                        manifold.Points[num10] = value5;
                        num10++;
                    }
                }
                manifold.PointCount = num10;
            }

            private EPAxis ComputeEdgeSeparation()
            {
                EPAxis result = default;
                result.Type = EPAxisType.EdgeA;
                result.Index = (!_front) ? 1 : 0;
                result.Separation = float.MaxValue;
                for (int i = 0; i < _polygonB.Count; i++)
                {
                    float num = Vector2.Dot(_normal, _polygonB.Vertices[i] - _v1);
                    if (num < result.Separation)
                    {
                        result.Separation = num;
                    }
                }
                return result;
            }

            private EPAxis ComputePolygonSeparation()
            {
                EPAxis result = default;
                result.Type = EPAxisType.Unknown;
                result.Index = -1;
                result.Separation = float.MinValue;
                Vector2 value = new(0f - _normal.Y, _normal.X);
                for (int i = 0; i < _polygonB.Count; i++)
                {
                    Vector2 vector = -_polygonB.Normals[i];
                    float val = Vector2.Dot(vector, _polygonB.Vertices[i] - _v1);
                    float val2 = Vector2.Dot(vector, _polygonB.Vertices[i] - _v2);
                    float num = Math.Min(val, val2);
                    if (num > _radius)
                    {
                        result.Type = EPAxisType.EdgeB;
                        result.Index = i;
                        result.Separation = num;
                        return result;
                    }
                    if (Vector2.Dot(vector, value) >= 0f)
                    {
                        if (Vector2.Dot(vector - _upperLimit, _normal) < -(float)Math.PI / 90f)
                        {
                            continue;
                        }
                    }
                    else if (Vector2.Dot(vector - _lowerLimit, _normal) < -(float)Math.PI / 90f)
                    {
                        continue;
                    }
                    if (num > result.Separation)
                    {
                        result.Type = EPAxisType.EdgeB;
                        result.Index = i;
                        result.Separation = num;
                    }
                }
                return result;
            }
        }

        [ThreadStatic]
        private static DistanceInput _input;

        public static bool TestOverlap(Shape shapeA, int indexA, Shape shapeB, int indexB, ref Transform xfA, ref Transform xfB)
        {
            _input ??= new DistanceInput();
            _input.ProxyA.Set(shapeA, indexA);
            _input.ProxyB.Set(shapeB, indexB);
            _input.TransformA = xfA;
            _input.TransformB = xfB;
            _input.UseRadii = true;
            Distance.ComputeDistance(out DistanceOutput output, out SimplexCache _, _input);
            return output.Distance < 1.1920929E-06f;
        }

        public static void GetPointStates(out FixedArray2<PointState> state1, out FixedArray2<PointState> state2, ref Manifold manifold1, ref Manifold manifold2)
        {
            state1 = default;
            state2 = default;
            for (int i = 0; i < manifold1.PointCount; i++)
            {
                ContactID id = manifold1.Points[i].Id;
                state1[i] = PointState.Remove;
                for (int j = 0; j < manifold2.PointCount; j++)
                {
                    if (manifold2.Points[j].Id.Key == id.Key)
                    {
                        state1[i] = PointState.Persist;
                        break;
                    }
                }
            }
            for (int k = 0; k < manifold2.PointCount; k++)
            {
                ContactID id2 = manifold2.Points[k].Id;
                state2[k] = PointState.Add;
                for (int l = 0; l < manifold1.PointCount; l++)
                {
                    if (manifold1.Points[l].Id.Key == id2.Key)
                    {
                        state2[k] = PointState.Persist;
                        break;
                    }
                }
            }
        }

        public static void CollideCircles(ref Manifold manifold, CircleShape circleA, ref Transform xfA, CircleShape circleB, ref Transform xfB)
        {
            manifold.PointCount = 0;
            Vector2 vector = MathUtils.Mul(ref xfA, circleA.Position);
            Vector2 vector2 = MathUtils.Mul(ref xfB, circleB.Position);
            Vector2 vector3 = vector2 - vector;
            float num = Vector2.Dot(vector3, vector3);
            float num2 = circleA.Radius + circleB.Radius;
            if (!(num > num2 * num2))
            {
                manifold.Type = ManifoldType.Circles;
                manifold.LocalPoint = circleA.Position;
                manifold.LocalNormal = Vector2.Zero;
                manifold.PointCount = 1;
                ManifoldPoint value = manifold.Points[0];
                value.LocalPoint = circleB.Position;
                value.Id.Key = 0u;
                manifold.Points[0] = value;
            }
        }

        public static void CollidePolygonAndCircle(ref Manifold manifold, PolygonShape polygonA, ref Transform xfA, CircleShape circleB, ref Transform xfB)
        {
            manifold.PointCount = 0;
            Vector2 v = MathUtils.Mul(ref xfB, circleB.Position);
            Vector2 vector = MathUtils.MulT(ref xfA, v);
            int num = 0;
            float num2 = float.MinValue;
            float num3 = polygonA.Radius + circleB.Radius;
            int count = polygonA.Vertices.Count;
            for (int i = 0; i < count; i++)
            {
                Vector2 vector2 = polygonA.Normals[i];
                Vector2 vector3 = vector - polygonA.Vertices[i];
                float num4 = (vector2.X * vector3.X) + (vector2.Y * vector3.Y);
                if (num4 > num3)
                {
                    return;
                }
                if (num4 > num2)
                {
                    num2 = num4;
                    num = i;
                }
            }
            int num5 = num;
            int index = (num5 + 1 < count) ? (num5 + 1) : 0;
            Vector2 vector4 = polygonA.Vertices[num5];
            Vector2 vector5 = polygonA.Vertices[index];
            if (num2 < 1.1920929E-07f)
            {
                manifold.PointCount = 1;
                manifold.Type = ManifoldType.FaceA;
                manifold.LocalNormal = polygonA.Normals[num];
                manifold.LocalPoint = 0.5f * (vector4 + vector5);
                ManifoldPoint value = manifold.Points[0];
                value.LocalPoint = circleB.Position;
                value.Id.Key = 0u;
                manifold.Points[0] = value;
                return;
            }
            float num6 = ((vector.X - vector4.X) * (vector5.X - vector4.X)) + ((vector.Y - vector4.Y) * (vector5.Y - vector4.Y));
            float num7 = ((vector.X - vector5.X) * (vector4.X - vector5.X)) + ((vector.Y - vector5.Y) * (vector4.Y - vector5.Y));
            if (num6 <= 0f)
            {
                float num8 = ((vector.X - vector4.X) * (vector.X - vector4.X)) + ((vector.Y - vector4.Y) * (vector.Y - vector4.Y));
                if (!(num8 > num3 * num3))
                {
                    manifold.PointCount = 1;
                    manifold.Type = ManifoldType.FaceA;
                    manifold.LocalNormal = vector - vector4;
                    float num9 = 1f / (float)Math.Sqrt((manifold.LocalNormal.X * manifold.LocalNormal.X) + (manifold.LocalNormal.Y * manifold.LocalNormal.Y));
                    manifold.LocalNormal.X *= num9;
                    manifold.LocalNormal.Y *= num9;
                    manifold.LocalPoint = vector4;
                    ManifoldPoint value2 = manifold.Points[0];
                    value2.LocalPoint = circleB.Position;
                    value2.Id.Key = 0u;
                    manifold.Points[0] = value2;
                }
            }
            else if (num7 <= 0f)
            {
                float num10 = ((vector.X - vector5.X) * (vector.X - vector5.X)) + ((vector.Y - vector5.Y) * (vector.Y - vector5.Y));
                if (!(num10 > num3 * num3))
                {
                    manifold.PointCount = 1;
                    manifold.Type = ManifoldType.FaceA;
                    manifold.LocalNormal = vector - vector5;
                    float num11 = 1f / (float)Math.Sqrt((manifold.LocalNormal.X * manifold.LocalNormal.X) + (manifold.LocalNormal.Y * manifold.LocalNormal.Y));
                    manifold.LocalNormal.X *= num11;
                    manifold.LocalNormal.Y *= num11;
                    manifold.LocalPoint = vector5;
                    ManifoldPoint value3 = manifold.Points[0];
                    value3.LocalPoint = circleB.Position;
                    value3.Id.Key = 0u;
                    manifold.Points[0] = value3;
                }
            }
            else
            {
                Vector2 vector6 = 0.5f * (vector4 + vector5);
                Vector2 vector7 = vector - vector6;
                Vector2 vector8 = polygonA.Normals[num5];
                float num12 = (vector7.X * vector8.X) + (vector7.Y * vector8.Y);
                if (!(num12 > num3))
                {
                    manifold.PointCount = 1;
                    manifold.Type = ManifoldType.FaceA;
                    manifold.LocalNormal = polygonA.Normals[num5];
                    manifold.LocalPoint = vector6;
                    ManifoldPoint value4 = manifold.Points[0];
                    value4.LocalPoint = circleB.Position;
                    value4.Id.Key = 0u;
                    manifold.Points[0] = value4;
                }
            }
        }

        public static void CollidePolygons(ref Manifold manifold, PolygonShape polyA, ref Transform transformA, PolygonShape polyB, ref Transform transformB)
        {
            manifold.PointCount = 0;
            float num = polyA.Radius + polyB.Radius;
            float num2 = FindMaxSeparation(out int edgeIndex, polyA, ref transformA, polyB, ref transformB);
            if (num2 > num)
            {
                return;
            }
            float num3 = FindMaxSeparation(out int edgeIndex2, polyB, ref transformB, polyA, ref transformA);
            if (num3 > num)
            {
                return;
            }
            PolygonShape polygonShape;
            PolygonShape poly;
            Transform xf;
            Transform xf2;
            int num4;
            bool flag;
            if (num3 > (0.98f * num2) + 0.001f)
            {
                polygonShape = polyB;
                poly = polyA;
                xf = transformB;
                xf2 = transformA;
                num4 = edgeIndex2;
                manifold.Type = ManifoldType.FaceB;
                flag = true;
            }
            else
            {
                polygonShape = polyA;
                poly = polyB;
                xf = transformA;
                xf2 = transformB;
                num4 = edgeIndex;
                manifold.Type = ManifoldType.FaceA;
                flag = false;
            }
            FindIncidentEdge(out FixedArray2<ClipVertex> c, polygonShape, ref xf, num4, poly, ref xf2);
            int count = polygonShape.Vertices.Count;
            int num5 = num4;
            int num6 = (num4 + 1 < count) ? (num4 + 1) : 0;
            Vector2 vector = polygonShape.Vertices[num5];
            Vector2 vector2 = polygonShape.Vertices[num6];
            Vector2 v = vector2 - vector;
            v = Vector2.Normalize(v);
            Vector2 localNormal = new(v.Y, 0f - v.X);
            Vector2 localPoint = 0.5f * (vector + vector2);
            Vector2 vector3 = MathUtils.Mul(xf.q, v);
            float y = vector3.Y;
            float num7 = 0f - vector3.X;
            vector = MathUtils.Mul(ref xf, vector);
            vector2 = MathUtils.Mul(ref xf, vector2);
            float num8 = (y * vector.X) + (num7 * vector.Y);
            float offset = 0f - ((vector3.X * vector.X) + (vector3.Y * vector.Y)) + num;
            float offset2 = (vector3.X * vector2.X) + (vector3.Y * vector2.Y) + num;
            int num9 = ClipSegmentToLine(out FixedArray2<ClipVertex> vOut, ref c, -vector3, offset, num5);
            if (num9 < 2)
            {
                return;
            }
            num9 = ClipSegmentToLine(out FixedArray2<ClipVertex> vOut2, ref vOut, vector3, offset2, num6);
            if (num9 < 2)
            {
                return;
            }
            manifold.LocalNormal = localNormal;
            manifold.LocalPoint = localPoint;
            int num10 = 0;
            for (int i = 0; i < 2; i++)
            {
                Vector2 v2 = vOut2[i].V;
                float num11 = (y * v2.X) + (num7 * v2.Y) - num8;
                if (num11 <= num)
                {
                    ManifoldPoint value = manifold.Points[num10];
                    value.LocalPoint = MathUtils.MulT(ref xf2, vOut2[i].V);
                    value.Id = vOut2[i].ID;
                    if (flag)
                    {
                        ContactFeature features = value.Id.Features;
                        value.Id.Features.IndexA = features.IndexB;
                        value.Id.Features.IndexB = features.IndexA;
                        value.Id.Features.TypeA = features.TypeB;
                        value.Id.Features.TypeB = features.TypeA;
                    }
                    manifold.Points[num10] = value;
                    num10++;
                }
            }
            manifold.PointCount = num10;
        }

        public static void CollideEdgeAndCircle(ref Manifold manifold, EdgeShape edgeA, ref Transform transformA, CircleShape circleB, ref Transform transformB)
        {
            manifold.PointCount = 0;
            Vector2 vector = MathUtils.MulT(ref transformA, MathUtils.Mul(ref transformB, ref circleB._position));
            Vector2 vertex = edgeA.Vertex1;
            Vector2 vertex2 = edgeA.Vertex2;
            Vector2 value = vertex2 - vertex;
            float num = Vector2.Dot(value, vertex2 - vector);
            float num2 = Vector2.Dot(value, vector - vertex);
            float num3 = edgeA.Radius + circleB.Radius;
            ContactFeature features = default;
            features.IndexB = 0;
            features.TypeB = 0;
            Vector2 vector2;
            Vector2 value2;
            if (num2 <= 0f)
            {
                vector2 = vertex;
                value2 = vector - vector2;
                float result = Vector2.Dot(value2, value2);
                if (result > num3 * num3)
                {
                    return;
                }
                if (edgeA.HasVertex0)
                {
                    Vector2 vertex3 = edgeA.Vertex0;
                    Vector2 vector3 = vertex;
                    Vector2 value3 = vector3 - vertex3;
                    float num4 = Vector2.Dot(value3, vector3 - vector);
                    if (num4 > 0f)
                    {
                        return;
                    }
                }
                features.IndexA = 0;
                features.TypeA = 0;
                manifold.PointCount = 1;
                manifold.Type = ManifoldType.Circles;
                manifold.LocalNormal = Vector2.Zero;
                manifold.LocalPoint = vector2;
                ManifoldPoint value4 = new()
                {
                    Id =
                    {
                        Key = 0u,
                        Features = features
                    },
                    LocalPoint = circleB.Position
                };
                manifold.Points[0] = value4;
                return;
            }
            if (num <= 0f)
            {
                vector2 = vertex2;
                value2 = vector - vector2;
                float result2 = Vector2.Dot(value2, value2);
                if (result2 > num3 * num3)
                {
                    return;
                }
                if (edgeA.HasVertex3)
                {
                    Vector2 vertex4 = edgeA.Vertex3;
                    Vector2 vector4 = vertex2;
                    Vector2 value5 = vertex4 - vector4;
                    float num5 = Vector2.Dot(value5, vector - vector4);
                    if (num5 > 0f)
                    {
                        return;
                    }
                }
                features.IndexA = 1;
                features.TypeA = 0;
                manifold.PointCount = 1;
                manifold.Type = ManifoldType.Circles;
                manifold.LocalNormal = Vector2.Zero;
                manifold.LocalPoint = vector2;
                ManifoldPoint value6 = new()
                {
                    Id =
                    {
                        Key = 0u,
                        Features = features
                    },
                    LocalPoint = circleB.Position
                };
                manifold.Points[0] = value6;
                return;
            }
            float result3 = Vector2.Dot(value, value);
            vector2 = 1f / result3 * ((num * vertex) + (num2 * vertex2));
            value2 = vector - vector2;
            float result4 = Vector2.Dot(value2, value2);
            if (!(result4 > num3 * num3))
            {
                Vector2 vector5 = new(0f - value.Y, value.X);
                if (Vector2.Dot(vector5, vector - vertex) < 0f)
                {
                    vector5 = new Vector2(0f - vector5.X, 0f - vector5.Y);
                }
                vector5 = Vector2.Normalize(vector5);
                features.IndexA = 0;
                features.TypeA = 1;
                manifold.PointCount = 1;
                manifold.Type = ManifoldType.FaceA;
                manifold.LocalNormal = vector5;
                manifold.LocalPoint = vertex;
                ManifoldPoint value7 = new()
                {
                    Id =
                    {
                        Key = 0u,
                        Features = features
                    },
                    LocalPoint = circleB.Position
                };
                manifold.Points[0] = value7;
            }
        }

        public static void CollideEdgeAndPolygon(ref Manifold manifold, EdgeShape edgeA, ref Transform xfA, PolygonShape polygonB, ref Transform xfB)
        {
            EPCollider ePCollider = new();
            ePCollider.Collide(ref manifold, edgeA, ref xfA, polygonB, ref xfB);
        }

        private static int ClipSegmentToLine(out FixedArray2<ClipVertex> vOut, ref FixedArray2<ClipVertex> vIn, Vector2 normal, float offset, int vertexIndexA)
        {
            vOut = default;
            ClipVertex value = vIn[0];
            ClipVertex value2 = vIn[1];
            int num = 0;
            float num2 = (normal.X * value.V.X) + (normal.Y * value.V.Y) - offset;
            float num3 = (normal.X * value2.V.X) + (normal.Y * value2.V.Y) - offset;
            if (num2 <= 0f)
            {
                vOut[num++] = value;
            }
            if (num3 <= 0f)
            {
                vOut[num++] = value2;
            }
            if (num2 * num3 < 0f)
            {
                float num4 = num2 / (num2 - num3);
                ClipVertex value3 = vOut[num];
                value3.V.X = value.V.X + (num4 * (value2.V.X - value.V.X));
                value3.V.Y = value.V.Y + (num4 * (value2.V.Y - value.V.Y));
                value3.ID.Features.IndexA = (byte)vertexIndexA;
                value3.ID.Features.IndexB = value.ID.Features.IndexB;
                value3.ID.Features.TypeA = 0;
                value3.ID.Features.TypeB = 1;
                vOut[num] = value3;
                num++;
            }
            return num;
        }

        private static float EdgeSeparation(PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2)
        {
            List<Vector2> vertices = poly1.Vertices;
            List<Vector2> normals = poly1.Normals;
            int count = poly2.Vertices.Count;
            List<Vector2> vertices2 = poly2.Vertices;
            Vector2 vector = MathUtils.Mul(xf1.q, normals[edge1]);
            Vector2 value = MathUtils.MulT(xf2.q, vector);
            int index = 0;
            float num = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                float num2 = Vector2.Dot(vertices2[i], value);
                if (num2 < num)
                {
                    num = num2;
                    index = i;
                }
            }
            Vector2 vector2 = MathUtils.Mul(ref xf1, vertices[edge1]);
            Vector2 vector3 = MathUtils.Mul(ref xf2, vertices2[index]);
            return Vector2.Dot(vector3 - vector2, vector);
        }

        private static float FindMaxSeparation(out int edgeIndex, PolygonShape poly1, ref Transform xf1, PolygonShape poly2, ref Transform xf2)
        {
            int count = poly1.Vertices.Count;
            List<Vector2> normals = poly1.Normals;
            Vector2 v = MathUtils.Mul(ref xf2, poly2.MassData.Centroid) - MathUtils.Mul(ref xf1, poly1.MassData.Centroid);
            Vector2 value = MathUtils.MulT(xf1.q, v);
            int num = 0;
            float num2 = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                float num3 = Vector2.Dot(normals[i], value);
                if (num3 > num2)
                {
                    num2 = num3;
                    num = i;
                }
            }
            float num4 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
            int num5 = (num - 1 >= 0) ? (num - 1) : (count - 1);
            float num6 = EdgeSeparation(poly1, ref xf1, num5, poly2, ref xf2);
            int num7 = (num + 1 < count) ? (num + 1) : 0;
            float num8 = EdgeSeparation(poly1, ref xf1, num7, poly2, ref xf2);
            int num9;
            int num10;
            float num11;
            if (num6 > num4 && num6 > num8)
            {
                num9 = -1;
                num10 = num5;
                num11 = num6;
            }
            else
            {
                if (!(num8 > num4))
                {
                    edgeIndex = num;
                    return num4;
                }
                num9 = 1;
                num10 = num7;
                num11 = num8;
            }
            while (true)
            {
                num = (num9 != -1) ? ((num10 + 1 < count) ? (num10 + 1) : 0) : ((num10 - 1 >= 0) ? (num10 - 1) : (count - 1));
                num4 = EdgeSeparation(poly1, ref xf1, num, poly2, ref xf2);
                if (!(num4 > num11))
                {
                    break;
                }
                num10 = num;
                num11 = num4;
            }
            edgeIndex = num10;
            return num11;
        }

        private static void FindIncidentEdge(out FixedArray2<ClipVertex> c, PolygonShape poly1, ref Transform xf1, int edge1, PolygonShape poly2, ref Transform xf2)
        {
            c = default;
            Vertices normals = poly1.Normals;
            int count = poly2.Vertices.Count;
            Vertices vertices = poly2.Vertices;
            Vertices normals2 = poly2.Normals;
            Vector2 value = MathUtils.MulT(xf2.q, MathUtils.Mul(xf1.q, normals[edge1]));
            int num = 0;
            float num2 = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                float num3 = Vector2.Dot(value, normals2[i]);
                if (num3 < num2)
                {
                    num2 = num3;
                    num = i;
                }
            }
            int num4 = num;
            int num5 = (num4 + 1 < count) ? (num4 + 1) : 0;
            ClipVertex value2 = c[0];
            value2.V = MathUtils.Mul(ref xf2, vertices[num4]);
            value2.ID.Features.IndexA = (byte)edge1;
            value2.ID.Features.IndexB = (byte)num4;
            value2.ID.Features.TypeA = 1;
            value2.ID.Features.TypeB = 0;
            c[0] = value2;
            ClipVertex value3 = c[1];
            value3.V = MathUtils.Mul(ref xf2, vertices[num5]);
            value3.ID.Features.IndexA = (byte)edge1;
            value3.ID.Features.IndexB = (byte)num5;
            value3.ID.Features.TypeA = 1;
            value3.ID.Features.TypeB = 0;
            c[1] = value3;
        }
    }
}
