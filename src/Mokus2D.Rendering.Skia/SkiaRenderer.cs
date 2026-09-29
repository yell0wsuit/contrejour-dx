using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;

using Mokus2D.Graphics;

using SkiaSharp;

namespace Mokus2D.Rendering.Skia
{
    // IRenderer on a Skia canvas. Each DrawTriangles call is one DrawVertices: the engine batches
    // before it gets here. DrawState's enums define the pixel math; Skia's vertex draw computes
    // shader × premultiplied vertex color, so each case below picks the shader and the vertex
    // colors that make that product equal the contract's source.
    public sealed class SkiaRenderer : IRenderer, IDisposable
    {
        // Untextured primitives need vertex color interpolated straight, as GL does, and Skia
        // interpolates it premultiplied. Their vertex color carries rgb with alpha 255 and their
        // texture coordinate carries alpha; Skia interpolates both linearly, and this shader rebuilds
        // the source weighted by its own alpha, (S.rgb·S.a, S.a) with S = c·opacity, through the
        // Modulate with the vertex color.
        private const string PrimitiveShaderSource =
            "uniform float opacity;" +
            "half4 main(float2 p) { float a = p.x * opacity; float k = opacity * a; return half4(k, k, k, a); }";

        // Draw sizes that keep their buffers. Past this, a draw of a new size allocates its own.
        private const int MaximumCachedSizes = 64;

        private readonly SKRuntimeEffect _primitiveEffect;

        private readonly SKPaint _paint = new() { Color = SKColors.White };

        private readonly Dictionary<int, VertexBuffers> _vertexBuffers = [];

        private readonly Dictionary<int, ushort[]> _indexBuffers = [];

        private SKShader _primitiveShader;

        private float _primitiveOpacity;

        private SKCanvas _canvas;

        private int _width;

        private int _height;

        public SkiaRenderer()
        {
            _primitiveEffect = SKRuntimeEffect.CreateShader(PrimitiveShaderSource, out string errors)
                ?? throw new InvalidOperationException($"Skia rejected the primitive shader: {errors}");
        }

        // SKVertices reads each array's whole length as the vertex count, so buffers are exact-size.
        private readonly record struct VertexBuffers(SKPoint[] Positions, SKPoint[] TextureCoordinates, SKColor[] Colors);

        // Where draws go from now on: a canvas of width × height pixels. The renderer never clears it.
        public void SetTarget(SKCanvas canvas, int width, int height)
        {
            ArgumentNullException.ThrowIfNull(canvas);
            _canvas = canvas;
            _width = width;
            _height = height;
        }

