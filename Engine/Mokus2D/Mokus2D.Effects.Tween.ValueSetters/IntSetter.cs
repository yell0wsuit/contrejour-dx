using Mokus2D.Util.MathUtils;

namespace Mokus2D.Effects.Tween.ValueSetters;

public class IntSetter : ValueSetter<int>
{
    protected override int Lerp(int value1, int value2, float amount)
    {
        return (int)amount.Lerp(value1, value2);
    }
}
