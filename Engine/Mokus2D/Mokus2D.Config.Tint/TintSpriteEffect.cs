using Microsoft.Xna.Framework.Graphics;
using Mokus2D.Visual.Shaders;

namespace Mokus2D.Config.Tint;

public class TintSpriteEffect : TextureMatrixEffectBase
{
	private readonly EffectParameter _tintEnabled;

	public bool TintEnabled
	{
		get
		{
			return _tintEnabled.GetValueBoolean();
		}
		set
		{
			_tintEnabled.SetValue(value);
		}
	}

	public TintSpriteEffect()
		: base("Mokus2D.Shaders.SpriteShader")
	{
		_tintEnabled = base.Parameters["TintEnabled"];
	}

	protected TintSpriteEffect(string path)
		: base(path)
	{
	}
}
