using Microsoft.Xna.Framework;
using Mokus2D.Input;
using Mokus2D.Visual;

namespace Mokus2D.UI.TouchFilters;

public class AreaTouchFilter : TypeTouchFilter
{
	private readonly Sprite _area;

	public AreaTouchFilter(Sprite area)
	{
		_area = area;
	}

	protected override bool Matches(Touch touch)
	{
		if (base.Matches(touch))
		{
			return IsInArea(touch);
		}
		return false;
	}

	private bool IsInArea(Touch touch)
	{
		Vector2 value = _area.GlobalToLocal(touch.Position);
		return _area.Bounds.Contains(value);
	}
}