        public ITexture CreateTexture(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            using SKCodec codec = SKCodec.Create(stream) ?? throw new InvalidDataException("The stream is not a supported image.");
            SKImageInfo info = new(codec.Info.Width, codec.Info.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
            using SKBitmap bitmap = SKBitmap.Decode(codec, info) ?? throw new InvalidDataException("The image could not be decoded.");
            SKImage image = SKImage.FromBitmap(bitmap) ?? throw new InvalidDataException("The image could not be decoded.");
            return new SkiaTexture(image);
        }

        public void DrawTriangles(Vertex[] vertices, int vertexCount, short[] indices, int indexCount, in Matrix4x4 transform, in DrawState state)
        {
            if (_canvas == null)
            {
                throw new InvalidOperationException("SetTarget must be called before drawing.");
            }
            SkiaTexture texture = state.Texture == null ? null : state.Texture as SkiaTexture
                ?? throw new ArgumentException("The texture was not created by this renderer.", nameof(state));
            if (texture == null && state.ColorMode == ColorMode.Sprite)
            {
                throw new ArgumentException("Sprite draws need a texture.", nameof(state));
            }
            bool weighted = state.Blend != BlendMode.AlphaBlend;
            if (state.ColorMode == ColorMode.Primitive && !weighted)
            {
                throw new NotSupportedException("Primitive draws with AlphaBlend have no premultiplied equivalent.");
            }
            ObjectDisposedException.ThrowIf(texture?.IsDisposed == true, state.Texture);
            if (indexCount < 3)
            {
                return;
            }

            float opacity = Math.Clamp(state.Opacity, 0f, 1f);
            Matrix3x2 toPixels = ToPixels(transform);
            VertexBuffers buffers = VertexBuffersFor(vertexCount);
            for (int i = 0; i < vertexCount; i++)
            {
                Vertex vertex = vertices[i];
                Vector2 position = Vector2.Transform(new Vector2(vertex.Position.X, vertex.Position.Y), toPixels);
                buffers.Positions[i] = new SKPoint(position.X, position.Y);
                if (texture == null)
                {
                    buffers.TextureCoordinates[i] = new SKPoint(vertex.Color.A / 255f, 0f);
                    buffers.Colors[i] = new SKColor(vertex.Color.R, vertex.Color.G, vertex.Color.B, 255);
                }
                else
                {
                    buffers.TextureCoordinates[i] = new SKPoint(vertex.TextureCoordinate.X, vertex.TextureCoordinate.Y);
                    buffers.Colors[i] = TexturedVertexColor(vertex.Color, state.ColorMode, weighted, opacity);
                }
            }
            // A GPU drops a triangle with a non-finite vertex and draws the rest; Skia would drop the
            // whole draw, since its bounds are no longer finite. The engine does send such triangles:
            // zero-size glyphs come out at NaN in the same draw as a label's visible ones.
            int keptCount = 0;
            for (int i = 0; i + 2 < indexCount; i += 3)
            {
                if (IsFinite(buffers.Positions, indices, i))
                {
                    keptCount += 3;
                }
            }
            if (keptCount == 0)
            {
                return;
            }
            ushort[] triangles = IndexBufferFor(keptCount);
            int kept = 0;
            for (int i = 0; i + 2 < indexCount; i += 3)
            {
                if (IsFinite(buffers.Positions, indices, i))
                {
                    triangles[kept] = (ushort)indices[i];
                    triangles[kept + 1] = (ushort)indices[i + 1];
                    triangles[kept + 2] = (ushort)indices[i + 2];
                    kept += 3;
                }
            }
            // Skia bounds the draw by every position, drawn or not, so the dropped ones move to a
            // finite point no kept triangle uses.
            for (int i = 0; i < vertexCount; i++)
            {
                SKPoint position = buffers.Positions[i];
                if (!float.IsFinite(position.X) || !float.IsFinite(position.Y))
                {
                    buffers.Positions[i] = SKPoint.Empty;
                }
            }
            if (texture != null)
            {
                AssertUniformPerTriangle(buffers.Colors, triangles);
            }

            _paint.BlendMode = state.Blend == BlendMode.Additive ? SKBlendMode.Plus : SKBlendMode.SrcOver;
            _paint.Shader = texture?.Shader(state.Sampler, weighted) ?? PrimitiveShader(opacity);
            using SKVertices skiaVertices = SKVertices.CreateCopy(SKVertexMode.Triangles, buffers.Positions, buffers.TextureCoordinates, buffers.Colors, triangles)
                ?? throw new InvalidOperationException("Skia could not create the vertex set.");
            _canvas.DrawVertices(skiaVertices, SKBlendMode.Modulate, _paint);
            // The paint would otherwise keep the texture's shader, and through it the image, alive.
            _paint.Shader = null;
        }

        public void Dispose()
        {
            _paint.Dispose();
            _primitiveShader?.Dispose();
            _primitiveShader = null;
            _primitiveEffect.Dispose();
        }

        // Textured draws: the texture shader supplies tex (or its alpha-weighted form) and the vertex
        // color supplies the rest. For Sprite, c for AlphaBlend and (c.rgb·c.a, c.a) when weighted;
        // for Primitive (weighted only), c·opacity. Skia premultiplies these straight colors, which is
        // exact when a triangle's three vertex colors are equal.
        private static SKColor TexturedVertexColor(Color color, ColorMode mode, bool weighted, float opacity)
        {
            if (mode == ColorMode.Primitive)
            {
                return new SKColor(Scale(color.R, opacity), Scale(color.G, opacity), Scale(color.B, opacity), Scale(color.A, opacity));
            }
            float weight = weighted ? color.A / 255f : 1f;
            return new SKColor(Scale(color.R, weight), Scale(color.G, weight), Scale(color.B, weight), color.A);
        }

        private static bool IsFinite(SKPoint[] positions, short[] indices, int first)
        {
            for (int i = first; i < first + 3; i++)
            {
                SKPoint position = positions[(ushort)indices[i]];
                if (!float.IsFinite(position.X) || !float.IsFinite(position.Y))
                {
                    return false;
                }
            }
            return true;
        }

        private static byte Scale(byte channel, float factor)
        {
            return (byte)MathF.Round(channel * factor);
        }

        [Conditional("DEBUG")]
        private static void AssertUniformPerTriangle(SKColor[] colors, ushort[] triangles)
        {
            for (int i = 0; i + 2 < triangles.Length; i += 3)
            {
                SKColor first = colors[triangles[i]];
                Debug.Assert(first == colors[triangles[i + 1]] && first == colors[triangles[i + 2]],
                    "A textured triangle mixes vertex colors; Skia would interpolate them premultiplied.");
            }
        }

        // transform maps to clip space; its x/y rows and translation are the whole map for the
        // engine's orthographic matrices. Clip space then maps to pixels with y pointing down.
        private Matrix3x2 ToPixels(in Matrix4x4 transform)
        {
            Matrix3x2 affine = new(transform.M11, transform.M12, transform.M21, transform.M22, transform.M41, transform.M42);
            Matrix3x2 clipToPixels = new(_width / 2f, 0f, 0f, -_height / 2f, _width / 2f, _height / 2f);
            return affine * clipToPixels;
        }

        private SKShader PrimitiveShader(float opacity)
        {
            if (_primitiveShader == null || opacity != _primitiveOpacity)
            {
                _primitiveShader?.Dispose();
                using SKRuntimeEffectUniforms uniforms = new(_primitiveEffect) { ["opacity"] = opacity };
                _primitiveShader = _primitiveEffect.ToShader(uniforms);
                _primitiveOpacity = opacity;
            }
            return _primitiveShader;
        }

        private VertexBuffers VertexBuffersFor(int count)
        {
            if (_vertexBuffers.TryGetValue(count, out VertexBuffers cached))
            {
                return cached;
            }
            VertexBuffers buffers = new(new SKPoint[count], new SKPoint[count], new SKColor[count]);
            if (_vertexBuffers.Count < MaximumCachedSizes)
            {
                _vertexBuffers[count] = buffers;
            }
            return buffers;
        }

        private ushort[] IndexBufferFor(int count)
        {
            if (_indexBuffers.TryGetValue(count, out ushort[] cached))
            {
                return cached;
            }
            ushort[] buffer = new ushort[count];
            if (_indexBuffers.Count < MaximumCachedSizes)
            {
                _indexBuffers[count] = buffer;
            }
            return buffer;
        }
    }
}
