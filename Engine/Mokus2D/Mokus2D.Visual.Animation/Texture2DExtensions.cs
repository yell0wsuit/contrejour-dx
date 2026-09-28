using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Animation;

public static class Texture2DExtensions
{
	public static Vector2 Size(this Texture2D texture)
	{
		return new Vector2(texture.Width, texture.Height);
	}

	public static Vector2 GetTextureCoords(this Texture2D texture, Vector2 position)
	{
		return position / texture.Size();
	}
}
