namespace Mokus2D.Effects.Tween.ValueSetters;

public class BoolSetter : ValueSetter<bool>
{
    protected override bool Lerp(bool from, bool to, float amount)
    {
        if (!(amount >= 0.5f))
        {
            return from;
        }
        return to;
    }
}
