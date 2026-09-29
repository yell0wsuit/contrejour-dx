using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay;

public class PlasticineSprite : PrimitivesNode
{
    private static readonly Color COLOR = new(0, 0, 0, 255);

    public VertexPositionColor[] Vertices { get; private set; }

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
        if (Vertices != null)
        {
            GraphUtil.SetColor(Vertices, Color);
        }
    }

    protected override void DrawPrimitives()
    {
        GraphUtil.DrawTriangleStrip(Vertices);
    }

    public void InitVertices(int verticesCount)
    {
        Vertices = new VertexPositionColor[verticesCount];
        RefreshColor();
    }
}
