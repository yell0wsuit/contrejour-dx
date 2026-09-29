using System;
using System.IO;

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
    }
}
