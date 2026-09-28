using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

using Microsoft.Xna.Framework;

namespace FarseerPhysics.Collision;

public class DistanceProxy
{
    internal float Radius;

    internal Vertices Vertices = [];

    public void Set(Shape shape, int index)
    {
        switch (shape.ShapeType)
        {
            case ShapeType.Circle:
                {
                    CircleShape circleShape = (CircleShape)shape;
                    Vertices.Clear();
                    Vertices.Add(circleShape.Position);
                    Radius = circleShape.Radius;
                    break;
                }
            case ShapeType.Polygon:
                {
                    PolygonShape polygonShape = (PolygonShape)shape;
                    Vertices.Clear();
                    for (int i = 0; i < polygonShape.Vertices.Count; i++)
                    {
                        Vertices.Add(polygonShape.Vertices[i]);
                    }
                    Radius = polygonShape.Radius;
                    break;
                }
            case ShapeType.Chain:
                {
                    ChainShape chainShape = (ChainShape)shape;
                    Vertices.Clear();
                    Vertices.Add(chainShape.Vertices[index]);
                    Vertices.Add((index + 1 < chainShape.Vertices.Count) ? chainShape.Vertices[index + 1] : chainShape.Vertices[0]);
                    Radius = chainShape.Radius;
                    break;
                }
            case ShapeType.Edge:
                {
                    EdgeShape edgeShape = (EdgeShape)shape;
                    Vertices.Clear();
                    Vertices.Add(edgeShape.Vertex1);
                    Vertices.Add(edgeShape.Vertex2);
                    Radius = edgeShape.Radius;
                    break;
                }
        }
    }

    public int GetSupport(Vector2 direction)
    {
        int result = 0;
        float num = Vector2.Dot(Vertices[0], direction);
        for (int i = 1; i < Vertices.Count; i++)
        {
            float num2 = Vector2.Dot(Vertices[i], direction);
            if (num2 > num)
            {
                result = i;
                num = num2;
            }
        }
        return result;
    }

    public Vector2 GetSupportVertex(Vector2 direction)
    {
        int index = 0;
        float num = Vector2.Dot(Vertices[0], direction);
        for (int i = 1; i < Vertices.Count; i++)
        {
            float num2 = Vector2.Dot(Vertices[i], direction);
            if (num2 > num)
            {
                index = i;
                num = num2;
            }
        }
        return Vertices[index];
    }
}
