namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal sealed class DTSweepContext : TriangulationContext
{
    public sealed class DTSweepBasin
    {
        public AdvancingFrontNode bottomNode;

        public bool leftHighest;

        public AdvancingFrontNode leftNode;

        public AdvancingFrontNode rightNode;

        public double width;
    }

    public sealed class DTSweepEdgeEvent
    {
        public DTSweepConstraint ConstrainedEdge;

        public bool Right;
    }

    private const float ALPHA = 0.3f;

    public DTSweepBasin Basin = new();

    public DTSweepEdgeEvent EdgeEvent = new();

    private DTSweepPointComparator _comparator = new();

    public AdvancingFront aFront;

    public TriangulationPoint Head { get; set; }

    public TriangulationPoint Tail { get; set; }

    public DTSweepContext()
    {
        Clear();
    }

    public void RemoveFromList(DelaunayTriangle triangle)
    {
        _ = Triangles.Remove(triangle);
    }

    public void MeshClean(DelaunayTriangle triangle)
    {
        MeshCleanReq(triangle);
    }

    private void MeshCleanReq(DelaunayTriangle triangle)
    {
        if (triangle == null || triangle.IsInterior)
        {
            return;
        }
        triangle.IsInterior = true;
        ITriangulatable.AddTriangle(triangle);
        for (int i = 0; i < 3; i++)
        {
            if (!triangle.EdgeIsConstrained[i])
            {
                MeshCleanReq(triangle.Neighbors[i]);
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        Triangles.Clear();
    }

    public void AddNode(AdvancingFrontNode node)
    {
        AdvancingFront.AddNode(node);
    }

    public void RemoveNode(AdvancingFrontNode node)
    {
        AdvancingFront.RemoveNode(node);
    }

    public AdvancingFrontNode LocateNode(TriangulationPoint point)
    {
        return aFront.LocateNode(point);
    }

    public void CreateAdvancingFront()
    {
        DelaunayTriangle delaunayTriangle = new(Points[0], Tail, Head);
        Triangles.Add(delaunayTriangle);
        AdvancingFrontNode advancingFrontNode = new(delaunayTriangle.Points[1])
        {
            Triangle = delaunayTriangle
        };
        AdvancingFrontNode advancingFrontNode2 = new(delaunayTriangle.Points[0])
        {
            Triangle = delaunayTriangle
        };
        AdvancingFrontNode tail = new(delaunayTriangle.Points[2]);
        aFront = new AdvancingFront(advancingFrontNode, tail);
        AdvancingFront.AddNode(advancingFrontNode2);
        aFront.Head.Next = advancingFrontNode2;
        advancingFrontNode2.Next = aFront.Tail;
        advancingFrontNode2.Prev = aFront.Head;
        aFront.Tail.Prev = advancingFrontNode2;
    }

    public void MapTriangleToNodes(DelaunayTriangle t)
    {
        for (int i = 0; i < 3; i++)
        {
            if (t.Neighbors[i] == null)
            {
                AdvancingFrontNode advancingFrontNode = aFront.LocatePoint(t.PointCW(t.Points[i]));
                advancingFrontNode?.Triangle = t;
            }
        }
    }

    public override void PrepareTriangulation(ITriangulatable t)
    {
        base.PrepareTriangulation(t);
        double x;
        double num = x = Points[0].X;
        double y;
        double num2 = y = Points[0].Y;
        foreach (TriangulationPoint point in Points)
        {
            if (point.X > num)
            {
                num = point.X;
            }
            if (point.X < x)
            {
                x = point.X;
            }
            if (point.Y > num2)
            {
                num2 = point.Y;
            }
            if (point.Y < y)
            {
                y = point.Y;
            }
        }
        double num3 = 0.30000001192092896 * (num - x);
        double num4 = 0.30000001192092896 * (num2 - y);
        TriangulationPoint head = new(num + num3, y - num4);
        TriangulationPoint tail = new(x - num3, y - num4);
        Head = head;
        Tail = tail;
        Points.Sort(_comparator);
    }

    public void FinalizeTriangulation()
    {
        ITriangulatable.AddTriangles(Triangles);
        Triangles.Clear();
    }

    public override TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b)
    {
        return new DTSweepConstraint(a, b);
    }
}
