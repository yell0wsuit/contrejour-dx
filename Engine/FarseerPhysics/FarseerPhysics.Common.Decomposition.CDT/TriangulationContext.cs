using System.Collections.Generic;
using FarseerPhysics.Common.Decomposition.CDT.Delaunay;

namespace FarseerPhysics.Common.Decomposition.CDT;

internal abstract class TriangulationContext
{
	public readonly List<TriangulationPoint> Points = new List<TriangulationPoint>(200);

	public readonly List<DelaunayTriangle> Triangles = new List<DelaunayTriangle>();

	private int _stepTime = -1;

	public TriangulationMode TriangulationMode { get; protected set; }

	public Triangulatable Triangulatable { get; private set; }

	public bool WaitUntilNotified { get; private set; }

	public bool Terminated { get; set; }

	public int StepCount { get; private set; }

	public virtual bool IsDebugEnabled { get; protected set; }

	public TriangulationContext()
	{
		Terminated = false;
	}

	public void Done()
	{
		StepCount++;
	}

	public virtual void PrepareTriangulation(Triangulatable t)
	{
		Triangulatable = t;
		TriangulationMode = t.TriangulationMode;
		t.PrepareTriangulation(this);
	}

	public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

	public void Update(string message)
	{
	}

	public virtual void Clear()
	{
		Points.Clear();
		Terminated = false;
		StepCount = 0;
	}
}
