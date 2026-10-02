using System.Collections.Generic;

using Mokus2D.Graphics;

using SkiaSharp;

namespace Mokus2D.Rendering.Skia
{
    // A typeface plus one SKFont per size asked for; labels use a handful of sizes. The typeface lives on
    // the CPU, so it survives render target and device changes: Skia re-caches glyphs in any new context.
    internal sealed class SkiaFontFace(SKTypeface typeface) : IFontFace
    {
        private readonly Dictionary<float, SKFont> _fonts = [];

        public bool IsDisposed { get; private set; }

        public FontMetrics GetMetrics(float size)
        {
            SKFontMetrics metrics = FontFor(size).Metrics;
            return new FontMetrics(metrics.Ascent, metrics.Descent);
        }

        public float MeasureText(string text, float size)
        {
            return string.IsNullOrEmpty(text) ? 0f : FontFor(size).MeasureText(text);
        }

        public SKFont FontFor(float size)
        {
            if (!_fonts.TryGetValue(size, out SKFont font))
            {
                font = new SKFont(typeface, size);
                _fonts[size] = font;
            }
            return font;
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;
            foreach (SKFont font in _fonts.Values)
            {
                font.Dispose();
            }
            _fonts.Clear();
            typeface.Dispose();
        }
    }
}
