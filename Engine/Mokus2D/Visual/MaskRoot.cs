using Microsoft.Xna.Framework;

namespace Mokus2D.Visual;

public class MaskRoot : RootNode
{
    public MaskRoot(int width, int height, Vector2 spritesScaleFactor)
        : base(width, height, spritesScaleFactor)
    {
    }

    public MaskRoot(int width, int height)
        : base(width, height)
    {
    }

    public override void AddChild(Node node, int nodeLayer)
    {
        Children.Add(node);
    }

    public override void RemoveChild(Node node)
    {
        _ = Children.Remove(node);
    }
}
