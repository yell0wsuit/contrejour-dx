using System;

using Mokus2D.Input;

namespace Mokus2D.UI.TouchFilters;

public class TypeTouchFilter
{
    private TouchType? type;

    public ref TouchType? Type => ref type;

    public TypeTouchFilter()
    {
    }

    protected virtual bool Matches(Touch touch)
    {
        return !Type.HasValue || touch.Type == Type;
    }
}
