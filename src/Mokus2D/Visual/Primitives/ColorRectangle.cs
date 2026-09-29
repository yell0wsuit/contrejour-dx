using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual.Util;

namespace Mokus2D.Visual.Primitives;

public class ColorRectangle : PrimitivesNode
{
    private Vector2 size;

    private bool sizeDirty;

    private readonly VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];

    public Vector2 Size
    {
        get => size;
        set
        {
            if (size != value)
            {
                size = value;
                sizeDirty = true;
            }
        }
    }

    public override float OpacityFloat
    {
        set
        {
            if (value != OpacityFloat)
            {
                base.OpacityFloat = value;
                Color = Color.ChangeAlpha(OpacityFloat);
            }
        }
    }

    public override Color Color
    {
        set
        {
            if (Color != value)
            {
                base.Color = value;
                RefreshColors();
            }
        }
    }

    public ColorRectangle(Color color, Vector2 size)
    {
        Color = color;
        Size = size;
        RefreshColors();
    }

    private void RefreshSize()
    {
        float x = size.X;
        float y = size.Y;
        vertices[0].Position = Vector2.Zero.ToVector3();
        vertices[1].Position = new Vector2(x, 0f).ToVector3();
        vertices[2].Position = new Vector2(0f, y).ToVector3();
        vertices[3].Position = new Vector2(x, y).ToVector3();
        sizeDirty = false;
    }

    private void RefreshColors()
    {
        GraphUtil.SetColor(vertices, Color);
    }

    protected override void DrawPrimitives()
    {
        TryRefreshSize();
        if (size.X > 0f && size.Y > 0f && OpacityByte > 0)
        {
            GraphUtil.DrawTriangleStrip(vertices);
        }
    }

    private void TryRefreshSize()
    {
        if (sizeDirty)
        {
            RefreshSize();
        }
    }
}
