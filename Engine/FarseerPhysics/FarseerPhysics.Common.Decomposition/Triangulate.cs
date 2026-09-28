using System;
using System.Collections.Generic;

using FarseerPhysics.Common.ConvexHull;

namespace FarseerPhysics.Common.Decomposition;

public static class Triangulate
{
    public static List<Vertices> ConvexPartition(Vertices vertices, TriangulationAlgorithm algorithm, bool discardAndFixInvalid = true, float tolerance = 0.001f)
    {
        if (vertices.Count <= 3)
        {
            List<Vertices> list = [vertices];
            return list;
        }
        List<Vertices> list2;
        switch (algorithm)
        {
            case TriangulationAlgorithm.Earclip:
                if (vertices.IsCounterClockWise())
                {
                    Vertices vertices4 = [.. vertices];
                    vertices4.Reverse();
                    list2 = EarclipDecomposer.ConvexPartition(vertices4, tolerance);
                }
                else
                {
                    list2 = EarclipDecomposer.ConvexPartition(vertices, tolerance);
                }
                break;
            case TriangulationAlgorithm.Bayazit:
                if (!vertices.IsCounterClockWise())
                {
                    Vertices vertices3 = [.. vertices];
                    vertices3.Reverse();
                    list2 = BayazitDecomposer.ConvexPartition(vertices3);
                }
                else
                {
                    list2 = BayazitDecomposer.ConvexPartition(vertices);
                }
                break;
            case TriangulationAlgorithm.Flipcode:
                if (!vertices.IsCounterClockWise())
                {
                    Vertices vertices2 = [.. vertices];
                    vertices2.Reverse();
                    list2 = FlipcodeDecomposer.ConvexPartition(vertices2);
                }
                else
                {
                    list2 = FlipcodeDecomposer.ConvexPartition(vertices);
                }
                break;
            case TriangulationAlgorithm.Seidel:
                list2 = SeidelDecomposer.ConvexPartition(vertices, tolerance);
                break;
            case TriangulationAlgorithm.SeidelTrapezoids:
                list2 = SeidelDecomposer.ConvexPartitionTrapezoid(vertices, tolerance);
                break;
            case TriangulationAlgorithm.Delauny:
                list2 = CDTDecomposer.ConvexPartition(vertices);
                break;
            default:
                throw new ArgumentOutOfRangeException("algorithm");
        }
        if (discardAndFixInvalid)
        {
            for (int num = list2.Count - 1; num >= 0; num--)
            {
                Vertices polygon = list2[num];
                if (!ValidatePolygon(polygon))
                {
                    list2.RemoveAt(num);
                }
            }
        }
        return list2;
    }

    private static bool ValidatePolygon(Vertices polygon)
    {
        PolygonError polygonError = polygon.CheckPolygon();
        switch (polygonError)
        {
            case PolygonError.InvalidAmountOfVertices:
            case PolygonError.NotSimple:
            case PolygonError.AreaTooSmall:
            case PolygonError.SideTooSmall:
                return false;
            case PolygonError.NotCounterClockWise:
                polygon.Reverse();
                break;
        }
        if (polygonError == PolygonError.NotConvex)
        {
            polygon = GiftWrap.GetConvexHull(polygon);
            return ValidatePolygon(polygon);
        }
        return true;
    }
}
