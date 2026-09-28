namespace Mokus2D.Effects.Tween.ValueSetters;

public class BoolSetter : ValueSetter<bool>
{
    protected override bool Lerp(bool value1, bool value2, float amount)
    {
        return !(amount >= 0.5f) ? value1 : value2;
    }
}
