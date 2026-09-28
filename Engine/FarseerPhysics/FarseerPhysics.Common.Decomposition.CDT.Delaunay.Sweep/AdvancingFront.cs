using System;
using System.Text;

namespace FarseerPhysics.Common.Decomposition.CDT.Delaunay.Sweep;

internal sealed class AdvancingFront
{
    public AdvancingFrontNode Head;

    private AdvancingFrontNode Search;

    public AdvancingFrontNode Tail;

    public AdvancingFront(AdvancingFrontNode head, AdvancingFrontNode tail)
    {
        Head = head;
        Tail = tail;
        Search = head;
        AddNode(head);
        AddNode(tail);
    }

    public static void AddNode(AdvancingFrontNode node)
    {
    }

    public static void RemoveNode(AdvancingFrontNode node)
    {
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        for (AdvancingFrontNode advancingFrontNode = Head; advancingFrontNode != Tail; advancingFrontNode = advancingFrontNode.Next)
        {
            _ = stringBuilder.Append(advancingFrontNode.Point.X).Append("->");
        }
        _ = stringBuilder.Append(Tail.Point.X);
        return stringBuilder.ToString();
    }

    private AdvancingFrontNode FindSearchNode()
    {
        return Search;
    }

    public AdvancingFrontNode LocateNode(TriangulationPoint point)
    {
        return LocateNode(point.X);
    }

    private AdvancingFrontNode LocateNode(double x)
    {
        AdvancingFrontNode advancingFrontNode = FindSearchNode();
        if (x < advancingFrontNode.Value)
        {
            while ((advancingFrontNode = advancingFrontNode.Prev) != null)
            {
                if (x >= advancingFrontNode.Value)
                {
                    Search = advancingFrontNode;
                    return advancingFrontNode;
                }
            }
        }
        else
        {
            while ((advancingFrontNode = advancingFrontNode.Next) != null)
            {
                if (x < advancingFrontNode.Value)
                {
                    Search = advancingFrontNode.Prev;
                    return advancingFrontNode.Prev;
                }
            }
        }
        return null;
    }

    public AdvancingFrontNode LocatePoint(TriangulationPoint point)
    {
        double x = point.X;
        AdvancingFrontNode advancingFrontNode = FindSearchNode();
        double x2 = advancingFrontNode.Point.X;
        if (x == x2)
        {
            if (point != advancingFrontNode.Point)
            {
                if (point == advancingFrontNode.Prev.Point)
                {
                    advancingFrontNode = advancingFrontNode.Prev;
                }
                else
                {
                    if (point != advancingFrontNode.Next.Point)
                    {
                        throw new InvalidOperationException("Failed to find Node for given afront point");
                    }
                    advancingFrontNode = advancingFrontNode.Next;
                }
            }
        }
        else if (x < x2)
        {
            while ((advancingFrontNode = advancingFrontNode.Prev) != null && point != advancingFrontNode.Point)
            {
            }
        }
        else
        {
            while ((advancingFrontNode = advancingFrontNode.Next) != null && point != advancingFrontNode.Point)
            {
            }
        }
        Search = advancingFrontNode;
        return advancingFrontNode;
    }
}
