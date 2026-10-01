using Mokus2D.Graphics;

namespace Mokus2D.Tests
{
    // A face whose line is exactly size tall (ascent 0.8, descent 0.2) and whose characters each advance
    // half the size. Source records which file it was made from.
    internal sealed class StubFontFace(string source = null) : IFontFace
    {
        public string Source { get; } = source;

        public bool IsDisposed { get; private set; }

        public FontMetrics GetMetrics(float size)
        {
            return new FontMetrics(-0.8f * size, 0.2f * size);
        }

        public float MeasureText(string text, float size)
        {
            return text.Length * size * 0.5f;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
