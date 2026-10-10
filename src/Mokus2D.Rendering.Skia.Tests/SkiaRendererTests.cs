using System;
using System.IO;
using System.Numerics;

using Mokus2D.Graphics;

using SkiaSharp;

using Xunit;

namespace Mokus2D.Rendering.Skia.Tests
{
    public class SkiaRendererTests
    {
        // Texture premultiplication, vertex-color quantization and blending each round once.
        private const float Tolerance = 2f;

        private static readonly SKColor Background = new(40, 80, 120, 255);

        private static readonly Color White = new(255, 255, 255, 255);

        // The contract's result for one pixel, rgb in 0-255. tex is premultiplied, c straight, dst
        // premultiplied, all 0-1.
        private static Vector3 Expected(Vector4 tex, Vector4 c, ColorMode mode, BlendMode blend, float opacity, Vector4 dst)
        {
            Vector4 s = mode == ColorMode.Sprite
                ? new Vector4(tex.X * c.X * c.W, tex.Y * c.Y * c.W, tex.Z * c.Z * c.W, tex.W * c.W)
                : tex * c * opacity;
            Vector4 weighted = new(s.X * s.W, s.Y * s.W, s.Z * s.W, s.W);
            Vector4 result = blend switch
            {
                BlendMode.AlphaBlend => s + (dst * (1f - s.W)),
                BlendMode.Additive => weighted + dst,
                BlendMode.NonPremultiplied => weighted + (dst * (1f - s.W)),
                _ => throw new ArgumentOutOfRangeException(nameof(blend)),
            };
            result = Vector4.Clamp(result, Vector4.Zero, Vector4.One) * 255f;
            return new Vector3(result.X, result.Y, result.Z);
        }

        private static void AssertRgb(Vector3 expected, Vector4 actual)
        {
            Vector3 rgb = new(actual.X, actual.Y, actual.Z);
            Vector3 error = Vector3.Abs(expected - rgb);
            Assert.True(error.X <= Tolerance && error.Y <= Tolerance && error.Z <= Tolerance, $"expected {expected}, got {rgb}");
        }

        [Theory]
        [InlineData(ColorMode.Sprite, BlendMode.AlphaBlend, true)]
        [InlineData(ColorMode.Sprite, BlendMode.Additive, true)]
        [InlineData(ColorMode.Sprite, BlendMode.NonPremultiplied, true)]
        [InlineData(ColorMode.Primitive, BlendMode.NonPremultiplied, true)]
        [InlineData(ColorMode.Primitive, BlendMode.Additive, true)]
        [InlineData(ColorMode.Primitive, BlendMode.NonPremultiplied, false)]
        [InlineData(ColorMode.Primitive, BlendMode.Additive, false)]
        public void BlendsAsTheContractSays(ColorMode mode, BlendMode blend, bool textured)
        {
            using RenderTarget target = new(4, 4, Background);
            SKColor texel = new(200, 100, 50, 128);
            ITexture texture = textured ? target.Texture(1, 1, texel) : null;
            Color color = new(204, 153, 102, 128);
            const float opacity = 0.6f;

            target.Draw(RenderTarget.Quad(-1f, 1f, 1f, -1f, color), new DrawState(texture, blend, SamplerMode.LinearClamp, mode, opacity));

            float texelAlpha = texel.Alpha / 255f;
            Vector4 tex = textured
                ? new Vector4(texel.Red / 255f * texelAlpha, texel.Green / 255f * texelAlpha, texel.Blue / 255f * texelAlpha, texelAlpha)
                : Vector4.One;
            Vector4 c = new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            Vector4 dst = new(Background.Red / 255f, Background.Green / 255f, Background.Blue / 255f, 1f);
            AssertRgb(Expected(tex, c, mode, blend, opacity, dst), target.Pixel(1, 1));
            texture?.Dispose();
        }

