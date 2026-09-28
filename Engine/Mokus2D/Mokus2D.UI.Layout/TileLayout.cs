using Microsoft.Xna.Framework;

using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public class TileLayout(Node container, int maxItems) : LayoutBase(container)
{
    private readonly int _maxItems = maxItems;

    private Vector2 Margins;

    private Vector2? ItemSize;

    public void Apply()
    {
        for (int i = 0; i < Container.Children.Count; i++)
        {
            Node node = Container.Children[i];
            Vector2 position = new Util.Data.Point
            {
                X = i % _maxItems,
                Y = i / _maxItems
            } * (Size(node) + Margins);
            node.Position = position;
        }
    }

    private Vector2 Size(Node child)
    {
        return ItemSize ?? ((ISizeNode)child).ScaledSize();
    }
}
