using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.Shaders;

public abstract class TextureMatrixEffectBase : SpriteBatchEffectBase
{
	private readonly EffectParameter _texture;

	private readonly EffectParameter _matrix;

	protected TextureMatrixEffectBase(string path)
		: base(path)
	{
		_texture = base.Parameters["Texture"];
		_matrix = base.Parameters["Matrix"];
	}

	public override void Apply(Matrix matrix, Texture2D texture)
	{
		_texture.SetValue(texture);
		_matrix.SetValue(matrix);
		ApplyPasses();
	}

	protected virtual void ApplyPasses()
	{
		Effect.CurrentTechnique.Passes[0].Apply();
	}
}
