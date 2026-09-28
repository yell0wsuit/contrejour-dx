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

    readonly VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    Vector3 IVertex.Position
    {
        readonly get => Position; set => Position = value;
    }

    Color IVertex.Color
    {
        readonly get => Color; set => Color = value;
    }

    Vector2 IVertex.TextureCoordinate
    {
        readonly get => TextureCoordinate; set => TextureCoordinate = value;
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
        return left.Position == right.Position && left.Color == right.Color ? left.TextureCoordinate == right.TextureCoordinate : false;
    }

    public static bool operator !=(SpriteVertex left, SpriteVertex right)
    {
        return !(left == right);
    }

    public override readonly string ToString()
    {
        return string.Format(CultureInfo.CurrentCulture, "{{Position:{0} Color:{1} TextureCoordinate:{2}}}", new object[3] { Position, Color, TextureCoordinate });
    }

    public override readonly bool Equals(object obj)
    {
        return obj == null || (object)obj.GetType() != GetType() ? false : this == (SpriteVertex)obj;
    }
}
