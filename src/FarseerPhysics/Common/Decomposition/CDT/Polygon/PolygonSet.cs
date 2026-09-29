using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.CDT.Polygon
{
    internal sealed class PolygonSet
    {
        private readonly List<Polygon> _polygons = [];

        public IEnumerable<Polygon> Polygons => _polygons;

        public PolygonSet()
        {
        }

        public PolygonSet(Polygon poly)
        {
            _polygons.Add(poly);
        }

        public void Add(Polygon p)
        {
            _polygons.Add(p);
        }
    }
}
