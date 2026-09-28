using System;
using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel;

internal class Triangulator
{
    public List<Trapezoid> Trapezoids;

    public List<List<Point>> Triangles;

    private Trapezoid _boundingBox;

    private List<Edge> _edgeList;

    private QueryGraph _queryGraph;

    private float _sheer = 0.001f;

    private TrapezoidalMap _trapezoidalMap;

    private List<MonotoneMountain> _xMonoPoly;

    public Triangulator(List<Point> polyLine, float sheer)
    {
        _sheer = sheer;
        Triangles = new List<List<Point>>();
        Trapezoids = new List<Trapezoid>();
        _xMonoPoly = new List<MonotoneMountain>();
        _edgeList = InitEdges(polyLine);
        _trapezoidalMap = new TrapezoidalMap();
        _boundingBox = _trapezoidalMap.BoundingBox(_edgeList);
        _queryGraph = new QueryGraph(Sink.Isink(_boundingBox));
        Process();
    }

    private void Process()
    {
        foreach (Edge edge in _edgeList)
        {
            List<Trapezoid> list = _queryGraph.FollowEdge(edge);
            foreach (Trapezoid item2 in list)
            {
                _trapezoidalMap.Map.Remove(item2);
                bool flag = item2.Contains(edge.P);
                bool flag2 = item2.Contains(edge.Q);
                Trapezoid[] array;
                if (flag && flag2)
                {
                    array = _trapezoidalMap.Case1(item2, edge);
                    _queryGraph.Case1(item2.Sink, edge, array);
                }
                else if (flag && !flag2)
                {
                    array = _trapezoidalMap.Case2(item2, edge);
                    _queryGraph.Case2(item2.Sink, edge, array);
                }
                else if (!flag && !flag2)
                {
                    array = _trapezoidalMap.Case3(item2, edge);
                    _queryGraph.Case3(item2.Sink, edge, array);
                }
                else
                {
                    array = _trapezoidalMap.Case4(item2, edge);
                    _queryGraph.Case4(item2.Sink, edge, array);
                }
                Trapezoid[] array2 = array;
                foreach (Trapezoid item in array2)
                {
                    _trapezoidalMap.Map.Add(item);
                }
            }
            _trapezoidalMap.Clear();
        }
        foreach (Trapezoid item3 in _trapezoidalMap.Map)
        {
            MarkOutside(item3);
        }
        foreach (Trapezoid item4 in _trapezoidalMap.Map)
        {
            if (item4.Inside)
            {
                Trapezoids.Add(item4);
                item4.AddPoints();
            }
        }
        CreateMountains();
    }

    private void CreateMountains()
    {
        foreach (Edge edge in _edgeList)
        {
            if (edge.MPoints.Count <= 2)
            {
                continue;
            }
            MonotoneMountain monotoneMountain = new MonotoneMountain();
            List<Point> list = new List<Point>(edge.MPoints);
            list.Sort((Point p1, Point p2) => p1.X.CompareTo(p2.X));
            foreach (Point item in list)
            {
                monotoneMountain.Add(item);
            }
            monotoneMountain.Process();
            foreach (List<Point> triangle in monotoneMountain.Triangles)
            {
                Triangles.Add(triangle);
            }
            _xMonoPoly.Add(monotoneMountain);
        }
    }

    private void MarkOutside(Trapezoid t)
    {
        if (t.Top == _boundingBox.Top || t.Bottom == _boundingBox.Bottom)
        {
            t.TrimNeighbors();
        }
    }

    private List<Edge> InitEdges(List<Point> points)
    {
        List<Edge> list = new List<Edge>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            list.Add(new Edge(points[i], points[i + 1]));
        }
        list.Add(new Edge(points[0], points[points.Count - 1]));
        return OrderSegments(list);
    }

    private List<Edge> OrderSegments(List<Edge> edgeInput)
    {
        List<Edge> list = new List<Edge>();
        foreach (Edge item in edgeInput)
        {
            Point point = ShearTransform(item.P);
            Point point2 = ShearTransform(item.Q);
            if (point.X > point2.X)
            {
                list.Add(new Edge(point2, point));
            }
            else if (point.X < point2.X)
            {
                list.Add(new Edge(point, point2));
            }
        }
        Shuffle(list);
        return list;
    }

    private static void Shuffle<T>(IList<T> list)
    {
        Random random = new Random();
        int num = list.Count;
        while (num > 1)
        {
            num--;
            int index = random.Next(num + 1);
            T value = list[index];
            list[index] = list[num];
            list[num] = value;
        }
    }

    private Point ShearTransform(Point point)
    {
        return new Point(point.X + _sheer * point.Y, point.Y);
    }
}
