using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Graphics.MonoGame
{
    public sealed class MonoGameTexture(Texture2D texture) : ITexture
    {
        public Texture2D Texture { get; } = texture;

        public string Name { get; set; }

        public int Width => Texture.Width;

        public int Height => Texture.Height;

        public bool IsDisposed => Texture.IsDisposed;

        // The MonoGame texture behind an engine texture; null for null.
        public static Texture2D Unwrap(ITexture texture)
        {
            return ((MonoGameTexture)texture)?.Texture;
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
