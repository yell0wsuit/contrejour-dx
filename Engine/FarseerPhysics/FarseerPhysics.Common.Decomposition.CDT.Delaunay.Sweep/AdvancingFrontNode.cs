namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal sealed class AdvancingFrontNode(TriangulationPoint point)
{
    public AdvancingFrontNode Next;

    public TriangulationPoint Point = point;

    public AdvancingFrontNode Prev;

    public DelaunayTriangle Triangle;

    public double Value = point.X;

    public bool HasNext => Next != null;

    public bool HasPrev => Prev != null;
}
