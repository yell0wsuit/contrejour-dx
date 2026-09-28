using Microsoft.Xna.Framework;

using Mokus2D.Util.Data;
using Mokus2D.Util.Extensions;
using Mokus2D.Visual;

namespace Mokus2D.UI.Layout;

public class TileLayout : LayoutBase
{
    private readonly int _maxItems;

    public Vector2 Margins;

    public Vector2? ItemSize;

    public TileLayout(Node container, int maxItems)
        : base(container)
    {
        _maxItems = maxItems;
    }

    public void Apply()
    {
        for (int i = 0; i < Container.Children.Count; i++)
        {
            Node node = Container.Children[i];
            Vector2 position = new Mokus2D.Util.Data.Point
            {
                X = i % _maxItems,
                Y = i / _maxItems
            } * (Size(node) + Margins);
            node.Position = position;
        }
    }

    private Vector2 Size(Node child)
    {
        if (ItemSize.HasValue)
        {
            return ItemSize.Value;
        }
        return ((ISizeNode)child).ScaledSize();
    }
}
