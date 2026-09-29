using System.Numerics;

using Mokus2D.Util.Resources;

namespace Mokus2D.ScreenControl
{
    public abstract class ScreenControllerBase(IViewScroller screenScroller) : DisposableBase
    {
        protected IViewScroller ScreenScroller { get; } = screenScroller;

        public float Speed { get; set; } = 500f;

        public Vector2 Direction { get; protected set; }

        public Vector2 ScrollSpeed => Direction * Speed;
    }
}
