using System;

using Mokus2D.Util.Data;

namespace ContreJour.Browser.Platform
{
    // The game's fixed logical canvas, chosen once at Play, as the desktop host chooses its full-screen size at
    // startup: the screen's size in backing pixels, so the art is picked for the largest the page can become
    // (the browser's own full screen). Always landscape, since the game has no portrait layout, with the pixel
    // ratio capped at 2 like the page's backing store. Named apart from the desktop's CanvasSize, whose rule
    // differs.
    public static class BrowserCanvas
    {
        public const float MaxPixelRatio = 2f;

        public static float PixelRatio(double devicePixelRatio)
        {
            return double.IsFinite(devicePixelRatio) && devicePixelRatio > 0
                ? (float)Math.Min(devicePixelRatio, MaxPixelRatio)
                : 1f;
        }

        public static Point LogicalSize(double screenWidth, double screenHeight, double devicePixelRatio)
        {
            float ratio = PixelRatio(devicePixelRatio);
            double longSide = Math.Max(screenWidth, screenHeight) * ratio;
            double shortSide = Math.Min(screenWidth, screenHeight) * ratio;
            return new Point(Math.Max(1, (int)Math.Round(longSide)), Math.Max(1, (int)Math.Round(shortSide)));
        }
    }
}
