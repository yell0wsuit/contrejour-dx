namespace FarseerPhysics.Common.Decomposition.CDT.Polygon
{
    internal sealed class PolygonPoint(double x, double y) : TriangulationPoint(x, y)
    {
        public PolygonPoint Next { get; set; }

        public PolygonPoint Previous { get; set; }
    }
}
