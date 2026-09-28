using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Mokus2D.Visual;

public static class ScreenConstants
{
    public static class OsSizes
    {
        public static Vector2 IPhoneRetina = new Vector2(960f, 640f);

        public static Vector2 W7 = new Vector2(800f, 480f);
    }

    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct Scales
    {
        public static float fromIPhone2ByHeight = OsSizes.W7.Y / OsSizes.IPhoneRetina.Y;
    }

    public static readonly Vector2 WP7_LEVEL_SIZE = new Vector2(OsSizes.IPhoneRetina.X / Scales.fromIPhone2ByHeight, OsSizes.IPhoneRetina.Y);

    public static readonly Vector2 IPhoneScreenCenter = OsSizes.IPhoneRetina / 2f;

    public static readonly Vector2 W7FromIPhoneSize = OsSizes.W7 / Scales.fromIPhone2ByHeight;

    public static readonly Vector2 W7FromIPhoneScreenCenter = W7FromIPhoneSize / 2f;
}
