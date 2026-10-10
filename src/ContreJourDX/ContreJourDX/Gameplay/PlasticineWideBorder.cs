using Mokus2D.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Util;

namespace ContreJourDX.Gameplay
{
    public class PlasticineWideBorder : PrimitivesNode
    {
        public Vertex[] OutBorder { get; private set; }

        public Vertex[] InBorder { get; private set; }

        public void SetSizeBorderColorBorderOutColor(int value, Color borderColor, Color borderOutColor)
        {
            Color = borderColor;
            int num = (value * 2 * 2) + 2;
            OutBorder = new Vertex[num];
            InBorder = new Vertex[num];
            GraphUtil.SetGradientColorsStrip(Color, borderOutColor, OutBorder);
            GraphUtil.SetColor(InBorder, Color);
        }

        protected override void DrawPrimitives()
        {
            GraphUtil.DrawTriangleStrip(InBorder);
            GraphUtil.DrawTriangleStrip(OutBorder);
        }
    }
}
