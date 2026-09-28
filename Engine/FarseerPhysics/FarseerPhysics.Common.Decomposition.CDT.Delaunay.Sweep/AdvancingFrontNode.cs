namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal sealed class AdvancingFrontNode
{
    public AdvancingFrontNode Next;

    public TriangulationPoint Point;

    public AdvancingFrontNode Prev;

    public DelaunayTriangle Triangle;

    public double Value;

    public bool HasNext => Next != null;

    public bool HasPrev => Prev != null;

    public AdvancingFrontNode(TriangulationPoint point)
    {
        Point = point;
        Value = point.X;
    }
}
