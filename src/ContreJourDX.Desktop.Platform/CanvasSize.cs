using System;

namespace ContreJourDX.Desktop.Platform
{
    // Picks the fixed logical canvas the game lays itself out on: the full-screen window's pixel
    // size, or the display's when the window has no area yet (minimized at launch).
    public static class CanvasSize
    {
        public static (int Width, int Height) Choose(int windowPixelWidth, int windowPixelHeight, int displayWidth, int displayHeight, float displayDensity)
        {
            return windowPixelWidth > 0 && windowPixelHeight > 0
                ? (windowPixelWidth, windowPixelHeight)
                : ((int)MathF.Round(displayWidth * displayDensity), (int)MathF.Round(displayHeight * displayDensity));
        }
    }
}
