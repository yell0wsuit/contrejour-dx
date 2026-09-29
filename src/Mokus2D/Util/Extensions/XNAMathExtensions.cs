using Microsoft.Xna.Framework;

namespace Mokus2D.Util.Extensions;

public static class XNAMathExtensions
{
    public static float ToRadians(this float value)
    {
        return MathHelper.ToRadians(value);
    }

    public static float ToRadians(this int value)
    {
        return MathHelper.ToRadians(value);
    }

    public static float ToDegrees(this float value)
    {
        return MathHelper.ToDegrees(value);
    }

    public static float ToDegrees(this int value)
    {
        return MathHelper.ToDegrees(value);
    }
}
