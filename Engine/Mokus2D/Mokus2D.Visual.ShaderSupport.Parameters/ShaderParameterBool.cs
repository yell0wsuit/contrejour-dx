using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.ShaderSupport.Parameters;

public class ShaderParameterBool : ShaderParameter<bool>
{
    public ShaderParameterBool(EffectParameterCollection parameters, string name)
        : base(parameters, name)
    {
    }

    public ShaderParameterBool(EffectParameter parameter)
        : base(parameter)
    {
    }

    protected override void SetValue(bool value)
    {
        Parameter.SetValue(value);
    }

    protected override bool GetValue()
    {
        return Parameter.GetValueBoolean();
    }

    public static implicit operator ShaderParameterBool(EffectParameter parameter)
    {
        return new ShaderParameterBool(parameter);
    }
}
