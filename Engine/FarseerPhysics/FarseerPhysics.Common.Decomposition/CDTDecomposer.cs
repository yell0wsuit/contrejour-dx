using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition.CDT;
using FarseerPhysics.Common.Decomposition.CDT.Delaunay;
using FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;
using FarseerPhysics.Common.Decomposition.CDT.Polygon;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Common.Decomposition;

internal static class CDTDecomposer
{
	public static List<Vertices> ConvexPartition(Vertices vertices)
	{
		Polygon polygon = new Polygon();
		foreach (Vector2 vertex in vertices)
		{
			polygon.Points.Add(new TriangulationPoint(vertex.X, vertex.Y));
		}
		if (vertices.Holes != null)
		{
			foreach (Vertices hole in vertices.Holes)
			{
				Polygon polygon2 = new Polygon();
				foreach (Vector2 item in hole)
				{
					polygon2.Points.Add(new TriangulationPoint(item.X, item.Y));
				}
				polygon.AddHole(polygon2);
			}
		}
		DTSweepContext dTSweepContext = new DTSweepContext();
		dTSweepContext.PrepareTriangulation(polygon);
		DTSweep.Triangulate(dTSweepContext);
		List<Vertices> list = new List<Vertices>();
		foreach (DelaunayTriangle triangle in polygon.Triangles)
		{
			Vertices vertices2 = new Vertices();
			foreach (TriangulationPoint point in triangle.Points)
			{
				vertices2.Add(new Vector2((float)point.X, (float)point.Y));
			}
			list.Add(vertices2);
		}
		return list;
	}
}
