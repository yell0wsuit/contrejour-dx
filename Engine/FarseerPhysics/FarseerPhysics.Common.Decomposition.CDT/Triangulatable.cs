using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition.CDT.Delaunay;

namespace FarseerPhysics.Common.Decomposition.CDT;

internal interface Triangulatable
{
	IList<TriangulationPoint> Points { get; }

	IList<DelaunayTriangle> Triangles { get; }

	TriangulationMode TriangulationMode { get; }

	void PrepareTriangulation(TriangulationContext tcx);

	void AddTriangle(DelaunayTriangle t);

	void AddTriangles(IEnumerable<DelaunayTriangle> list);

	void ClearTriangles();
}
