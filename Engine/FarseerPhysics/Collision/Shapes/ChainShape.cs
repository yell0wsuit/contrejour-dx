using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision.Shapes;

public class ChainShape : Shape
{
    public Vertices Vertices { get; set; }

    private Vector2 _prevVertex;

    private Vector2 _nextVertex;

    private bool _hasPrevVertex;

    private bool _hasNextVertex;

    private static readonly EdgeShape _edgeShape = new();

    public override int ChildCount => Vertices.Count - 1;

    public Vector2 PrevVertex
    {
        get => _prevVertex;
        set
        {
            _prevVertex = value;
            _hasPrevVertex = true;
        }
    }

    public Vector2 NextVertex
    {
        get => _nextVertex;
        set
        {
            _nextVertex = value;
            _hasNextVertex = true;
        }
    }

    public ChainShape()
        : base(0f)
    {
        ShapeType = ShapeType.Chain;
        _radius = 0.01f;
    }

    public ChainShape(Vertices vertices, bool createLoop = false)
        : base(0f)
    {
        ShapeType = ShapeType.Chain;
        _radius = 0.01f;
        for (int i = 1; i < vertices.Count; i++)
        {
            _ = vertices[i - 1];
            _ = vertices[i];
        }
        Vertices = [.. vertices];
        if (createLoop)
        {
            Vertices.Add(vertices[0]);
            PrevVertex = Vertices[^2];
            NextVertex = Vertices[1];
        }
    }

    internal void GetChildEdge(EdgeShape edge, int index)
    {
        edge.ShapeType = ShapeType.Edge;
        edge._radius = _radius;
        edge.Vertex1 = Vertices[index];
        edge.Vertex2 = Vertices[index + 1];
        if (index > 0)
        {
            edge.Vertex0 = Vertices[index - 1];
            edge.HasVertex0 = true;
        }
        else
        {
            edge.Vertex0 = _prevVertex;
            edge.HasVertex0 = _hasPrevVertex;
        }
        if (index < Vertices.Count - 2)
        {
            edge.Vertex3 = Vertices[index + 2];
            edge.HasVertex3 = true;
        }
        else
        {
            edge.Vertex3 = _nextVertex;
            edge.HasVertex3 = _hasNextVertex;
        }
    }

    public EdgeShape GetChildEdge(int index)
    {
        EdgeShape edgeShape = new();
        GetChildEdge(edgeShape, index);
        return edgeShape;
    }

    public override bool TestPoint(ref Transform transform, ref Vector2 point)
    {
        return false;
    }

    public override bool RayCast(out RayCastOutput output, ref RayCastInput input, ref Transform transform, int childIndex)
    {
        int num = childIndex + 1;
        if (num == Vertices.Count)
        {
            num = 0;
        }
        _edgeShape.Vertex1 = Vertices[childIndex];
        _edgeShape.Vertex2 = Vertices[num];
        return _edgeShape.RayCast(out output, ref input, ref transform, 0);
    }

    public override void ComputeAABB(out AABB aabb, ref Transform transform, int childIndex)
    {
        int num = childIndex + 1;
        if (num == Vertices.Count)
        {
            num = 0;
        }
        Vector2 value = MathUtils.Mul(ref transform, Vertices[childIndex]);
        Vector2 value2 = MathUtils.Mul(ref transform, Vertices[num]);
        aabb.LowerBound = Vector2.Min(value, value2);
        aabb.UpperBound = Vector2.Max(value, value2);
    }

    protected override void ComputeProperties()
    {
    }

    public override float ComputeSubmergedArea(ref Vector2 normal, float offset, ref Transform xf, out Vector2 sc)
    {
        sc = Vector2.Zero;
        return 0f;
    }

    public bool CompareTo(ChainShape shape)
    {
        if (Vertices.Count != shape.Vertices.Count)
        {
            return false;
        }
        for (int i = 0; i < Vertices.Count; i++)
        {
            if (Vertices[i] != shape.Vertices[i])
            {
                return false;
            }
        }
        return PrevVertex == shape.PrevVertex && NextVertex == shape.NextVertex;
    }

    public override Shape Clone()
    {
        ChainShape chainShape = new()
        {
            ShapeType = ShapeType,
            _density = _density,
            _radius = _radius,
            PrevVertex = _prevVertex,
            NextVertex = _nextVertex,
            _hasNextVertex = _hasNextVertex,
            _hasPrevVertex = _hasPrevVertex,
            Vertices = [.. Vertices],
            MassData = MassData
        };
        return chainShape;
    }
}
