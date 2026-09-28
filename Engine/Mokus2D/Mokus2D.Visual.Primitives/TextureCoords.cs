using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Primitives;

public class TextureCoords
{
	private Vector2 lt;

	private Vector2 size;

	public Vector2 Lt => lt;

	public Vector2 Size => size;

	public void Refresh(Texture2D texture, Rectangle textureCoords, Vector2 margins = default(Vector2))
	{
		int width = texture.Width;
		int height = texture.Height;
		lt = new Vector2(((float)textureCoords.Left + margins.X) / (float)width, ((float)textureCoords.Top + margins.X) / (float)height);
		size = new Vector2(((float)textureCoords.Width - margins.X * 2f) / (float)width, ((float)textureCoords.Height - margins.Y * 2f) / (float)height);
	}

	public Vector2 GetTexturePosition(Vector2 position)
	{
		return Lt + Size * position;
	}
}
