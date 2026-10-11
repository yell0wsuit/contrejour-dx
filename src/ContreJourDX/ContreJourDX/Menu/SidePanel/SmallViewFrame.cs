using System.Numerics;

using ContreJourDX.Config;

using Mokus2D.Graphics;
using Mokus2D.Visual;
using Mokus2D.Visual.Primitives;

namespace ContreJourDX.Menu.SidePanel
{
    public class SmallViewFrame : Node
    {
        private readonly ColorRectangle bottomLine;

        private readonly ColorRectangle topLine;

        public SmallViewFrame(float stripeHeight)
        {
            if (bottomLine == null)
            {
                bottomLine = new ColorRectangle(Color.Black, Vector2.Zero);
                AddChild(bottomLine);
            }
            if (topLine == null)
            {
                topLine = new ColorRectangle(Color.Black, Vector2.Zero);
                AddChild(topLine);
            }
            bottomLine.Size = new Vector2(ContreJourDXConfig.RootSize.X, stripeHeight);
            bottomLine.Position = new Vector2(0f, 0f - stripeHeight);
            topLine.Size = new Vector2(ContreJourDXConfig.RootSize.X, stripeHeight);
            topLine.Position = new Vector2(0f, ContreJourDXConfig.RootSize.Y);
        }
    }
}
