using Microsoft.Xna.Framework;
using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff;

public class ColorEffect : OnOffTweenEffect<Color>
{
	public ColorEffect(Node target, float duration, Color onColor, Color offColor)
		: base(target, duration, NodeValues.Color, onColor, offColor)
	{
	}

	public ColorEffect(Node target, float duration, Color onColor)
		: this(target, duration, onColor, Color.White)
	{
	}

	protected override void SetOn()
	{
		base.SetOn();
	}

	protected override void SetOff()
	{
		base.SetOff();
	}

	public override void SetOn(bool value)
	{
		base.SetOn(value);
	}
}
