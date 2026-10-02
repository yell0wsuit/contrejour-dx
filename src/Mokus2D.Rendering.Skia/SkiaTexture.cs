using Mokus2D.Graphics;

using SkiaSharp;

namespace Mokus2D.Rendering.Skia
{
    // A texture on a Skia image with premultiplied pixels, and the shaders draws sample it through.
    internal sealed class SkiaTexture(SKImage image) : ITexture
    {
        // Turns every color channel into the alpha and sets alpha to one. Skia applies color matrices
        // to straight colors, so multiplying the texture by the result weights its color by its alpha
        // and leaves its alpha as it was.
        private static readonly float[] AlphaToColor =
        [
            0f, 0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f, 0f,
            0f, 0f, 0f, 1f, 0f,
            0f, 0f, 0f, 0f, 1f,
        ];

        private static readonly SKSamplingOptions Bilinear = new(SKFilterMode.Linear, SKMipmapMode.None);

        // Indexed by slot: clamp or repeat, plain or weighted by alpha. Built on first use.
        private readonly SKShader[] _shaders = new SKShader[4];

        public SKImage Image { get; } = image;

        public string Name { get; set; }

        public int Width => Image.Width;

        public int Height => Image.Height;

        public bool IsDisposed { get; private set; }

        // The shader a draw samples this texture through. Texture coordinates are 0-1 across the
        // image. weightedByAlpha gives (tex.rgb·tex.a, tex.a), for blend modes that weight the source
        // by its own alpha.
        internal SKShader Shader(SamplerMode sampler, bool weightedByAlpha)
        {
            int slot = (sampler == SamplerMode.LinearWrap ? 2 : 0) + (weightedByAlpha ? 1 : 0);
            return _shaders[slot] ??= CreateShader(sampler, weightedByAlpha);
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;
            for (int i = 0; i < _shaders.Length; i++)
            {
                _shaders[i]?.Dispose();
                _shaders[i] = null;
            }
            Image.Dispose();
        }

        private SKShader CreateShader(SamplerMode sampler, bool weightedByAlpha)
        {
            SKShaderTileMode tile = sampler == SamplerMode.LinearWrap ? SKShaderTileMode.Repeat : SKShaderTileMode.Clamp;
            // The local matrix maps 0-1 texture coordinates onto the image's pixels.
            SKMatrix unitToPixels = SKMatrix.CreateScale(1f / Width, 1f / Height);
            SKShader plain = SKShader.CreateImage(Image, tile, tile, Bilinear, unitToPixels);
            if (!weightedByAlpha)
            {
                return plain;
            }
            using SKColorFilter alphaToColor = SKColorFilter.CreateColorMatrix(AlphaToColor);
            using SKShader alpha = plain.WithColorFilter(alphaToColor);
            SKShader weighted = SKShader.CreateBlend(SKBlendMode.Modulate, plain, alpha);
            plain.Dispose();
            return weighted;
        }
    }
}
