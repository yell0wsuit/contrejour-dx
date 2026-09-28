using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace Default.Namespace;

public class PlasticineWideBorder : PrimitivesNode
{
    private VertexPositionColor[] outBorder;

    private VertexPositionColor[] inBorder;

    public VertexPositionColor[] OutBorder => outBorder;

    public VertexPositionColor[] InBorder => inBorder;

    public void SetSizeBorderColorBorderOutColor(int value, Color borderColor, Color borderOutColor)
    {
        Color = borderColor;
        int num = (value * 2 * 2) + 2;
        outBorder = new VertexPositionColor[num];
        inBorder = new VertexPositionColor[num];
        GraphUtil.SetGradientColorsStrip(Color, borderOutColor, outBorder);
        GraphUtil.SetColor(inBorder, Color);
    }

    protected override void DrawPrimitives()
    {
        GraphUtil.DrawTriangleStrip(inBorder);
        GraphUtil.DrawTriangleStrip(outBorder);
    }
}
