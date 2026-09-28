using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel;

internal class Trapezoid
{
    public Edge Bottom;

    public bool Inside;

    public Point LeftPoint;

    public Trapezoid LowerLeft;

    public Trapezoid LowerRight;

    public Point RightPoint;

    public Sink Sink;

    public Edge Top;

    public Trapezoid UpperLeft;

    public Trapezoid UpperRight;

    public Trapezoid(Point leftPoint, Point rightPoint, Edge top, Edge bottom)
    {
        LeftPoint = leftPoint;
        RightPoint = rightPoint;
        Top = top;
        Bottom = bottom;
        UpperLeft = null;
        UpperRight = null;
        LowerLeft = null;
        LowerRight = null;
        Inside = true;
        Sink = null;
    }

    public void UpdateLeft(Trapezoid ul, Trapezoid ll)
    {
        UpperLeft = ul;
        if (ul != null)
        {
            ul.UpperRight = this;
        }
        LowerLeft = ll;
        if (ll != null)
        {
            ll.LowerRight = this;
        }
    }

    public void UpdateRight(Trapezoid ur, Trapezoid lr)
    {
        UpperRight = ur;
        if (ur != null)
        {
            ur.UpperLeft = this;
        }
        LowerRight = lr;
        if (lr != null)
        {
            lr.LowerLeft = this;
        }
    }

    public void UpdateLeftRight(Trapezoid ul, Trapezoid ll, Trapezoid ur, Trapezoid lr)
    {
        UpperLeft = ul;
        if (ul != null)
        {
            ul.UpperRight = this;
        }
        LowerLeft = ll;
        if (ll != null)
        {
            ll.LowerRight = this;
        }
        UpperRight = ur;
        if (ur != null)
        {
            ur.UpperLeft = this;
        }
        LowerRight = lr;
        if (lr != null)
        {
            lr.LowerLeft = this;
        }
    }

    public void TrimNeighbors()
    {
        if (Inside)
        {
            Inside = false;
            if (UpperLeft != null)
            {
                UpperLeft.TrimNeighbors();
            }
            if (LowerLeft != null)
            {
                LowerLeft.TrimNeighbors();
            }
            if (UpperRight != null)
            {
                UpperRight.TrimNeighbors();
            }
            if (LowerRight != null)
            {
                LowerRight.TrimNeighbors();
            }
        }
    }

    public bool Contains(Point point)
    {
        if (point.X > LeftPoint.X && point.X < RightPoint.X && Top.IsAbove(point))
        {
            return Bottom.IsBelow(point);
        }
        return false;
    }

    public List<Point> GetVertices()
    {
        List<Point> list = new List<Point>(4);
        list.Add(LineIntersect(Top, LeftPoint.X));
        list.Add(LineIntersect(Bottom, LeftPoint.X));
        list.Add(LineIntersect(Bottom, RightPoint.X));
        list.Add(LineIntersect(Top, RightPoint.X));
        return list;
    }

    private Point LineIntersect(Edge edge, float x)
    {
        float y = edge.Slope * x + edge.B;
        return new Point(x, y);
    }

    public void AddPoints()
    {
        if (LeftPoint != Bottom.P)
        {
            Bottom.AddMpoint(LeftPoint);
        }
        if (RightPoint != Bottom.Q)
        {
            Bottom.AddMpoint(RightPoint);
        }
        if (LeftPoint != Top.P)
        {
            Top.AddMpoint(LeftPoint);
        }
        if (RightPoint != Top.Q)
        {
            Top.AddMpoint(RightPoint);
        }
    }
}
