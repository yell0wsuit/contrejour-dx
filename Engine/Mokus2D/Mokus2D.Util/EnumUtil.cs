using System;
using System.Collections.Generic;
using System.Linq;

namespace Mokus2D.Util;

public static class EnumUtil
{
    public static List<T> GetValues<T>()
    {
        return new List<T>(Enum.GetValues(typeof(T)).Cast<T>());
    }
}
