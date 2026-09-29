using Microsoft.Xna.Framework;

using Mokus2D.Visual.Drawing;
using Mokus2D.Visual.Drawing.Vertex;

namespace Mokus2D.Config.Tint
{
    public class TintQuad<T> : Quad<T> where T : struct, IVertex, ITintVertex
    {
        public override void RefreshColor(Color color, float colorAmount)
        {
            base.RefreshColor(color, colorAmount);
            LeftBottom.ColorRatio = colorAmount;
            LeftTop.ColorRatio = colorAmount;
            RightBottom.ColorRatio = colorAmount;
            RightTop.ColorRatio = colorAmount;
        }
    }
}
