using Mokus2D.Util.MathUtils;

namespace Mokus2D.Effects.Tween.ValueSetters;

public class IntSetter : ValueSetter<int>
{
	protected override int Lerp(int from, int to, float amount)
	{
		return (int)amount.Lerp(from, to);
	}
}
