using Microsoft.Xna.Framework;

namespace Mokus2D.Effects.Tween.ValueSetters;

public class Vector3Setter : ValueSetter<Vector3>
{
	protected override Vector3 Lerp(Vector3 from, Vector3 to, float amount)
	{
		return Vector3.Lerp(from, to, amount);
	}
}
