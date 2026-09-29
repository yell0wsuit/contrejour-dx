using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.ShaderSupport.Parameters
{
    public class ShaderParameterFloat : ShaderParameter<float>
    {
        public ShaderParameterFloat(EffectParameterCollection parameters, string name)
            : base(parameters, name)
        {
        }

        public ShaderParameterFloat(EffectParameter parameter)
            : base(parameter)
        {
        }

        protected override void SetValue(float value)
        {
            Parameter.SetValue(value);
        }

        protected override float GetValue()
        {
            return Parameter.GetValueSingle();
        }

        public static implicit operator ShaderParameterFloat(EffectParameter parameter)
        {
            return new ShaderParameterFloat(parameter);
        }
    }
}
