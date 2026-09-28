using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Shaders;

public class BlurEffect : TextureMatrixEffectBase
{
	private readonly EffectParameter _pixelWidth;

	private readonly EffectParameter _isVertical;

	public bool IsVertical
	{
		get
		{
			return _isVertical.GetValueBoolean();
		}
		set
		{
			_isVertical.SetValue(value);
		}
	}

	public bool IsHorizontal
	{
		get
		{
			return !IsVertical;
		}
		set
		{
			IsVertical = !value;
		}
	}

	public BlurEffect()
		: base("Mokus2D.Shaders.Blur")
	{
		_pixelWidth = base.Parameters["PixelWidth"];
		_isVertical = base.Parameters["IsVertical"];
	}

	public override void Apply(Matrix matrix, Texture2D texture)
	{
		_pixelWidth.SetValue(1f / (float)texture.Width);
		base.Apply(matrix, texture);
	}

	protected override void ApplyPasses()
	{
		foreach (EffectPass pass in Effect.CurrentTechnique.Passes)
		{
			pass.Apply();
		}
	}
}
