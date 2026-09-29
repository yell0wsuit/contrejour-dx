using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel
{
    internal sealed class Trapezoid(Point leftPoint, Point rightPoint, Edge top, Edge bottom)
    {
        public Edge Bottom = bottom;

        public bool Inside = true;

        public Point LeftPoint = leftPoint;

        public Trapezoid LowerLeft;

        public Trapezoid LowerRight;

        public Point RightPoint = rightPoint;

        public Sink Sink;

        public Edge Top = top;

        public Trapezoid UpperLeft;

        public Trapezoid UpperRight;

        public void UpdateLeft(Trapezoid ul, Trapezoid ll)
        {
            UpperLeft = ul;
            ul?.UpperRight = this;
            LowerLeft = ll;
            ll?.LowerRight = this;
        }

        public void UpdateRight(Trapezoid ur, Trapezoid lr)
        {
            UpperRight = ur;
            ur?.UpperLeft = this;
            LowerRight = lr;
            lr?.LowerLeft = this;
        }

        public void UpdateLeftRight(Trapezoid ul, Trapezoid ll, Trapezoid ur, Trapezoid lr)
        {
            UpperLeft = ul;
            ul?.UpperRight = this;
            LowerLeft = ll;
            ll?.LowerRight = this;
            UpperRight = ur;
            ur?.UpperLeft = this;
            LowerRight = lr;
            lr?.LowerLeft = this;
        }

        public void TrimNeighbors()
        {
            if (Inside)
            {
                Inside = false;
                UpperLeft?.TrimNeighbors();
                LowerLeft?.TrimNeighbors();
                UpperRight?.TrimNeighbors();
                LowerRight?.TrimNeighbors();
            }
        }

        public bool Contains(Point point)
        {
            return point.X > LeftPoint.X && point.X < RightPoint.X && Top.IsAbove(point) && Bottom.IsBelow(point);
        }

        public List<Point> GetVertices()
        {
            List<Point> list =
            [
                LineIntersect(Top, LeftPoint.X),
                LineIntersect(Bottom, LeftPoint.X),
                LineIntersect(Bottom, RightPoint.X),
                LineIntersect(Top, RightPoint.X),
            ];
            return list;
        }

        private static Point LineIntersect(Edge edge, float x)
        {
            float y = (edge.Slope * x) + edge.B;
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
}
