using Microsoft.Xna.Framework;

using Mokus2D.Graphics;

namespace Mokus2D.Visual.Animation
{
    public static class TextureExtensions
    {
        public static Vector2 Size(this ITexture texture)
        {
            return new Vector2(texture.Width, texture.Height);
        }

        public static Vector2 GetTextureCoords(this ITexture texture, Vector2 position)
        {
            return position / texture.Size();
        }
    }
}
