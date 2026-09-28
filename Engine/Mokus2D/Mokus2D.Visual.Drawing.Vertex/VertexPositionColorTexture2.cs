using System.Globalization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Drawing.Vertex;

public struct VertexPositionColorTexture2 : IVertex, IVertexType
{
    public static readonly VertexDeclaration VertexDeclaration;

    public Vector3 Position;

    public Color Color;

    public float Rotation;

    public Vector2 Scale;

    public Vector2 TextureCoordinate;

    public Vector2 NormalMapTextureCoordinate;

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

    static VertexPositionColorTexture2()
    {
        VertexDeclaration = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 0), new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.BlendWeight, 0), new VertexElement(20, VertexElementFormat.Vector2, VertexElementUsage.BlendWeight, 1), new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0), new VertexElement(36, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1))
        {
            Name = "VertexPositionColorTexture2.VertexDeclaration"
        };
    }

    public VertexPositionColorTexture2(Vector3 position, Color color, Vector2 textureCoordinate, float rotation)
    {
        Position = position;
        Color = color;
        TextureCoordinate = textureCoordinate;
        Scale = Vector2.One;
        NormalMapTextureCoordinate = Vector2.Zero;
        Rotation = rotation;
    }

    public static bool operator ==(VertexPositionColorTexture2 left, VertexPositionColorTexture2 right)
    {
        if (left.Position == right.Position && left.Color == right.Color)
        {
            return left.TextureCoordinate == right.TextureCoordinate;
        }
        return false;
    }

    public static bool operator !=(VertexPositionColorTexture2 left, VertexPositionColorTexture2 right)
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
        return this == (VertexPositionColorTexture2)obj;
    }
}
