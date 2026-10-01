using System;

namespace Mokus2D.Graphics
{
    // A face's vertical extent at one size, in the text's own units: Ascent is negative (above the
    // baseline), Descent positive (below it). A line is Descent − Ascent tall.
    public readonly record struct FontMetrics(float Ascent, float Descent);

    // A font file loaded by the renderer. Sizes are em sizes in the text's own units.
    public interface IFontFace : IDisposable
    {
        FontMetrics GetMetrics(float size);

        // The advance of the whole text; 0 for an empty string. A character the face lacks counts as its
        // missing-glyph box.
        float MeasureText(string text, float size);
    }
}
