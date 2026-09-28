using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Mokus2D.Util.Extensions;

public static class XNACollectionExtensions
{
    public static Vector2 GetVector2(this IDictionary<string, string> source, string key, Vector2 defaultValue = default(Vector2))
    {
        if (source.ContainsKey(key))
        {
            return source[key].ToVector2();
        }
        return defaultValue;
    }
}
