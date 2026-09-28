using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Vertex;

public struct TintSpriteVertex : ITintVertex, IVertex, IVertexType
{
    public Vector3 Position;

    public Color Color;

    public Vector2 TextureCoordinate;

    public float ColorRatio;

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

    float ITintVertex.ColorRatio
    {
        get
        {
            return ColorRatio;
        }
        set
        {
            ColorRatio = value;
        }
    }

    static TintSpriteVertex()
    {
        VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(16, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 0))
        {
            Name = "TintSpriteVertex.VertexDeclaration"
        };
    }

    public TintSpriteVertex(Vector3 position, Color color, Vector2 textureCoordinate, float colorRatio)
    {
        Position = position;
        Color = color;
        TextureCoordinate = textureCoordinate;
        ColorRatio = colorRatio;
    }

    public static bool operator ==(TintSpriteVertex left, TintSpriteVertex right)
    {
        if (left.Position == right.Position && left.Color == right.Color)
        {
            return left.TextureCoordinate == right.TextureCoordinate;
        }
        return false;
    }

    public static bool operator !=(TintSpriteVertex left, TintSpriteVertex right)
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
        return this == (TintSpriteVertex)obj;
    }
}
