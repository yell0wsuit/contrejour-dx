using System;

using SkiaSharp;

namespace ContreJourDX.Desktop.Platform.Graphics
{
    // Proves a shaded draw reaches a device's render target. A driver that fails to link a program
    // draws nothing while Skia reports success, so a clear alone cannot tell a working device from
    // a silent one; a gradient forces a program to exist.
    public static class DrawCheck
    {
        public static SKColor Background => SKColors.Black;

        public static void Draw(SKCanvas canvas, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(canvas);
            canvas.Clear(Background);
            using SKShader shader = SKShader.CreateLinearGradient(
                new SKPoint(0, 0),
                new SKPoint(width, height),
                [SKColors.Red, SKColors.Blue],
                null,
                SKShaderTileMode.Clamp);
            using SKPaint paint = new() { Shader = shader };
            canvas.DrawRect(SKRect.Create(0, 0, width, height), paint);
        }

        public static SKPointI Sample(int width, int height)
        {
            return new SKPointI(width / 2, height / 2);
        }

        // The gradient is opaque and never black, so a black or transparent sample means nothing
        // was drawn (transparent: a readback that reported success without writing).
        public static bool Drew(SKColor sample)
        {
            return sample.Alpha != 0 && sample != Background;
        }
    }
}
