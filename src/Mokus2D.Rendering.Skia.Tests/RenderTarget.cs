using System;
using System.IO;
using System.Numerics;

using Mokus2D.Graphics;

using SkiaSharp;

namespace Mokus2D.Rendering.Skia.Tests
{
    // A small CPU surface cleared to a background, with a renderer drawing into it.
    internal sealed class RenderTarget : IDisposable
    {
        // Two triangles over a quad given as top-left, top-right, bottom-left, bottom-right.
        public static readonly short[] QuadIndices = [0, 1, 2, 1, 3, 2];

        public RenderTarget(int width, int height, SKColor background)
        {
            Width = width;
            Height = height;
            Surface = SKSurface.Create(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Premul));
            Surface.Canvas.Clear(background);
            Renderer = new SkiaRenderer();
            Renderer.SetTarget(Surface.Canvas, width, height);
        }

        public int Width { get; }

        public int Height { get; }

        public SKSurface Surface { get; }

        public SkiaRenderer Renderer { get; }

        // A quad from (left, top) to (right, bottom) in clip space (y up), texture coordinates 0 to
        // (uMax, 1), every vertex the given color unless the right-hand pair is given its own.
        public static Vertex[] Quad(float left, float top, float right, float bottom, Color color, Color? rightColor = null, float uMax = 1f)
        {
            Color rightSide = rightColor ?? color;
            return
            [
                new Vertex(new Vector3(left, top, 0f), color, new Vector2(0f, 0f)),
                new Vertex(new Vector3(right, top, 0f), rightSide, new Vector2(uMax, 0f)),
                new Vertex(new Vector3(left, bottom, 0f), color, new Vector2(0f, 1f)),
                new Vertex(new Vector3(right, bottom, 0f), rightSide, new Vector2(uMax, 1f)),
            ];
        }

        public void Draw(Vertex[] vertices, DrawState state)
        {
            Renderer.DrawTriangles(vertices, vertices.Length, QuadIndices, QuadIndices.Length, Matrix4x4.Identity, state);
        }

        // The pixel at (x, y), premultiplied, channels 0-255.
        public Vector4 Pixel(int x, int y)
        {
            using SKPixmap pixmap = Surface.PeekPixels();
            ReadOnlySpan<byte> bytes = pixmap.GetPixelSpan();
            int i = ((y * Width) + x) * 4;
            return new Vector4(bytes[i], bytes[i + 1], bytes[i + 2], bytes[i + 3]);
        }

        // A texture from straight-alpha pixels, row by row, through a PNG as the game loads them.
        public ITexture Texture(int width, int height, params SKColor[] straightPixels)
        {
            using SKBitmap bitmap = new(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul));
            for (int i = 0; i < straightPixels.Length; i++)
            {
                bitmap.SetPixel(i % width, i / width, straightPixels[i]);
            }
            using SKData png = bitmap.Encode(SKEncodedImageFormat.Png, 100);
            using MemoryStream stream = new(png.ToArray());
            return Renderer.CreateTexture(stream);
        }

        public void Dispose()
        {
            Renderer.Dispose();
            Surface.Dispose();
        }
    }
}
