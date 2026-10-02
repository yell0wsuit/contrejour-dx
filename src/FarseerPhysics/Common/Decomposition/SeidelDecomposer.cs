using System.Collections.Generic;
using System.Numerics;

using FarseerPhysics.Common.Decomposition.Seidel;

namespace FarseerPhysics.Common.Decomposition
{
    internal static class SeidelDecomposer
    {
        public static List<Vertices> ConvexPartition(Vertices vertices, float sheer = 0.001f)
        {
            List<Point> list = new(vertices.Count);
            foreach (Vector2 vertex in vertices)
            {
                list.Add(new Point(vertex.X, vertex.Y));
            }
            Triangulator triangulator = new(list, sheer);
            List<Vertices> list2 = [];
            foreach (List<Point> triangle in triangulator.Triangles)
            {
                Vertices vertices2 = new(triangle.Count);
                foreach (Point item in triangle)
                {
                    vertices2.Add(new Vector2(item.X, item.Y));
                }
                list2.Add(vertices2);
            }
            return list2;
        }

        public static List<Vertices> ConvexPartitionTrapezoid(Vertices vertices, float sheer = 0.001f)
        {
            List<Point> list = new(vertices.Count);
            foreach (Vector2 vertex in vertices)
            {
                list.Add(new Point(vertex.X, vertex.Y));
            }
            Triangulator triangulator = new(list, sheer);
            List<Vertices> list2 = [];
            foreach (Trapezoid trapezoid in triangulator.Trapezoids)
            {
                Vertices vertices2 = [];
                List<Point> vertices3 = trapezoid.GetVertices();
                foreach (Point item in vertices3)
                {
                    vertices2.Add(new Vector2(item.X, item.Y));
                }
                list2.Add(vertices2);
            }
            return list2;
        }
    }
}
