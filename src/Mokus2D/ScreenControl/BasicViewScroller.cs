using System;
using System.Numerics;

using Mokus2D.Interfaces;
using Mokus2D.Util;

namespace Mokus2D.ScreenControl
{
    public class BasicViewScroller : IViewScroller, IUpdatable
    {
        private Vector2 _viewPosition;

        public Vector2 ScrollSpeed { get; set; }

        public Vector2 ViewPosition
        {
            get => _viewPosition;
            set
            {
                if (_viewPosition != value)
                {
                    _viewPosition = value;
                    ViewPositionChangeEvent.Dispatch(ViewPosition);
                }
            }
        }

        public event Action<Vector2> ViewPositionChangeEvent;

        public void Update(float time)
        {
            ViewPosition += ScrollSpeed * time;
        }
    }
}
