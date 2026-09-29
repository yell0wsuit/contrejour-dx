using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mokus2D.Visual.ShaderSupport.Parameters
{
    public class ShaderParameterVector2 : ShaderParameter<Vector2>
    {
        public ShaderParameterVector2(EffectParameterCollection parameters, string name)
            : base(parameters, name)
        {
        }

        public ShaderParameterVector2(EffectParameter parameter)
            : base(parameter)
        {
        }

        protected override void SetValue(Vector2 value)
        {
            Parameter.SetValue(value);
        }

        protected override Vector2 GetValue()
        {
            return Parameter.GetValueVector2();
        }

        public static implicit operator ShaderParameterVector2(EffectParameter parameter)
        {
            return new ShaderParameterVector2(parameter);
        }
    }
}
