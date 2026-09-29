using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace Mokus2D.Visual
{
    public class Scale9FilledBorder : Scale9Border
    {
        private readonly Sprite _fill;

        public Scale9FilledBorder(string whiteSquareId, Color borderColor, Color backgroundColor, Vector2 size)
            : base(size, whiteSquareId, whiteSquareId)
        {
            _fill = new Sprite(whiteSquareId)
            {
                Color = backgroundColor,
                ColorRatio = 1f
            };
            AddChildAt(_fill, 0);
            SetColor(CornerSprites, borderColor);
            SetColor(SideSprites, borderColor);
        }

        private static void SetColor(List<ISizeNode> nodes, Color borderColor)
        {
            foreach (Node node in nodes.Cast<Node>())
            {
                node.Color = borderColor;
                node.ColorRatio = 1f;
            }
        }
    }
}
