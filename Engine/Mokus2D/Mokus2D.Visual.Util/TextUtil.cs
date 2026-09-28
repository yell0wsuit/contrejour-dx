using Microsoft.Xna.Framework.Input;
using Mokus2D.Platforms.Input;

namespace Mokus2D.Visual.Util;

public static class TextUtil
{
	public static char? KeyToChar(Keys keys, bool isUpper)
	{
		if (keys == Keys.Back)
		{
			return null;
		}
		return KeyboardUtil.GetCharsFromKeys(keys, isUpper);
	}
}
