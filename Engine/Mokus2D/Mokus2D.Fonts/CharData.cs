using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Visual.Interfaces;

namespace Mokus2D.Fonts;

public class CharData : ISpriteData, ITextureNodeData, IConfig
{
	private readonly FontData _font;

	public readonly float Width;

	public string Id => _font.Id;

	public IDictionary<string, string> Config => null;

	public string TextureName => _font.TextureName;

	public Texture2D Texture
	{
		get
		{
			return _font.Texture;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public float ScaleFactor => _font.ScaleFactor;

	public Vector2 Anchor { get; private set; }

	public Rectangle TextureRect { get; private set; }

	public CharData(FontData font, Rectangle textureRect, Vector2 anchor, float width)
	{
		_font = font;
		TextureRect = textureRect;
		Anchor = new Vector2(anchor.X / (float)textureRect.Width, anchor.Y / (float)textureRect.Height);
		Width = width;
	}
}
