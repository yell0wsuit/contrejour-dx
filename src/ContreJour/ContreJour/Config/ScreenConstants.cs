using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace ContreJour.Config
{
    public static class ScreenConstants
    {
        public static class OsSizes
        {
            public static readonly Vector2 IPhoneRetina = new(960f, 640f);

            public static readonly Vector2 W7 = new(800f, 480f);
        }

        [StructLayout(LayoutKind.Sequential, Size = 1)]
        public readonly struct Scales
        {
            public static readonly float fromIPhone2ByHeight = OsSizes.W7.Y / OsSizes.IPhoneRetina.Y;
        }

        public static readonly Vector2 Wp7LevelSize = new(OsSizes.IPhoneRetina.X / Scales.fromIPhone2ByHeight, OsSizes.IPhoneRetina.Y);

        public static readonly Vector2 IPhoneScreenCenter = XnaMath.Divide(OsSizes.IPhoneRetina, 2f);

        public static readonly Vector2 W7FromIPhoneSize = XnaMath.Divide(OsSizes.W7, Scales.fromIPhone2ByHeight);

        public static readonly Vector2 W7FromIPhoneScreenCenter = XnaMath.Divide(W7FromIPhoneSize, 2f);
    }
}
