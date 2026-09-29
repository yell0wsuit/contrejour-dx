using System.IO;

using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Graphics.MonoGame
{
    // IRenderer on a MonoGame GraphicsDevice. It lives in the engine only while the rendering seam is
    // being built; it moves into the desktop host once nothing in the engine needs MonoGame graphics.
    public sealed class MonoGameRenderer(GraphicsDevice device) : IRenderer
    {
        public ITexture CreateTexture(Stream stream)
        {
            // Sprites are drawn with premultiplied-alpha blending, as MonoGame's content loader
            // prepared raw image files; straight alpha shows white fringes around soft edges.
            return new MonoGameTexture(Texture2D.FromStream(device, stream, DefaultColorProcessors.PremultiplyAlpha));
        }
    }
}
