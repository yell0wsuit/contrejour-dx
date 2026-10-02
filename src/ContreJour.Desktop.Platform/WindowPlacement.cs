using System;

using SDL3;

namespace ContreJour.Desktop.Platform
{
    // Normal client dimensions and the player's maximized choice, independent of fullscreen.
    // Adapted from CutTheRopeDX's SdlWindowService.NextMaximizedState.
    public sealed class WindowPlacement(int width, int height, bool maximized)
    {
        private bool _observedMaximized;

        public int Width { get; private set; } = width;
        public int Height { get; private set; } = height;
        public bool Maximized { get; private set; } = maximized;

        // Returns whether the saved placement changed. Fullscreen/minimized surfaces never replace
        // normal dimensions; unchanged flags before startup maximization keep the saved choice.
        public bool Refresh(SDL.WindowFlags flags, int width, int height)
        {
            if (width <= 0 || height <= 0 || (flags & (SDL.WindowFlags.Fullscreen | SDL.WindowFlags.Minimized)) != 0)
            {
                return false;
            }
            bool changed = false;
            bool maximized = (flags & SDL.WindowFlags.Maximized) != 0;
            if (maximized != _observedMaximized)
            {
                // Cocoa also derives this flag from geometry. Returning to the same screen-filling
                // normal size is not evidence that the player asked to maximize.
                if (!maximized || width != Width || height != Height)
                {
                    changed = Maximized != maximized;
                    Maximized = maximized;
                }
                _observedMaximized = maximized;
            }
            if (!maximized && (width != Width || height != Height))
            {
                Width = width;
                Height = height;
                changed = true;
            }
            return changed;
        }

        // Record a fitted client size before requesting it from SDL, so a geometry-derived zoom
        // flag is compared against that size while retaining the observed maximization history.
        public void SetNormalSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        // SDL sizes the client; its title bar and borders live outside that area.
        public static int ClampSide(int requested, int usable, int decoration)
        {
            int available = Math.Max(1, usable - decoration);
            int wanted = requested > 0 ? requested : (int)(usable * 0.8f);
            return Math.Min(available, Math.Clamp(wanted, 320, 4096));
        }
    }
}
