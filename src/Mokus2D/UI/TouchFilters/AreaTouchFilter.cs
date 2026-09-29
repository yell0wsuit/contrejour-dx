using System.Numerics;

using Mokus2D.Input;
using Mokus2D.Visual;

namespace Mokus2D.UI.TouchFilters
{
    public class AreaTouchFilter(Sprite area) : TypeTouchFilter
    {
        private readonly Sprite _area = area;

        protected override bool Matches(Touch touch)
        {
            return base.Matches(touch) && IsInArea(touch);
        }

        private bool IsInArea(Touch touch)
        {
            Vector2 value = _area.GlobalToLocal(touch.Position);
            return _area.Bounds.Contains(value);
        }
    }
}
