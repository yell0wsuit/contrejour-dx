using System.Collections.Generic;

using Mokus2D.Data;

namespace Mokus2D.Collections.QuadTree;

public class QuadTreeNode<T> where T : class, IQuadTreeObject<T>
{
    private static int _id;

    public readonly int ID = _id++;

    internal readonly QuadTreeNode<T>[] Nodes = new QuadTreeNode<T>[4];

    internal List<T> Objects = [];

    public QuadTreeNode<T> Parent { get; internal set; }

    public QuadTreeNode<T> this[QuadDirection direction]
    {
        get => direction switch
        {
            QuadDirection.NW => Nodes[0],
            QuadDirection.NE => Nodes[1],
            QuadDirection.SW => Nodes[2],
            QuadDirection.SE => Nodes[3],
            _ => null,
        };
        set
        {
            switch (direction)
            {
                case QuadDirection.NW:
                    Nodes[0] = value;
                    break;
                case QuadDirection.NE:
                    Nodes[1] = value;
                    break;
                case QuadDirection.SW:
                    Nodes[2] = value;
                    break;
                case QuadDirection.SE:
                    Nodes[3] = value;
                    break;
            }
            value?.Parent = this;
        }
    }

    public RectangleFloat Bounds { get; internal set; }

    public bool HasChildNodes()
    {
        return Nodes[0] != null;
    }

    public QuadTreeNode(RectangleFloat bounds)
    {
        Bounds = bounds;
    }

    public QuadTreeNode(float x, float y, float width, float height)
        : this(new RectangleFloat(x, y, width, height))
    {
    }
}
