using System.Collections.Generic;

using FarseerPhysics.Common.Decomposition.CDT.Delaunay;

namespace FarseerPhysics.Common.Decomposition.CDT.Sets
{
    internal class PointSet(List<TriangulationPoint> points) : ITriangulatable
    {
        public IList<TriangulationPoint> Points { get; private set; } = [.. points];

        public IList<DelaunayTriangle> Triangles { get; private set; }

        public virtual TriangulationMode TriangulationMode => TriangulationMode.Unconstrained;

        public void AddTriangle(DelaunayTriangle t)
        {
            Triangles.Add(t);
        }

        public void AddTriangles(IEnumerable<DelaunayTriangle> list)
        {
            foreach (DelaunayTriangle item in list)
            {
                Triangles.Add(item);
            }
        }

        public void ClearTriangles()
        {
            Triangles.Clear();
        }

        public virtual void PrepareTriangulation(TriangulationContext tcx)
        {
            if (Triangles == null)
            {
                Triangles = new List<DelaunayTriangle>(Points.Count);
            }
            else
            {
                Triangles.Clear();
            }
            tcx.Points.AddRange(Points);
        }
    }
}