        [Fact]
        public void UntexturedGradientsInterpolateStraight()
        {
            // Opaque grey fading to transparent black: with straight interpolation the source is
            // 127·(1 − t) at alpha (1 − t), so NonPremultiplied over black leaves 127·(1 − t)².
            using RenderTarget target = new(101, 1, SKColors.Black);
            Vertex[] quad = RenderTarget.Quad(-1f, 1f, 1f, -1f, new Color(127, 127, 127, 255), new Color(0, 0, 0, 0));

            target.Draw(quad, new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            foreach (int x in new[] { 25, 50, 75 })
            {
                float t = (x + 0.5f) / 101f;
                float expected = 127f * (1f - t) * (1f - t);
                Assert.InRange(target.Pixel(x, 0).X, expected - Tolerance, expected + Tolerance);
            }
        }

        [Theory]
        [InlineData(SamplerMode.LinearClamp, false)]
        [InlineData(SamplerMode.LinearWrap, true)]
        public void SamplerModeClampsOrRepeats(SamplerMode sampler, bool repeats)
        {
            // A red|blue texture stretched over u = 0..2: past u = 1 clamping stays blue, repeating
            // brings red back.
            using RenderTarget target = new(8, 1, SKColors.Black);
            ITexture texture = target.Texture(2, 1, SKColors.Red, SKColors.Blue);

            target.Draw(RenderTarget.Quad(-1f, 1f, 1f, -1f, White, uMax: 2f), new DrawState(texture, BlendMode.AlphaBlend, sampler, ColorMode.Sprite));

            float red = target.Pixel(7, 0).X;
            if (repeats)
            {
                Assert.True(red >= 50f, $"red {red}");
            }
            else
            {
                Assert.True(red <= Tolerance, $"red {red}");
            }
            texture.Dispose();
        }

        [Fact]
        public void ClipSpaceMapsToPixelsWithYDown()
        {
            // Clip x 0..1, y 0..1 is the top-right quarter.
            using RenderTarget target = new(8, 8, SKColors.Black);

            target.Draw(RenderTarget.Quad(0f, 1f, 1f, 0f, White), new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            Assert.Equal(255f, target.Pixel(6, 1).X);
            Assert.Equal(0f, target.Pixel(1, 1).X);
            Assert.Equal(0f, target.Pixel(6, 6).X);
        }

        [Fact]
        public void TheTransformIsAppliedBeforeClipSpace()
        {
            // An orthographic matrix over the 8×8 pixels, y down, as the engine's screen matrices are.
            using RenderTarget target = new(8, 8, SKColors.Black);
            Matrix4x4 screen = Matrix4x4.CreateOrthographicOffCenter(0f, 8f, 8f, 0f, 0f, 1f);
            Vertex[] quad = RenderTarget.Quad(4f, 0f, 8f, 4f, White);

            target.Renderer.DrawTriangles(quad, quad.Length, RenderTarget.QuadIndices, RenderTarget.QuadIndices.Length, screen,
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            Assert.Equal(255f, target.Pixel(6, 1).X);
            Assert.Equal(0f, target.Pixel(1, 1).X);
            Assert.Equal(0f, target.Pixel(6, 6).X);
        }

        [Fact]
        public void CreateTexturePremultipliesAlpha()
        {
            using RenderTarget target = new(1, 1, SKColors.Black);
            ITexture texture = target.Texture(1, 1, new SKColor(200, 100, 50, 128));

            target.Draw(RenderTarget.Quad(-1f, 1f, 1f, -1f, White), new DrawState(texture, BlendMode.AlphaBlend, SamplerMode.LinearClamp, ColorMode.Sprite));

            AssertRgb(new Vector3(200f, 100f, 50f) * (128f / 255f), target.Pixel(0, 0));
            Assert.Equal(1, texture.Width);
            Assert.Equal(1, texture.Height);
            texture.Dispose();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void PrimitiveWithAlphaBlendIsNotSupported(bool textured)
        {
            using RenderTarget target = new(1, 1, SKColors.Black);
            ITexture texture = textured ? target.Texture(1, 1, SKColors.White) : null;

            _ = Assert.Throws<NotSupportedException>(() =>
                target.Draw(RenderTarget.Quad(-1f, 1f, 1f, -1f, White), new DrawState(texture, BlendMode.AlphaBlend, SamplerMode.LinearClamp, ColorMode.Primitive)));
            texture?.Dispose();
        }

        [Fact]
        public void SpriteDrawsNeedATexture()
        {
            using RenderTarget target = new(1, 1, SKColors.Black);

            _ = Assert.Throws<ArgumentException>(() =>
                target.Draw(RenderTarget.Quad(-1f, 1f, 1f, -1f, White), new DrawState(null, BlendMode.AlphaBlend, SamplerMode.LinearClamp, ColorMode.Sprite)));
        }

        [Fact]
        public void UnreadableImagesAreRejected()
        {
            using SkiaRenderer renderer = new();
            using MemoryStream stream = new([1, 2, 3, 4, 5, 6, 7, 8]);

            _ = Assert.Throws<InvalidDataException>(() => renderer.CreateTexture(stream));
        }

        [Fact]
        public void DrawingBeforeSetTargetIsRejected()
        {
            using SkiaRenderer renderer = new();
            Vertex[] quad = RenderTarget.Quad(-1f, 1f, 1f, -1f, White);

            _ = Assert.Throws<InvalidOperationException>(() =>
                renderer.DrawTriangles(quad, quad.Length, RenderTarget.QuadIndices, RenderTarget.QuadIndices.Length, Matrix4x4.Identity,
                    new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive)));
        }

        [Fact]
        public void DrawingWithADisposedTextureIsRejected()
        {
            using RenderTarget target = new(1, 1, SKColors.Black);
            ITexture texture = target.Texture(1, 1, SKColors.White);
            texture.Dispose();

            Assert.True(texture.IsDisposed);
            _ = Assert.Throws<ObjectDisposedException>(() =>
                target.Draw(RenderTarget.Quad(-1f, 1f, 1f, -1f, White), new DrawState(texture, BlendMode.AlphaBlend, SamplerMode.LinearClamp, ColorMode.Sprite)));
        }

        [Fact]
        public void OnlyTheCountedVerticesAndIndicesAreDrawn()
        {
            // The first four vertices are the top-left quarter; four more, and six indices naming
            // them, cover everything and must be ignored.
            using RenderTarget target = new(8, 8, SKColors.Black);
            Vertex[] vertices = [.. RenderTarget.Quad(-1f, 1f, 0f, 0f, White), .. RenderTarget.Quad(-1f, 1f, 1f, -1f, new Color(255, 0, 0, 255))];
            short[] indices = [0, 1, 2, 1, 3, 2, 4, 5, 6, 5, 7, 6];

            target.Renderer.DrawTriangles(vertices, 4, indices, 6, Matrix4x4.Identity,
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            Assert.Equal(new Vector4(255f, 255f, 255f, 255f), target.Pixel(1, 1));
            Assert.Equal(new Vector4(0f, 0f, 0f, 255f), target.Pixel(6, 6));
        }

        [Fact]
        public void TrianglesWithANonFiniteVertexAreSkippedAndTheRestDrawn()
        {
            // Zero-size glyphs reach the renderer with NaN positions in the same draw as real ones.
            // A GPU drops just those triangles; the rest of the draw must still appear.
            using RenderTarget target = new(8, 8, SKColors.Black);
            Vertex[] broken = RenderTarget.Quad(float.NaN, float.NaN, float.NaN, float.NaN, White);
            Vertex[] vertices = [.. RenderTarget.Quad(-1f, 1f, 0f, 0f, White), .. broken];
            short[] indices = [0, 1, 2, 1, 3, 2, 4, 5, 6, 5, 7, 6];

            target.Renderer.DrawTriangles(vertices, vertices.Length, indices, indices.Length, Matrix4x4.Identity,
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            Assert.Equal(new Vector4(255f, 255f, 255f, 255f), target.Pixel(1, 1));
            Assert.Equal(new Vector4(0f, 0f, 0f, 255f), target.Pixel(6, 6));
        }

        [Fact]
        public void ATriangleWithOneNonFiniteVertexIsNotDrawn()
        {
            // Two corners on screen, one at NaN: nothing may appear, least of all a triangle reaching
            // toward wherever the NaN corner was parked.
            using RenderTarget target = new(8, 8, SKColors.Black);
            Vertex[] vertices =
            [
                new(new Vector3(1f, -1f, 0f), White, Vector2.Zero),
                new(new Vector3(1f, 1f, 0f), White, Vector2.Zero),
                new(new Vector3(float.NaN, 0f, 0f), White, Vector2.Zero),
            ];
            short[] indices = [0, 1, 2];

            target.Renderer.DrawTriangles(vertices, vertices.Length, indices, indices.Length, Matrix4x4.Identity,
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    Assert.Equal(0f, target.Pixel(x, y).X);
                }
            }
        }

        [Fact]
        public void IndicesPastShortMaxValueAreReadUnsigned()
        {
            // Indices are 16-bit unsigned, as the GPU reads them: a batch past 32767 vertices hands
            // over negative shorts that name vertices 32768 and up.
            using RenderTarget target = new(8, 8, SKColors.Black);
            const int first = 40000;
            Vertex[] vertices = new Vertex[first + 4];
            RenderTarget.Quad(-1f, 1f, 0f, 0f, White).CopyTo(vertices, first);
            short[] indices = [.. Array.ConvertAll(RenderTarget.QuadIndices, i => unchecked((short)(first + i)))];

            target.Renderer.DrawTriangles(vertices, vertices.Length, indices, indices.Length, Matrix4x4.Identity,
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            Assert.Equal(new Vector4(255f, 255f, 255f, 255f), target.Pixel(1, 1));
        }

        [Fact]
        public void ConsecutiveDrawsOfTheSameSizeDrawTheirOwnData()
        {
            using RenderTarget target = new(8, 1, SKColors.Black);
            DrawState state = new(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive);

            target.Draw(RenderTarget.Quad(-1f, 1f, 0f, -1f, White), state);
            target.Draw(RenderTarget.Quad(0f, 1f, 1f, -1f, new Color(255, 0, 0, 255)), state);

            Assert.Equal(new Vector4(255f, 255f, 255f, 255f), target.Pixel(1, 0));
            Assert.Equal(new Vector4(255f, 0f, 0f, 255f), target.Pixel(6, 0));
        }

        [Fact]
        public void UntexturedDrawsPickUpAChangedOpacity()
        {
            // White at opacity 0.5 under NonPremultiplied over black: 0.5 · 0.5 = 0.25.
            using RenderTarget target = new(8, 1, SKColors.Black);

            target.Draw(RenderTarget.Quad(-1f, 1f, 0f, -1f, White), new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive, 1f));
            target.Draw(RenderTarget.Quad(0f, 1f, 1f, -1f, White), new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive, 0.5f));

            Assert.InRange(target.Pixel(1, 0).X, 255f - Tolerance, 255f);
            Assert.InRange(target.Pixel(6, 0).X, (0.25f * 255f) - Tolerance, (0.25f * 255f) + Tolerance);
        }

        [Fact]
        public void EmptyDrawsLeaveTheTargetUntouched()
        {
            using RenderTarget target = new(2, 2, Background);
            Vertex[] quad = RenderTarget.Quad(-1f, 1f, 1f, -1f, White);

            target.Renderer.DrawTriangles(quad, quad.Length, RenderTarget.QuadIndices, 0, Matrix4x4.Identity,
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            Assert.Equal(new Vector4(Background.Red, Background.Green, Background.Blue, 255f), target.Pixel(0, 0));
        }

        [Fact]
        public void SetTargetRedirectsLaterDraws()
        {
            using RenderTarget first = new(2, 2, SKColors.Black);
            using SKSurface second = SKSurface.Create(new SKImageInfo(2, 2, SKColorType.Rgba8888, SKAlphaType.Premul));
            second.Canvas.Clear(SKColors.Black);

            first.Renderer.SetTarget(second.Canvas, 2, 2);
            first.Draw(RenderTarget.Quad(-1f, 1f, 1f, -1f, White), new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearWrap, ColorMode.Primitive));

            Assert.Equal(0f, first.Pixel(0, 0).X);
            using SKPixmap pixmap = second.PeekPixels();
            Assert.Equal(255, pixmap.GetPixelSpan()[0]);
        }

        private sealed class ForeignFace : IFontFace
        {
            public FontMetrics GetMetrics(float size)
            {
                return default;
            }

            public float MeasureText(string text, float size)
            {
                return 0f;
            }

            public void Dispose()
            {
            }
        }

        private static IFontFace PatrickHand(RenderTarget target)
        {
            using FileStream stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "fonts", "PatrickHand-Regular.ttf"));
            return target.Renderer.CreateFontFace(stream);
        }

        // Text space in pixels, origin at the target's center. yDirection 1 keeps text space's y pointing
        // down the screen (so −y, where glyphs rise, is up); −1 flips it.
        private static Matrix4x4 TextTransform(RenderTarget target, float yDirection)
        {
            return Matrix4x4.CreateScale(2f / target.Width, -2f * yDirection / target.Height, 1f);
        }

        // Inked pixels (red above half) in the rows above and below the center row.
        private static (int Above, int Below) InkAroundCenterRow(RenderTarget target)
        {
            int above = 0;
            int below = 0;
            int middle = target.Height / 2;
            for (int y = 0; y < target.Height; y++)
            {
                for (int x = 0; x < target.Width; x++)
                {
                    if (target.Pixel(x, y).X > 128f)
                    {
                        if (y < middle)
                        {
                            above++;
                        }
                        else if (y > middle)
                        {
                            below++;
                        }
                    }
                }
            }
            return (above, below);
        }

        [Fact]
        public void CreateFontFaceRejectsDataThatIsNotAFont()
        {
            using RenderTarget target = new(1, 1, SKColors.Black);
            using MemoryStream stream = new([1, 2, 3, 4, 5, 6, 7, 8]);

            _ = Assert.Throws<InvalidDataException>(() => target.Renderer.CreateFontFace(stream));
        }

        [Fact]
        public void FaceMetricsAndWidthsScaleWithSize()
        {
            using RenderTarget target = new(1, 1, SKColors.Black);
            using IFontFace face = PatrickHand(target);

            FontMetrics large = face.GetMetrics(100f);
            FontMetrics small = face.GetMetrics(50f);

            Assert.True(large.Ascent < 0f && large.Descent > 0f, $"ascent {large.Ascent}, descent {large.Descent}");
            Assert.Equal((large.Descent - large.Ascent) / 2f, small.Descent - small.Ascent, 0.5f);
            Assert.Equal(face.MeasureText("Contre Jour DX", 100f) / 2f, face.MeasureText("Contre Jour DX", 50f), 1f);
            Assert.Equal(0f, face.MeasureText(string.Empty, 100f));
        }

        [Theory]
        [InlineData(1f, true)]
        [InlineData(-1f, false)]
        public void TextRisesTowardNegativeYOfItsSpace(float yDirection, bool inkAbove)
        {
            using RenderTarget target = new(64, 64, SKColors.Black);
            using IFontFace face = PatrickHand(target);

            target.Renderer.DrawText(face, "T", 40f, TextTransform(target, yDirection), White);

            (int above, int below) = InkAroundCenterRow(target);
            bool ok = inkAbove ? above > 50 && below * 10 < above : below > 50 && above * 10 < below;
            Assert.True(ok, $"above {above}, below {below}");
        }

        [Fact]
        public void TextColorFollowsTheSpriteContract()
        {
            // Where a glyph covers a pixel fully, text must equal a white texel drawn as a sprite with the
            // same vertex color. Opaque white on black first marks those pixels.
            using RenderTarget mask = new(64, 64, SKColors.Black);
            using RenderTarget target = new(64, 64, Background);
            using IFontFace maskFace = PatrickHand(mask);
            using IFontFace face = PatrickHand(target);
            Color color = new(204, 153, 102, 128);

            mask.Renderer.DrawText(maskFace, "H", 48f, TextTransform(mask, 1f), White);
            target.Renderer.DrawText(face, "H", 48f, TextTransform(target, 1f), color);

            Vector4 c = new(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
            Vector4 dst = new(Background.Red / 255f, Background.Green / 255f, Background.Blue / 255f, 1f);
            Vector3 expected = Expected(Vector4.One, c, ColorMode.Sprite, BlendMode.AlphaBlend, 1f, dst);
            int covered = 0;
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    if (mask.Pixel(x, y).X == 255f)
                    {
                        AssertRgb(expected, target.Pixel(x, y));
                        covered++;
                    }
                }
            }
            Assert.True(covered > 10, $"covered {covered}");
        }

