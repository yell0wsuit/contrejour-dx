using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal class DTSweepPointComparator : IComparer<TriangulationPoint>
{
    public int Compare(TriangulationPoint p1, TriangulationPoint p2)
    {
        if (p1.Y < p2.Y)
        {
            return -1;
        }
        if (p1.Y > p2.Y)
        {
            return 1;
        }
        return p1.X < p2.X ? -1 : p1.X > p2.X ? 1 : 0;
    }
}
