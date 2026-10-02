using Mokus2D.Input;
using Mokus2D.Platforms.Input;

namespace Mokus2D.Visual.Util
{
    public static class TextUtil
    {
        public static char? KeyToChar(Key keys, bool isUpper)
        {
            return keys == Key.Back ? null : KeyboardUtil.GetCharsFromKeys(keys, isUpper);
        }
    }
}
