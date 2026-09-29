using Microsoft.Xna.Framework;

namespace Mokus2D.Effects.Tween.ValueSetters
{
    public class ColorSetter : ValueSetter<Color>
    {
        protected override Color Lerp(Color value1, Color value2, float amount)
        {
            return Color.Lerp(value1, value2, amount);
        }
    }
}
