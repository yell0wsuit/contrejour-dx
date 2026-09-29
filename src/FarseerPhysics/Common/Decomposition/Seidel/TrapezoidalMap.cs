using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel
{
    internal sealed class TrapezoidalMap
    {
        public HashSet<Trapezoid> Map;

        private Edge _bCross;

        private Edge _cross;

        private readonly float _margin;

        public TrapezoidalMap()
        {
            Map = [];
            _margin = 50f;
            _bCross = null;
            _cross = null;
        }

        public void Clear()
        {
            _bCross = null;
            _cross = null;
        }

        public static Trapezoid[] Case1(Trapezoid t, Edge e)
        {
            Trapezoid[] array =
            [
                new(t.LeftPoint, e.P, t.Top, t.Bottom),
                new(e.P, e.Q, t.Top, e),
                new(e.P, e.Q, e, t.Bottom),
                new(e.Q, t.RightPoint, t.Top, t.Bottom)
            ];
            array[0].UpdateLeft(t.UpperLeft, t.LowerLeft);
            array[1].UpdateLeftRight(array[0], null, array[3], null);
            array[2].UpdateLeftRight(null, array[0], null, array[3]);
            array[3].UpdateRight(t.UpperRight, t.LowerRight);
            return array;
        }

        public Trapezoid[] Case2(Trapezoid t, Edge e)
        {
            Point rightPoint = (e.Q.X != t.RightPoint.X) ? t.RightPoint : e.Q;
            Trapezoid[] array =
            [
                new(t.LeftPoint, e.P, t.Top, t.Bottom),
                new(e.P, rightPoint, t.Top, e),
                new(e.P, rightPoint, e, t.Bottom)
            ];
            array[0].UpdateLeft(t.UpperLeft, t.LowerLeft);
            array[1].UpdateLeftRight(array[0], null, t.UpperRight, null);
            array[2].UpdateLeftRight(null, array[0], null, t.LowerRight);
            _bCross = t.Bottom;
            _cross = t.Top;
            e.Above = array[1];
            e.Below = array[2];
            return array;
        }

        public Trapezoid[] Case3(Trapezoid t, Edge e)
        {
            Point leftPoint = (e.P.X != t.LeftPoint.X) ? t.LeftPoint : e.P;
            Point rightPoint = (e.Q.X != t.RightPoint.X) ? t.RightPoint : e.Q;
            Trapezoid[] array = new Trapezoid[2];
            if (_cross == t.Top)
            {
                array[0] = t.UpperLeft;
                array[0].UpdateRight(t.UpperRight, null);
                array[0].RightPoint = rightPoint;
            }
            else
            {
                array[0] = new Trapezoid(leftPoint, rightPoint, t.Top, e);
                array[0].UpdateLeftRight(t.UpperLeft, e.Above, t.UpperRight, null);
            }
            if (_bCross == t.Bottom)
            {
                array[1] = t.LowerLeft;
                array[1].UpdateRight(null, t.LowerRight);
                array[1].RightPoint = rightPoint;
            }
            else
            {
                array[1] = new Trapezoid(leftPoint, rightPoint, e, t.Bottom);
                array[1].UpdateLeftRight(e.Below, t.LowerLeft, null, t.LowerRight);
            }
            _bCross = t.Bottom;
            _cross = t.Top;
            e.Above = array[0];
            e.Below = array[1];
            return array;
        }

        public Trapezoid[] Case4(Trapezoid t, Edge e)
        {
            Point leftPoint = (e.P.X != t.LeftPoint.X) ? t.LeftPoint : e.P;
            Trapezoid[] array = new Trapezoid[3];
            if (_cross == t.Top)
            {
                array[0] = t.UpperLeft;
                array[0].RightPoint = e.Q;
            }
            else
            {
                array[0] = new Trapezoid(leftPoint, e.Q, t.Top, e);
                array[0].UpdateLeft(t.UpperLeft, e.Above);
            }
            if (_bCross == t.Bottom)
            {
                array[1] = t.LowerLeft;
                array[1].RightPoint = e.Q;
            }
            else
            {
                array[1] = new Trapezoid(leftPoint, e.Q, e, t.Bottom);
                array[1].UpdateLeft(e.Below, t.LowerLeft);
            }
            array[2] = new Trapezoid(e.Q, t.RightPoint, t.Top, t.Bottom);
            array[2].UpdateLeftRight(array[0], array[1], t.UpperRight, t.LowerRight);
            return array;
        }

        public Trapezoid BoundingBox(List<Edge> edges)
        {
            Point point = edges[0].P + _margin;
            Point point2 = edges[0].Q - _margin;
            foreach (Edge edge3 in edges)
            {
                if (edge3.P.X > point.X)
                {
                    point = new Point(edge3.P.X + _margin, point.Y);
                }
                if (edge3.P.Y > point.Y)
                {
                    point = new Point(point.X, edge3.P.Y + _margin);
                }
                if (edge3.Q.X > point.X)
                {
                    point = new Point(edge3.Q.X + _margin, point.Y);
                }
                if (edge3.Q.Y > point.Y)
                {
                    point = new Point(point.X, edge3.Q.Y + _margin);
                }
                if (edge3.P.X < point2.X)
                {
                    point2 = new Point(edge3.P.X - _margin, point2.Y);
                }
                if (edge3.P.Y < point2.Y)
                {
                    point2 = new Point(point2.X, edge3.P.Y - _margin);
                }
                if (edge3.Q.X < point2.X)
                {
                    point2 = new Point(edge3.Q.X - _margin, point2.Y);
                }
                if (edge3.Q.Y < point2.Y)
                {
                    point2 = new Point(point2.X, edge3.Q.Y - _margin);
                }
            }
            Edge edge = new(new Point(point2.X, point.Y), new Point(point.X, point.Y));
            Edge edge2 = new(new Point(point2.X, point2.Y), new Point(point.X, point2.Y));
            Point p = edge2.P;
            Point q = edge.Q;
            return new Trapezoid(p, q, edge, edge2);
        }
    }
}
