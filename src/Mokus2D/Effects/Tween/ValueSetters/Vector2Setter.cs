using Microsoft.Xna.Framework;

namespace Mokus2D.Effects.Tween.ValueSetters
{
    public class Vector2Setter : ValueSetter<Vector2>
    {
        protected override Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return Vector2.Lerp(value1, value2, amount);
        }
    }
}
