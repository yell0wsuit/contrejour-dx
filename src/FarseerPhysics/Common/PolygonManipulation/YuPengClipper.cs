using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.PolygonManipulation
{
    public static class YuPengClipper
    {
        private sealed class Edge(Vector2 edgeStart, Vector2 edgeEnd)
        {
            public Vector2 EdgeStart { get; private set; } = edgeStart;

            public Vector2 EdgeEnd { get; private set; } = edgeEnd;

            public Vector2 GetCenter()
            {
                return XnaMath.Divide(EdgeStart + EdgeEnd, 2f);
            }

            public static Edge operator -(Edge e)
            {
                return new Edge(e.EdgeEnd, e.EdgeStart);
            }

            public override bool Equals(object obj)
            {
                return obj != null && Equals(obj as Edge);
            }

            public bool Equals(Edge e)
            {
                return e != null && VectorEqual(EdgeStart, e.EdgeStart) && VectorEqual(EdgeEnd, e.EdgeEnd);
            }

            public override int GetHashCode()
            {
                return EdgeStart.GetHashCode() ^ EdgeEnd.GetHashCode();
            }
        }

        public static List<Vertices> Union(Vertices polygon1, Vertices polygon2, out PolyClipError error)
        {
            return Execute(polygon1, polygon2, PolyClipType.Union, out error);
        }

        public static List<Vertices> Difference(Vertices polygon1, Vertices polygon2, out PolyClipError error)
        {
            return Execute(polygon1, polygon2, PolyClipType.Difference, out error);
        }

        public static List<Vertices> Intersect(Vertices polygon1, Vertices polygon2, out PolyClipError error)
        {
            return Execute(polygon1, polygon2, PolyClipType.Intersect, out error);
        }

        private static List<Vertices> Execute(Vertices subject, Vertices clip, PolyClipType clipType, out PolyClipError error)
        {
            CalculateIntersections(subject, clip, out Vertices slicedPoly, out Vertices slicedPoly2);
            Vector2 value = subject.GetAABB().LowerBound;
            Vector2 value2 = clip.GetAABB().LowerBound;
            Vector2 result = XnaMath.Min(value, value2);
            result = Vector2.One - result;
            if (result != Vector2.Zero)
            {
                slicedPoly.Translate(ref result);
                slicedPoly2.Translate(ref result);
            }
            slicedPoly.ForceCounterClockWise();
            slicedPoly2.ForceCounterClockWise();
            CalculateSimplicalChain(slicedPoly, out List<float> coeff, out List<Edge> simplicies);
            CalculateSimplicalChain(slicedPoly2, out List<float> coeff2, out List<Edge> simplicies2);
            CalculateResultChain(coeff, simplicies, coeff2, simplicies2, clipType, out List<Edge> resultSimplices);
            error = BuildPolygonsFromChain(resultSimplices, out List<Vertices> result2);
            result *= -1f;
            for (int i = 0; i < result2.Count; i++)
            {
                result2[i].Translate(ref result);
                _ = SimplifyTools.CollinearSimplify(result2[i]);
            }
            return result2;
        }

        private static void CalculateIntersections(Vertices polygon1, Vertices polygon2, out Vertices slicedPoly1, out Vertices slicedPoly2)
        {
            slicedPoly1 = [.. polygon1];
            slicedPoly2 = [.. polygon2];
            for (int i = 0; i < polygon1.Count; i++)
            {
                Vector2 vector = polygon1[i];
                Vector2 vector2 = polygon1[polygon1.NextIndex(i)];
                for (int j = 0; j < polygon2.Count; j++)
                {
                    Vector2 vector3 = polygon2[j];
                    Vector2 vector4 = polygon2[polygon2.NextIndex(j)];
                    if (!LineTools.LineIntersect(vector, vector2, vector3, vector4, out Vector2 intersectionPoint))
                    {
                        continue;
                    }
                    float alpha = GetAlpha(vector, vector2, intersectionPoint);
                    if (alpha is > 0f and < 1f)
                    {
                        int k;
                        for (k = slicedPoly1.IndexOf(vector) + 1; k < slicedPoly1.Count && GetAlpha(vector, vector2, slicedPoly1[k]) <= alpha; k++)
                        {
                        }
                        slicedPoly1.Insert(k, intersectionPoint);
                    }
                    alpha = GetAlpha(vector3, vector4, intersectionPoint);
                    if (alpha is > 0f and < 1f)
                    {
                        int l;
                        for (l = slicedPoly2.IndexOf(vector3) + 1; l < slicedPoly2.Count && GetAlpha(vector3, vector4, slicedPoly2[l]) <= alpha; l++)
                        {
                        }
                        slicedPoly2.Insert(l, intersectionPoint);
                    }
                }
            }
            for (int m = 0; m < slicedPoly1.Count; m++)
            {
                int index = slicedPoly1.NextIndex(m);
                if ((slicedPoly1[index] - slicedPoly1[m]).LengthSquared() <= 1.1920929E-07f)
                {
                    slicedPoly1.RemoveAt(m);
                    m--;
                }
            }
            for (int n = 0; n < slicedPoly2.Count; n++)
            {
                int index2 = slicedPoly2.NextIndex(n);
                if ((slicedPoly2[index2] - slicedPoly2[n]).LengthSquared() <= 1.1920929E-07f)
                {
                    slicedPoly2.RemoveAt(n);
                    n--;
                }
            }
        }

        private static void CalculateSimplicalChain(Vertices poly, out List<float> coeff, out List<Edge> simplicies)
        {
            simplicies = [];
            coeff = [];
            for (int i = 0; i < poly.Count; i++)
            {
                simplicies.Add(new Edge(poly[i], poly[poly.NextIndex(i)]));
                coeff.Add(CalculateSimplexCoefficient(Vector2.Zero, poly[i], poly[poly.NextIndex(i)]));
            }
        }

        private static void CalculateResultChain(List<float> poly1Coeff, List<Edge> poly1Simplicies, List<float> poly2Coeff, List<Edge> poly2Simplicies, PolyClipType clipType, out List<Edge> resultSimplices)
        {
            resultSimplices = [];
            for (int i = 0; i < poly1Simplicies.Count; i++)
            {
                float num = 0f;
                if (poly2Simplicies.Contains(poly1Simplicies[i]))
                {
                    num = 1f;
                }
                else if (poly2Simplicies.Contains(-poly1Simplicies[i]) && clipType == PolyClipType.Union)
                {
                    num = 1f;
                }
                else
                {
                    for (int j = 0; j < poly2Simplicies.Count; j++)
                    {
                        if (!poly2Simplicies.Contains(-poly1Simplicies[i]))
                        {
                            num += CalculateBeta(poly1Simplicies[i].GetCenter(), poly2Simplicies[j], poly2Coeff[j]);
                        }
                    }
                }
                if (clipType == PolyClipType.Intersect)
                {
                    if (num == 1f)
                    {
                        resultSimplices.Add(poly1Simplicies[i]);
                    }
                }
                else if (num == 0f)
                {
                    resultSimplices.Add(poly1Simplicies[i]);
                }
            }
            for (int k = 0; k < poly2Simplicies.Count; k++)
            {
                if (resultSimplices.Contains(poly2Simplicies[k]) || resultSimplices.Contains(-poly2Simplicies[k]))
                {
                    continue;
                }
                if (poly1Simplicies.Contains(-poly2Simplicies[k]) && clipType == PolyClipType.Union)
                {
                    continue;
                }
                float num2 = 0f;
                for (int l = 0; l < poly1Simplicies.Count; l++)
                {
                    if (!poly1Simplicies.Contains(poly2Simplicies[k]) && !poly1Simplicies.Contains(-poly2Simplicies[k]))
                    {
                        num2 += CalculateBeta(poly2Simplicies[k].GetCenter(), poly1Simplicies[l], poly1Coeff[l]);
                    }
                }
                if (clipType is PolyClipType.Intersect or PolyClipType.Difference)
                {
                    if (num2 == 1f)
                    {
                        resultSimplices.Add(-poly2Simplicies[k]);
                    }
                }
                else if (num2 == 0f)
                {
                    resultSimplices.Add(poly2Simplicies[k]);
                }
            }
        }

        private static PolyClipError BuildPolygonsFromChain(List<Edge> simplicies, out List<Vertices> result)
        {
            result = [];
            PolyClipError result2 = PolyClipError.None;
            while (simplicies.Count > 0)
            {
                Vertices vertices = [simplicies[0].EdgeStart, simplicies[0].EdgeEnd];
                simplicies.RemoveAt(0);
                bool flag = false;
                int num = 0;
                int count = simplicies.Count;
                while (!flag && simplicies.Count > 0)
                {
                    if (VectorEqual(vertices[^1], simplicies[num].EdgeStart))
                    {
                        if (VectorEqual(simplicies[num].EdgeEnd, vertices[0]))
                        {
                            flag = true;
                        }
                        else
                        {
                            vertices.Add(simplicies[num].EdgeEnd);
                        }
                        simplicies.RemoveAt(num);
                        num--;
                    }
                    else if (VectorEqual(vertices[^1], simplicies[num].EdgeEnd))
                    {
                        if (VectorEqual(simplicies[num].EdgeStart, vertices[0]))
                        {
                            flag = true;
                        }
                        else
                        {
                            vertices.Add(simplicies[num].EdgeStart);
                        }
                        simplicies.RemoveAt(num);
                        num--;
                    }
                    if (!flag && ++num == simplicies.Count)
                    {
                        if (count == simplicies.Count)
                        {
                            result = [];
                            return PolyClipError.BrokenResult;
                        }
                        num = 0;
                        count = simplicies.Count;
                    }
                }
                if (vertices.Count < 3)
                {
                    result2 = PolyClipError.DegeneratedOutput;
                }
                result.Add(vertices);
            }
            return result2;
        }

        private static float CalculateBeta(Vector2 point, Edge e, float coefficient)
        {
            float result = 0f;
            if (PointInSimplex(point, e))
            {
                result = coefficient;
            }
            if (PointOnLineSegment(Vector2.Zero, e.EdgeStart, point) || PointOnLineSegment(Vector2.Zero, e.EdgeEnd, point))
            {
                result = 0.5f * coefficient;
            }
            return result;
        }

        private static float GetAlpha(Vector2 start, Vector2 end, Vector2 point)
        {
            return (point - start).LengthSquared() / (end - start).LengthSquared();
        }

        private static float CalculateSimplexCoefficient(Vector2 a, Vector2 b, Vector2 c)
        {
            float num = MathUtils.Area(ref a, ref b, ref c);
            return num < 0f ? -1f : num > 0f ? 1f : 0f;
        }

        private static bool PointInSimplex(Vector2 point, Edge edge)
        {
            Vertices vertices = [Vector2.Zero, edge.EdgeStart, edge.EdgeEnd];
            return vertices.PointInPolygon(ref point) == 1;
        }

        private static bool PointOnLineSegment(Vector2 start, Vector2 end, Vector2 point)
        {
            Vector2 value = end - start;
            return MathUtils.Area(ref start, ref end, ref point) == 0f && Vector2.Dot(point - start, value) >= 0f && Vector2.Dot(point - end, value) <= 0f;
        }

        private static bool VectorEqual(Vector2 vec1, Vector2 vec2)
        {
            return (vec2 - vec1).LengthSquared() <= 1.1920929E-07f;
        }
    }
}
