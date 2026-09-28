namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal class DTSweepContext : TriangulationContext
{
    public class DTSweepBasin
    {
        public AdvancingFrontNode bottomNode;

        public bool leftHighest;

        public AdvancingFrontNode leftNode;

        public AdvancingFrontNode rightNode;

        public double width;
    }

    public class DTSweepEdgeEvent
    {
        public DTSweepConstraint ConstrainedEdge;

        public bool Right;
    }

    private const float ALPHA = 0.3f;

    public DTSweepBasin Basin = new DTSweepBasin();

    public DTSweepEdgeEvent EdgeEvent = new DTSweepEdgeEvent();

    private DTSweepPointComparator _comparator = new DTSweepPointComparator();

    public AdvancingFront aFront;

    public TriangulationPoint Head { get; set; }

    public TriangulationPoint Tail { get; set; }

    public DTSweepContext()
    {
        Clear();
    }

    public void RemoveFromList(DelaunayTriangle triangle)
    {
        Triangles.Remove(triangle);
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
        base.Triangulatable.AddTriangle(triangle);
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
        aFront.AddNode(node);
    }

    public void RemoveNode(AdvancingFrontNode node)
    {
        aFront.RemoveNode(node);
    }

    public AdvancingFrontNode LocateNode(TriangulationPoint point)
    {
        return aFront.LocateNode(point);
    }

    public void CreateAdvancingFront()
    {
        DelaunayTriangle delaunayTriangle = new DelaunayTriangle(Points[0], Tail, Head);
        Triangles.Add(delaunayTriangle);
        AdvancingFrontNode advancingFrontNode = new AdvancingFrontNode(delaunayTriangle.Points[1]);
        advancingFrontNode.Triangle = delaunayTriangle;
        AdvancingFrontNode advancingFrontNode2 = new AdvancingFrontNode(delaunayTriangle.Points[0]);
        advancingFrontNode2.Triangle = delaunayTriangle;
        AdvancingFrontNode tail = new AdvancingFrontNode(delaunayTriangle.Points[2]);
        aFront = new AdvancingFront(advancingFrontNode, tail);
        aFront.AddNode(advancingFrontNode2);
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
                if (advancingFrontNode != null)
                {
                    advancingFrontNode.Triangle = t;
                }
            }
        }
    }

    public override void PrepareTriangulation(Triangulatable t)
    {
        base.PrepareTriangulation(t);
        double x;
        double num = (x = Points[0].X);
        double y;
        double num2 = (y = Points[0].Y);
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
        TriangulationPoint head = new TriangulationPoint(num + num3, y - num4);
        TriangulationPoint tail = new TriangulationPoint(x - num3, y - num4);
        Head = head;
        Tail = tail;
        Points.Sort(_comparator);
    }

    public void FinalizeTriangulation()
    {
        base.Triangulatable.AddTriangles(Triangles);
        Triangles.Clear();
    }

    public override TriangulationConstraint NewConstraint(TriangulationPoint a, TriangulationPoint b)
    {
        return new DTSweepConstraint(a, b);
    }
}
