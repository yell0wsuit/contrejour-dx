using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.ShaderSupport.Parameters;

public class ShaderParameterColor : ShaderParameter<Color>
{
    public ShaderParameterColor(EffectParameterCollection parameters, string name)
        : base(parameters, name)
    {
    }

    public ShaderParameterColor(EffectParameter parameter)
        : base(parameter)
    {
    }

    protected override void SetValue(Color value)
    {
        Parameter.SetValue(value.ToVector4());
    }

    protected override Color GetValue()
    {
        return Parameter.GetValueVector4().ToColor();
    }

    public static implicit operator ShaderParameterColor(EffectParameter parameter)
    {
        return new ShaderParameterColor(parameter);
    }
}
