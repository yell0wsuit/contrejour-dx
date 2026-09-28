using Mokus2D.Effects.OnOff;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls.Toggle;

public class AnimationToggleButton : ToggleButton
{
	public AnimationToggleButton(IAnimatedNode content, Sprite background)
		: base(background, new AnimationOnOff(content))
	{
	}
}
