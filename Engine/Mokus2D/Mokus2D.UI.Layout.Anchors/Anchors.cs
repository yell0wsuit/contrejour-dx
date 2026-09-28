using Microsoft.Xna.Framework;

namespace Mokus2D.UI.Layout.Anchors;

public static class Anchors
{
    public static readonly Vector2 LeftTop = Vector2.Zero;

    public static readonly Vector2 LeftBottom = new(0f, 1f);

    public static readonly Vector2 RightTop = new(1f, 0f);

    public static readonly Vector2 RightBottom = new(1f, 1f);

    public static readonly Vector2 Center = new(0.5f);

    public static readonly Vector2 CenterTop = new(0.5f, 0f);

    public static readonly Vector2 CenterBottom = new(0.5f, 1f);

    public static readonly Vector2 LeftCenter = new(0f, 0.5f);

    public static readonly Vector2 RightCenter = new(1f, 0.5f);
}
