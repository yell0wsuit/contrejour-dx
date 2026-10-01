using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Mokus2D.Graphics;

namespace Mokus2D.Visual.Text
{
    // One laid-out line: its text and its width, which counts a margin on each side.
    public readonly record struct TextLine(string Text, float Width);

    // A label's line layout. The measurements are the ones the sprite-glyph layout made, so labels keep
    // their size and place: widths and the text size include a margin on each side, every label is at
    // least one line (plus spacing) tall, and a newline that ends the text starts an empty line without
    // adding its height.
    public static class TextLayout
    {
        public static Vector2 Build(string text, IFontFace face, float emSize, float lineHeight, float lineSpacing, Vector2 margins, List<TextLine> lines)
        {
            lines.Clear();
            Vector2 size = new(0f, (margins.Y * 2f) + lineHeight + lineSpacing);
            StringBuilder line = new();
            for (int i = 0; i < text.Length; i++)
            {
                char symbol = text[i];
                if (symbol == '\r')
                {
                    continue;
                }
                if (symbol == '\n')
                {
                    if (i != text.Length - 1)
                    {
                        size.Y += lineHeight + lineSpacing;
                    }
                    size.X = Math.Max(size.X, AddLine(lines, line.ToString(), face, emSize, margins));
                    _ = line.Clear();
                    continue;
                }
                _ = line.Append(symbol == ' ' ? ' ' : symbol);
            }
            size.X = Math.Max(size.X, AddLine(lines, line.ToString(), face, emSize, margins));
            return size;
        }

        public static float LineOffset(TextAlign align, float areaWidth, float lineWidth)
        {
            return align switch
            {
                TextAlign.Left => 0f,
                TextAlign.Right => areaWidth - lineWidth,
                TextAlign.Center => (areaWidth - lineWidth) / 2f,
                _ => throw new InvalidOperationException(),
            };
        }

        // Where line index starts its baseline in the label's space. sign is the root's sprite Y scale
        // sign: lines advance, and the baseline drops below a line's top, toward +y·sign.
        public static Vector2 LineOrigin(Vector2 start, float lineOffset, int index, float lineHeight, float lineSpacing, float ascent, float sign)
        {
            return new Vector2(start.X + lineOffset, start.Y + (sign * ((index * (lineHeight + lineSpacing)) + ascent)));
        }

        private static float AddLine(List<TextLine> lines, string text, IFontFace face, float emSize, Vector2 margins)
        {
            float width = (margins.X * 2f) + face.MeasureText(text, emSize);
            lines.Add(new TextLine(text, width));
            return width;
        }
    }
}
