using Microsoft.Xna.Framework;

namespace Mokus2D.Effects.Tween.ValueSetters;

public class ColorSetter : ValueSetter<Color>
{
    protected override Color Lerp(Color from, Color to, float amount)
    {
        return Color.Lerp(from, to, amount);
    }
}
