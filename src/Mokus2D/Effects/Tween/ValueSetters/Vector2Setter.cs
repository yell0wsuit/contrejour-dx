using Microsoft.Xna.Framework;

using Mokus2D.Util.MathUtils;

namespace Mokus2D.Effects.Tween.ValueSetters
{
    public class Vector2Setter : ValueSetter<Vector2>
    {
        protected override Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return XnaMath.Lerp(value1, value2, amount);
        }
    }
}
