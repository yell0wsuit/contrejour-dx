using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.ShaderSupport.Parameters;

public class ShaderParameterVector3 : ShaderParameter<Vector3>
{
	public ShaderParameterVector3(EffectParameterCollection parameters, string name)
		: base(parameters, name)
	{
	}

	public ShaderParameterVector3(EffectParameter parameter)
		: base(parameter)
	{
	}

	protected override void SetValue(Vector3 value)
	{
		Parameter.SetValue(value);
	}

	protected override Vector3 GetValue()
	{
		return Parameter.GetValueVector3();
	}

	public static implicit operator ShaderParameterVector3(EffectParameter parameter)
	{
		return new ShaderParameterVector3(parameter);
	}
}
