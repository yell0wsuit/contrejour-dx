using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel;

internal sealed class Edge
{
    public Trapezoid Above;

    public float B;

    public Trapezoid Below;

    public HashSet<Point> MPoints;

    public Point P;

    public Point Q;

    public float Slope;

    public Edge(Point p, Point q)
    {
        P = p;
        Q = q;
        Slope = q.X - p.X != 0f ? (q.Y - p.Y) / (q.X - p.X) : 0f;
        B = p.Y - (p.X * Slope);
        Above = null;
        Below = null;
        MPoints = [p, q];
    }

    public bool IsAbove(Point point)
    {
        return P.Orient2D(Q, point) < 0f;
    }

    public bool IsBelow(Point point)
    {
        return P.Orient2D(Q, point) > 0f;
    }

    public void AddMpoint(Point point)
    {
        foreach (Point mPoint in MPoints)
        {
            if (!mPoint.Neq(point))
            {
                return;
            }
        }
        _ = MPoints.Add(point);
    }
}
