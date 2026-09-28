using Microsoft.Xna.Framework;
using Mokus2D.Util.Extensions;

namespace Mokus2D.Effects.Tween.ValueSetters;

public class Vector2Setter : ValueSetter<Vector2>
{
	protected override Vector2 Lerp(Vector2 from, Vector2 to, float amount)
	{
		return from.LerpTo(to, amount);
	}
}
