using System.Globalization;

namespace Mokus2D.Content;

public static class ContentUtil
{
    public static string GetResourcesSuffix(float scaleFactor)
    {
        return scaleFactor == 1f ? string.Empty : $"_x{(1f / scaleFactor).ToString(CultureInfo.InvariantCulture)}".Replace('.', '_');
    }
}
