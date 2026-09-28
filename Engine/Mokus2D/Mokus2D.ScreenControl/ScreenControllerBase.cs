using Microsoft.Xna.Framework;

using Mokus2D.Util.Resources;

namespace Mokus2D.ScreenControl;

public abstract class ScreenControllerBase(IViewScroller screenScroller) : DisposableBase
{
    protected readonly IViewScroller ScreenScroller = screenScroller;

    public float Speed = 500f;

    public Vector2 Direction { get; protected set; }

    public Vector2 ScrollSpeed => Direction * Speed;
}