        [Fact]
        public void CharactersTheFaceLacksStillMeasureAndDraw()
        {
            // Patrick Hand has no Hangul: a modded string falls back to the face's missing-glyph box.
            using RenderTarget target = new(64, 64, SKColors.Black);
            using IFontFace face = PatrickHand(target);

            Assert.True(face.MeasureText("가", 40f) > 0f);
            target.Renderer.DrawText(face, "가", 40f, TextTransform(target, 1f), White);
        }

        [Fact]
        public void FaceOutlivesATargetChange()
        {
            using RenderTarget target = new(64, 64, SKColors.Black);
            using IFontFace face = PatrickHand(target);
            using SKSurface other = SKSurface.Create(new SKImageInfo(64, 64, SKColorType.Rgba8888, SKAlphaType.Premul));
            other.Canvas.Clear(SKColors.Black);

            target.Renderer.SetTarget(other.Canvas, 64, 64);
            target.Renderer.DrawText(face, "T", 40f, TextTransform(target, 1f), White);

            using SKPixmap pixmap = other.PeekPixels();
            ReadOnlySpan<byte> bytes = pixmap.GetPixelSpan();
            bool inked = false;
            for (int i = 0; i < bytes.Length; i += 4)
            {
                inked |= bytes[i] > 128;
            }
            Assert.True(inked);
        }

        [Fact]
        public void DrawTextRejectsForeignAndDisposedFaces()
        {
            using RenderTarget target = new(8, 8, SKColors.Black);
            IFontFace face = PatrickHand(target);
            face.Dispose();

            _ = Assert.Throws<ArgumentException>(() => target.Renderer.DrawText(new ForeignFace(), "T", 10f, Matrix4x4.Identity, White));
            _ = Assert.Throws<ObjectDisposedException>(() => target.Renderer.DrawText(face, "T", 10f, Matrix4x4.Identity, White));
        }
    }
}
