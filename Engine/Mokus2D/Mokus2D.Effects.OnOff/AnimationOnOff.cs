using Mokus2D.Visual;
using Mokus2D.Visual.Animation;

namespace Mokus2D.Effects.OnOff;

public class AnimationOnOff : ActionOnOff
{
	private readonly IAnimatedNode _animation;

	public AnimationOnOff(IAnimatedNode animation)
		: base(null)
	{
		_animation = animation;
		SetAction(OnAction);
		_animation.Stoped = true;
		_animation.Repeat = false;
	}

	public override void SetOn(bool value)
	{
		base.SetOn(value);
		_animation.CurrentFrame = (value ? _animation.LastFrame() : 0);
	}

	private void OnAction(bool value)
	{
		Play(!value);
	}

	private void Play(bool rewind)
	{
		_animation.Rewind = rewind;
		_animation.Stoped = false;
	}
}
