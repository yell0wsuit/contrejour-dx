using System;
using System.IO;
using System.Numerics;

namespace Mokus2D.Graphics
{
    // A texture a renderer created. Its pixels are premultiplied by alpha.
    public interface ITexture : IDisposable
    {
        // The engine's asset name for the texture; the renderer only stores it.
        string Name { get; set; }

        int Width { get; }

        int Height { get; }

        bool IsDisposed { get; }
    }

    // The platform half of drawing. The engine batches and transforms; this only talks to the GPU.
    public interface IRenderer
    {
        // Decodes an image file (PNG). The pixels must come back premultiplied by alpha: every blend
        // mode assumes it. Throws InvalidDataException when the stream is not a supported image. The
        // caller owns and disposes the texture.
        ITexture CreateTexture(Stream stream);

        // Loads a font file (TrueType or OpenType). Throws InvalidDataException when the stream is not
        // a font this renderer can read. The caller owns and disposes the face.
        IFontFace CreateFontFace(Stream stream);

        // Draws indexCount / 3 triangles, in index order, both windings, no depth test. transform
        // maps Position to clip space. A triangle with a non-finite vertex position is skipped, as a
        // GPU rasterizer drops it, and the rest are still drawn. See DrawState's enums for the pixel
        // math.
        void DrawTriangles(Vertex[] vertices, int vertexCount, short[] indices, int indexCount, in Matrix4x4 transform, in DrawState state);

        // Draws one line of text whose baseline starts at the origin of transform's space, which
        // transform maps to clip space; glyphs advance toward +x and rise toward −y there. color is
        // straight alpha: the result is what ColorMode.Sprite gives for a white texel scaled by the
        // glyph's coverage, blended with BlendMode.AlphaBlend.
        void DrawText(IFontFace face, string text, float size, in Matrix4x4 transform, Color color);
    }
}
