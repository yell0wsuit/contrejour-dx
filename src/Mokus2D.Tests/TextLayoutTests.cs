using System.Collections.Generic;
using System.Numerics;

using Mokus2D.Visual.Text;

using Xunit;

namespace Mokus2D.Tests
{
    public class TextLayoutTests
    {
        private static readonly Vector2 Margins = new(2f);

        // At size 10 the stub advances 5 per character; lines are 20 tall.
        private static Vector2 Build(string text, List<TextLine> lines, float lineSpacing = 0f)
        {
            return TextLayout.Build(text, new StubFontFace(), 10f, 20f, lineSpacing, Margins, lines);
        }

        [Fact]
        public void OneLineIsItsWidthPlusMargins()
        {
            List<TextLine> lines = [];

            Vector2 size = Build("abc", lines);

            Assert.Equal([new TextLine("abc", 19f)], lines);
            Assert.Equal(new Vector2(19f, 24f), size);
        }

        [Fact]
        public void LinesStackWithTheirSpacing()
        {
            List<TextLine> lines = [];

            Vector2 size = Build("ab\ncdef", lines, 3f);

            Assert.Equal([new TextLine("ab", 14f), new TextLine("cdef", 24f)], lines);
            Assert.Equal(new Vector2(24f, 50f), size);
        }

        [Fact]
        public void ATrailingNewlineAddsAnEmptyLineButNoHeight()
        {
            List<TextLine> lines = [];

            Vector2 size = Build("ab\n", lines, 3f);

            Assert.Equal([new TextLine("ab", 14f), new TextLine(string.Empty, 4f)], lines);
            Assert.Equal(new Vector2(14f, 27f), size);
        }

        [Fact]
        public void ReturnsAreDroppedAndNoBreakSpacesBecomeSpaces()
        {
            List<TextLine> lines = [];

            _ = Build("a\r\nb c", lines);

            Assert.Equal([new TextLine("a", 9f), new TextLine("b c", 19f)], lines);
        }

        [Fact]
        public void EmptyTextIsOneEmptyLine()
        {
            List<TextLine> lines = [];

            Vector2 size = Build(string.Empty, lines);

            Assert.Equal([new TextLine(string.Empty, 4f)], lines);
            Assert.Equal(new Vector2(4f, 24f), size);
        }

        [Fact]
        public void OnlyANewlineIsTwoEmptyLines()
        {
            List<TextLine> lines = [];

            Vector2 size = Build("\n", lines);

            Assert.Equal([new TextLine(string.Empty, 4f), new TextLine(string.Empty, 4f)], lines);
            Assert.Equal(new Vector2(4f, 24f), size);
        }

        [Fact]
        public void BuildReplacesEarlierLines()
        {
            List<TextLine> lines = [new TextLine("old", 1f)];

            _ = Build("a", lines);

            Assert.Equal([new TextLine("a", 9f)], lines);
        }

        [Theory]
        [InlineData(TextAlign.Left, 0f)]
        [InlineData(TextAlign.Center, 30f)]
        [InlineData(TextAlign.Right, 60f)]
        public void LinesAlignInsideTheArea(TextAlign align, float expected)
        {
            Assert.Equal(expected, TextLayout.LineOffset(align, 100f, 40f));
        }

        [Theory]
        [InlineData(1f, 41f)]
        [InlineData(-1f, -37f)]
        public void BaselinesStepAwayFromTheStartBySign(float sign, float expectedY)
        {
            // Second line: one line pitch (20 + 3) plus the ascent (16) away from the start.
            Vector2 origin = TextLayout.LineOrigin(new Vector2(2f, 2f), 30f, 1, 20f, 3f, 16f, sign);

            Assert.Equal(new Vector2(32f, expectedY), origin);
        }
    }
}
