using Mokus2D.Util.MathUtils;

namespace Mokus2D.Effects.Tween.ValueSetters;

public class FloatSetter : ValueSetter<float>
{
	protected override float Lerp(float from, float to, float amount)
	{
		return amount.Lerp(from, to);
	}
}
