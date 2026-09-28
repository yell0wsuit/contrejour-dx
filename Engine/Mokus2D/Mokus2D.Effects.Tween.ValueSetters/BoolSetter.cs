namespace Mokus2D.Effects.Tween.ValueSetters;

public class BoolSetter : ValueSetter<bool>
{
    protected override bool Lerp(bool from, bool to, float amount)
    {
        return !(amount >= 0.5f) ? from : to;
    }
}
