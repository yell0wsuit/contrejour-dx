using System;
using Mokus2D.Input;

namespace Mokus2D.UI.TouchFilters;

public class TypeTouchFilter
{
	public TouchType? Type;

	public readonly Predicate<Touch> Predicate;

	public TypeTouchFilter()
	{
		Predicate = Matches;
	}

	protected virtual bool Matches(Touch touch)
	{
		if (Type.HasValue)
		{
			return touch.Type == Type;
		}
		return true;
	}
}
