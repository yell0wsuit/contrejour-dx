using Microsoft.Xna.Framework.Input;

using Mokus2D.Platforms.Input;

namespace Mokus2D.Visual.Util;

public static class TextUtil
{
    public static char? KeyToChar(Keys keys, bool isUpper)
    {
        return keys == Keys.Back ? null : KeyboardUtil.GetCharsFromKeys(keys, isUpper);
    }
}
