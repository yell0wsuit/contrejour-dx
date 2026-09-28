using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Vertex;

public struct SpriteVertex : IVertex, IVertexType
{
    public Vector3 Position;

    public Color Color;

    public Vector2 TextureCoordinate;

    public static readonly VertexDeclaration VertexDeclaration;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    Vector3 IVertex.Position
    {
        get
        {
            return Position;
        }
        set
        {
            Position = value;
        }
    }

    Color IVertex.Color
    {
        get
        {
            return Color;
        }
        set
        {
            Color = value;
        }
    }

    Vector2 IVertex.TextureCoordinate
    {
        get
        {
            return TextureCoordinate;
        }
        set
        {
            TextureCoordinate = value;
        }
    }

    static SpriteVertex()
    {
        VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0))
        {
            Name = "SpriteVertex.VertexDeclaration"
        };
    }

    public SpriteVertex(Vector3 position, Color color, Vector2 textureCoordinate)
    {
        Position = position;
        Color = color;
        TextureCoordinate = textureCoordinate;
    }

    public static bool operator ==(SpriteVertex left, SpriteVertex right)
    {
        if (left.Position == right.Position && left.Color == right.Color)
        {
            return left.TextureCoordinate == right.TextureCoordinate;
        }
        return false;
    }

    public static bool operator !=(SpriteVertex left, SpriteVertex right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        return string.Format(CultureInfo.CurrentCulture, "{{Position:{0} Color:{1} TextureCoordinate:{2}}}", new object[3] { Position, Color, TextureCoordinate });
    }

    public override bool Equals(object obj)
    {
        if (obj == null || (object)obj.GetType() != GetType())
        {
            return false;
        }
        return this == (SpriteVertex)obj;
    }
}
