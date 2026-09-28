using System.Collections.Generic;

using Mokus2D.Platforms.Input;

namespace Mokus2D.PlatformSupport.Input;

public static class CursorPoints
{
    private static readonly List<CursorPoint> Points = new(64);

    public static List<CursorPoint> GetCursorPoints()
    {
        Points.Clear();
        CursorPointsFiller.FillPoints(Points);
        return Points;
    }
}
