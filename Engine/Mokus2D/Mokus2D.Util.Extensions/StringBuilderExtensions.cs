using System.Text;

namespace Mokus2D.Util.Extensions;

public static class StringBuilderExtensions
{
	public static int? IndexOf(this StringBuilder stringBuilder, char symbol, int startIndex = 0)
	{
		for (int i = startIndex; i < stringBuilder.Length; i++)
		{
			if (stringBuilder[i] == symbol)
			{
				return i;
			}
		}
		return null;
	}
}
