using System;
using System.Numerics;

using Mokus2D.Interfaces;

namespace Mokus2D.ScreenControl
{
    public interface IViewScroller : IUpdatable
    {
        Vector2 ScrollSpeed { get; set; }

        Vector2 ViewPosition { get; set; }

        event Action<Vector2> ViewPositionChangeEvent;
    }
}
