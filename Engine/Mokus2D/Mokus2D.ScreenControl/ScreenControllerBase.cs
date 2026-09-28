using Microsoft.Xna.Framework;

using Mokus2D.Util.Resources;

namespace Mokus2D.ScreenControl;

public abstract class ScreenControllerBase : DisposableBase
{
    protected readonly IViewScroller ScreenScroller;

    public float Speed = 500f;

    public Vector2 Direction { get; protected set; }

    public Vector2 ScrollSpeed => Direction * Speed;

    protected ScreenControllerBase(IViewScroller screenScroller)
    {
        ScreenScroller = screenScroller;
    }
}
