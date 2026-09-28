using System.Globalization;

namespace Mokus2D.Content;

public static class ContentUtil
{
	private const string SuffixFormat = "_x{0}";

	public static string GetResourcesSuffix(float scaleFactor)
	{
		if (scaleFactor == 1f)
		{
			return string.Empty;
		}
		return $"_x{(1f / scaleFactor).ToString(CultureInfo.InvariantCulture)}".Replace('.', '_');
	}
}
