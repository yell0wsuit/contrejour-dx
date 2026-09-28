using System.Collections.Generic;

using FarseerPhysics.Common.Decomposition.Seidel;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.Decomposition;

internal static class SeidelDecomposer
{
    public static List<Vertices> ConvexPartition(Vertices vertices, float sheer = 0.001f)
    {
        List<FarseerPhysics.Common.Decomposition.Seidel.Point> list = new List<FarseerPhysics.Common.Decomposition.Seidel.Point>(vertices.Count);
        foreach (Vector2 vertex in vertices)
        {
            list.Add(new FarseerPhysics.Common.Decomposition.Seidel.Point(vertex.X, vertex.Y));
        }
        Triangulator triangulator = new Triangulator(list, sheer);
        List<Vertices> list2 = new List<Vertices>();
        foreach (List<FarseerPhysics.Common.Decomposition.Seidel.Point> triangle in triangulator.Triangles)
        {
            Vertices vertices2 = new Vertices(triangle.Count);
            foreach (FarseerPhysics.Common.Decomposition.Seidel.Point item in triangle)
            {
                vertices2.Add(new Vector2(item.X, item.Y));
            }
            list2.Add(vertices2);
        }
        return list2;
    }

    public static List<Vertices> ConvexPartitionTrapezoid(Vertices vertices, float sheer = 0.001f)
    {
        List<FarseerPhysics.Common.Decomposition.Seidel.Point> list = new List<FarseerPhysics.Common.Decomposition.Seidel.Point>(vertices.Count);
        foreach (Vector2 vertex in vertices)
        {
            list.Add(new FarseerPhysics.Common.Decomposition.Seidel.Point(vertex.X, vertex.Y));
        }
        Triangulator triangulator = new Triangulator(list, sheer);
        List<Vertices> list2 = new List<Vertices>();
        foreach (Trapezoid trapezoid in triangulator.Trapezoids)
        {
            Vertices vertices2 = new Vertices();
            List<FarseerPhysics.Common.Decomposition.Seidel.Point> vertices3 = trapezoid.GetVertices();
            foreach (FarseerPhysics.Common.Decomposition.Seidel.Point item in vertices3)
            {
                vertices2.Add(new Vector2(item.X, item.Y));
            }
            list2.Add(vertices2);
        }
        return list2;
    }
}
