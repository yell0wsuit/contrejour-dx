using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal class DTSweepPointComparator : IComparer<TriangulationPoint>
{
    public int Compare(TriangulationPoint p1, TriangulationPoint p2)
    {
        return p1.Y < p2.Y ? -1 : p1.Y > p2.Y ? 1 : p1.X < p2.X ? -1 : p1.X > p2.X ? 1 : 0;
    }
}
