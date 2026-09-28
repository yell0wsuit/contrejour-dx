using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class PlasticineSprite : PrimitivesNode
{
    private static readonly Color COLOR = new(0, 0, 0, 255);

    private VertexPositionColor[] vertices;

    public VertexPositionColor[] Vertices => vertices;

    public override Color Color
    {
        get => base.Color;
        set
        {
            if (Color != value)
            {
                base.Color = value;
                RefreshColor();
            }
        }
    }

    public PlasticineSprite()
    {
        Color = COLOR;
    }

    private void RefreshColor()
    {
        if (vertices != null)
        {
            GraphUtil.SetColor(vertices, Color);
        }
    }

    protected override void DrawPrimitives()
    {
        GraphUtil.DrawTriangleStrip(vertices);
    }

    public void InitVertices(int verticesCount)
    {
        vertices = new VertexPositionColor[verticesCount];
        RefreshColor();
    }
}
