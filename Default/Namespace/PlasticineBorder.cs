using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class PlasticineBorder : PrimitivesNode, IOpacity
{
    protected VertexPositionColorTexture[] outBorder;

    protected VertexPositionColorTexture[] inBorder;

    protected int polygonSize;

    private static readonly Color OUT_COLOR = new Color(255, 255, 255) * 0f;

    private static readonly Color IN_COLOR = new Color(0, 0, 0, 0);

    private static readonly Color CENTER_COLOR = new Color(127, 127, 127);

    private float WIDTH = 4f;

    public override float OpacityFloat
    {
        set
        {
            if (OpacityFloat != value)
            {
                base.OpacityFloat = value;
                CreateColors();
            }
        }
    }

    public PlasticineBorder(List<Vector2> initialPolygon)
    {
        List<Vector2> surface = new List<Vector2>();
        ContreDrawUtil.CreateBezierSurfaceSurfaceSegments(initialPolygon, ref surface, 3);
        polygonSize = surface.Count;
        outBorder = new VertexPositionColorTexture[surface.Count * 6];
        inBorder = new VertexPositionColorTexture[surface.Count * 6];
        GraphUtil.CreateGradientBorderWidthVertices(surface, BorderWidth(), inBorder);
        GraphUtil.CreateGradientBorderWidthVertices(surface, 0f - BorderWidth(), outBorder);
        CreateColors();
        OpacityFloat = 0f;
    }

    public virtual float BorderWidth()
    {
        return WIDTH / 2f;
    }

    public virtual Color OutColor()
    {
        return OUT_COLOR;
    }

    public virtual Color InColor()
    {
        return IN_COLOR;
    }

    public virtual Color CenterColor()
    {
        return CENTER_COLOR;
    }

    public void CreateColors()
    {
        Color startColor = CenterColor();
        GraphUtil.CreateGradientColorsList(polygonSize, startColor, InColor(), inBorder);
        GraphUtil.CreateGradientColorsList(polygonSize, startColor, OutColor(), outBorder);
    }

    protected override void DrawPrimitives()
    {
        if (OpacityFloat > 0f)
        {
            GraphUtil.FillTrianglesList(inBorder);
            GraphUtil.FillTrianglesList(outBorder);
        }
    }

    int IOpacity.OpacityByte
    {
        get
        {
            return base.OpacityByte;
        }
        set
        {
            base.OpacityByte = value;
        }
    }
}
