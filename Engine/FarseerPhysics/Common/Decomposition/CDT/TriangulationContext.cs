using System.Collections.Generic;

using FarseerPhysics.Common.Decomposition.CDT.Delaunay;

namespace FarseerPhysics.Common.Decomposition.CDT;

internal abstract class TriangulationContext
{
    public readonly List<TriangulationPoint> Points = new(200);

    public readonly List<DelaunayTriangle> Triangles = [];

    public TriangulationMode TriangulationMode { get; protected set; }

    public ITriangulatable ITriangulatable { get; private set; }

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

    public virtual void PrepareTriangulation(ITriangulatable t)
    {
        ITriangulatable = t;
        TriangulationMode = t.TriangulationMode;
        t.PrepareTriangulation(this);
    }

    public abstract TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b);

    public virtual void Clear()
    {
        Points.Clear();
        Terminated = false;
        StepCount = 0;
    }
}
