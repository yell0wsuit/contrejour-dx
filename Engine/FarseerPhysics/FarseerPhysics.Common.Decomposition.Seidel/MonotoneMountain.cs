using System;
using System.Collections.Generic;

namespace FarseerPhysics.Common.Decomposition.Seidel;

internal class MonotoneMountain
{
    private const float PiSlop = 3.1f;

    public List<List<Point>> Triangles;

    private HashSet<Point> _convexPoints;

    private Point _head;

    private List<Point> _monoPoly;

    private bool _positive;

    private int _size;

    private Point _tail;

    public MonotoneMountain()
    {
        _size = 0;
        _tail = null;
        _head = null;
        _positive = false;
        _convexPoints = new HashSet<Point>();
        _monoPoly = new List<Point>();
        Triangles = new List<List<Point>>();
    }

    public void Add(Point point)
    {
        if (_size == 0)
        {
            _head = point;
            _size = 1;
        }
        else if (_size == 1)
        {
            _tail = point;
            _tail.Prev = _head;
            _head.Next = _tail;
            _size = 2;
        }
        else
        {
            _tail.Next = point;
            point.Prev = _tail;
            _tail = point;
            _size++;
        }
    }

    public void Remove(Point point)
    {
        Point next = point.Next;
        Point prev = point.Prev;
        point.Prev.Next = next;
        point.Next.Prev = prev;
        _size--;
    }

    public void Process()
    {
        _positive = AngleSign();
        GenMonoPoly();
        Point next = _head.Next;
        while (next.Neq(_tail))
        {
            float num = Angle(next);
            if (num >= 3.1f || num <= -3.1f || num == 0f)
            {
                Remove(next);
            }
            else if (IsConvex(next))
            {
                _convexPoints.Add(next);
            }
            next = next.Next;
        }
        Triangulate();
    }

    private void Triangulate()
    {
        while (_convexPoints.Count != 0)
        {
            IEnumerator<Point> enumerator = _convexPoints.GetEnumerator();
            enumerator.MoveNext();
            Point current = enumerator.Current;
            _convexPoints.Remove(current);
            Point prev = current.Prev;
            Point item = current;
            Point next = current.Next;
            List<Point> list = new List<Point>(3);
            list.Add(prev);
            list.Add(item);
            list.Add(next);
            Triangles.Add(list);
            Remove(current);
            if (Valid(prev))
            {
                _convexPoints.Add(prev);
            }
            if (Valid(next))
            {
                _convexPoints.Add(next);
            }
        }
    }

    private bool Valid(Point p)
    {
        if (p.Neq(_head) && p.Neq(_tail))
        {
            return IsConvex(p);
        }
        return false;
    }

    private void GenMonoPoly()
    {
        for (Point point = _head; point != null; point = point.Next)
        {
            _monoPoly.Add(point);
        }
    }

    private float Angle(Point p)
    {
        Point point = p.Next - p;
        Point p2 = p.Prev - p;
        return (float)Math.Atan2(point.Cross(p2), point.Dot(p2));
    }

    private bool AngleSign()
    {
        Point point = _head.Next - _head;
        Point p = _tail - _head;
        return Math.Atan2(point.Cross(p), point.Dot(p)) >= 0.0;
    }

    private bool IsConvex(Point p)
    {
        if (_positive != Angle(p) >= 0f)
        {
            return false;
        }
        return true;
    }
}
