using Mokus2D.Util.MathUtils;

namespace Mokus2D.Effects.Tween.ValueSetters
{
    public class FloatSetter : ValueSetter<float>
    {
        protected override float Lerp(float value1, float value2, float amount)
        {
            return amount.Lerp(value1, value2);
        }
    }
}
