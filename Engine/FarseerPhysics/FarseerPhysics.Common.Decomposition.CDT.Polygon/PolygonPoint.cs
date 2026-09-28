namespace FarseerPhysics.Common.Decomposition.CDT.Polygon;

internal class PolygonPoint : TriangulationPoint
{
    public PolygonPoint Next { get; set; }

    public PolygonPoint Previous { get; set; }

    public PolygonPoint(double x, double y)
        : base(x, y)
    {
    }
}
