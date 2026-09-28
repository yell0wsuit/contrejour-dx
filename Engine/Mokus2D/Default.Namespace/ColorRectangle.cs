using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class ColorRectangle : PrimitivesNode
{
    protected Vector2 size;

    protected bool sizeDirty;

    protected VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[4];

    public Vector2 Size
    {
        get
        {
            return size;
        }
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

    public ColorRectangle(Color _color, Vector2 _size)
    {
        Color = _color;
        Size = _size;
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
        if (size.X > 0f && size.Y > 0f && base.OpacityByte > 0)
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
