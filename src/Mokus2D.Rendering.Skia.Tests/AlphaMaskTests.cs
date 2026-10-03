using System.Numerics;

using Mokus2D.Graphics;

using SkiaSharp;

using Xunit;

namespace Mokus2D.Rendering.Skia.Tests
{
    public class AlphaMaskTests
    {
        [Fact]
        public void MaskHidesPupilOutsideAnimatedEyeWithoutErasingBackground()
        {
            using RenderTarget target = new(8, 8, SKColors.Blue);
            using ITexture mask = target.Texture(4, 4,
                SKColors.Black, SKColors.Black, SKColors.Black, SKColors.Black,
                SKColors.Black, SKColors.Black, SKColors.Black, SKColors.Black,
                SKColors.Transparent, SKColors.Transparent, SKColors.Transparent, SKColors.Transparent,
                SKColors.Transparent, SKColors.Transparent, SKColors.Transparent, SKColors.Transparent);
            Vertex[] maskQuad = RenderTarget.Quad(-0.5f, 0.5f, 0.5f, -0.5f, Color.White);
            target.Renderer.BeginAlphaMask(maskQuad, mask, Matrix4x4.Identity);
            target.Draw(RenderTarget.Quad(-1, 1, 1, -1, new Color(0, 255, 0)),
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearClamp, ColorMode.Primitive));
            target.Renderer.EndAlphaMask();
            Assert.Equal(new Vector4(0, 255, 0, 255), target.Pixel(3, 2));
            Assert.Equal(new Vector4(0, 0, 255, 255), target.Pixel(3, 5));
            Assert.Equal(new Vector4(0, 0, 255, 255), target.Pixel(0, 2));
            // Closing the mask must restore drawing outside its bounds.
            target.Draw(RenderTarget.Quad(-1, 1, -0.5f, -1, new Color(255, 0, 0)),
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearClamp, ColorMode.Primitive));
            Assert.Equal(new Vector4(255, 0, 0, 255), target.Pixel(0, 2));
        }

        [Fact]
        public void MaskPreservesFractionalCoverageAndIgnoresItsRgb()
        {
            using RenderTarget target = new(8, 8, SKColors.Blue);
            using ITexture mask = target.Texture(1, 1, new SKColor(0, 0, 0, 128));
            target.Renderer.BeginAlphaMask(RenderTarget.Quad(-1, 1, 1, -1, new Color(255, 0, 0, 0)), mask, Matrix4x4.Identity);
            target.Draw(RenderTarget.Quad(-1, 1, 1, -1, new Color(0, 255, 0)),
                new DrawState(null, BlendMode.NonPremultiplied, SamplerMode.LinearClamp, ColorMode.Primitive));
            target.Renderer.EndAlphaMask();
            Assert.Equal(new Vector4(0, 128, 127, 255), target.Pixel(4, 4));
        }

        [Fact]
        public void RotatedNestedMasksRestoreTheirParentClip()
        {
            using RenderTarget target = new(16, 16, SKColors.Blue);
            using ITexture mask = target.Texture(1, 1, SKColors.Black);
            Vertex[] quad = RenderTarget.Quad(-0.5f, 0.5f, 0.5f, -0.5f, Color.White);
            target.Renderer.BeginAlphaMask(quad, mask, Matrix4x4.CreateRotationZ(float.Pi / 4));
            target.Renderer.BeginAlphaMask(quad, mask, Matrix4x4.CreateScale(0.5f));
            DrawState green = new(null, BlendMode.NonPremultiplied, SamplerMode.LinearClamp, ColorMode.Primitive);
            target.Draw(RenderTarget.Quad(-1, 1, 1, -1, new Color(0, 255, 0)), green);
            target.Renderer.EndAlphaMask();
            target.Draw(RenderTarget.Quad(-1, 1, 0, -1, new Color(255, 0, 0)), green);
            target.Renderer.EndAlphaMask();
            Assert.Equal(new Vector4(0, 255, 0, 255), target.Pixel(8, 8));
            Assert.Equal(new Vector4(255, 0, 0, 255), target.Pixel(4, 8));
            Assert.Equal(new Vector4(0, 0, 255, 255), target.Pixel(4, 4));
        }
    }
}
