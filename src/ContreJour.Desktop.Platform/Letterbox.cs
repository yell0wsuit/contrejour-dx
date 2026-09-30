using System;
using System.Numerics;

namespace ContreJour.Desktop.Platform
{
    // Fits the game's fixed logical canvas into a window, uniformly scaled and centered, and maps
    // positions between the canvas, the window's pixels and its points (SDL reports the mouse in
    // points; the drawable is in pixels).
    public readonly struct Letterbox
    {
        public Letterbox(Vector2 logicalSize, Vector2 windowSize, Vector2 pixelSize)
        {
            RequirePositive(logicalSize, nameof(logicalSize));
            RequirePositive(windowSize, nameof(windowSize));
            RequirePositive(pixelSize, nameof(pixelSize));
            LogicalSize = logicalSize;
            WindowSize = windowSize;
            PixelsPerPoint = pixelSize / windowSize;
            Scale = MathF.Min(pixelSize.X / logicalSize.X, pixelSize.Y / logicalSize.Y);
            Offset = (pixelSize - (logicalSize * Scale)) / 2f;
        }

        public Vector2 LogicalSize { get; }

        public Vector2 WindowSize { get; }

        // Canvas units to pixels.
        public float Scale { get; }

        // Top-left of the canvas inside the drawable, in pixels.
        public Vector2 Offset { get; }

        private Vector2 PixelsPerPoint { get; }

        public Vector2 LogicalToPixel(Vector2 logical)
        {
            return (logical * Scale) + Offset;
        }

        public Vector2 PixelToLogical(Vector2 pixel)
        {
            return (pixel - Offset) / Scale;
        }

        public Vector2 WindowToLogical(Vector2 windowPoint)
        {
            return PixelToLogical(windowPoint * PixelsPerPoint);
        }

        public Vector2 LogicalToWindow(Vector2 logical)
        {
            return LogicalToPixel(logical) / PixelsPerPoint;
        }

        private static void RequirePositive(Vector2 size, string name)
        {
            if (!(size.X > 0f && size.Y > 0f))
            {
                throw new ArgumentOutOfRangeException(name, size, "Sizes must be positive.");
            }
        }
    }
}
