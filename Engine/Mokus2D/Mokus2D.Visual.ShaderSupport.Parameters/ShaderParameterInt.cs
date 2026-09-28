using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.ShaderSupport.Parameters;

public class ShaderParameterInt : ShaderParameter<int>
{
	public ShaderParameterInt(EffectParameterCollection parameters, string name)
		: base(parameters, name)
	{
	}

	public ShaderParameterInt(EffectParameter parameter)
		: base(parameter)
	{
	}

	protected override void SetValue(int value)
	{
		Parameter.SetValue(value);
	}

	protected override int GetValue()
	{
		return Parameter.GetValueInt32();
	}

	public static implicit operator ShaderParameterInt(EffectParameter parameter)
	{
		return new ShaderParameterInt(parameter);
	}
}
