using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Graphics;

namespace ContreJour.Desktop.MonoGame
{
    public sealed class MonoGameTexture(Texture2D texture) : ITexture
    {
        public Texture2D Texture { get; } = texture;

        public string Name { get; set; }

        public int Width => Texture.Width;

        public int Height => Texture.Height;

        public bool IsDisposed => Texture.IsDisposed;

        // The MonoGame texture behind an engine texture; null for null.
        internal static Texture2D Unwrap(ITexture texture)
        {
            return ((MonoGameTexture)texture)?.Texture;
        }

        public void Dispose()
        {
            Texture.Dispose();
        }
    }
}
