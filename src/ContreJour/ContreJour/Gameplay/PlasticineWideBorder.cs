using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJour.Gameplay;

public class PlasticineWideBorder : PrimitivesNode
{
    public VertexPositionColor[] OutBorder { get; private set; }

    public VertexPositionColor[] InBorder { get; private set; }

    public void SetSizeBorderColorBorderOutColor(int value, Color borderColor, Color borderOutColor)
    {
        Color = borderColor;
        int num = (value * 2 * 2) + 2;
        OutBorder = new VertexPositionColor[num];
        InBorder = new VertexPositionColor[num];
        GraphUtil.SetGradientColorsStrip(Color, borderOutColor, OutBorder);
        GraphUtil.SetColor(InBorder, Color);
    }

    protected override void DrawPrimitives()
    {
        GraphUtil.DrawTriangleStrip(InBorder);
        GraphUtil.DrawTriangleStrip(OutBorder);
    }
}
