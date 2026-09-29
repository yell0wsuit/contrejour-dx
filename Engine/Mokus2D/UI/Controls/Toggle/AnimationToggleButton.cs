using Mokus2D.Effects.OnOff;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls.Toggle;

public class AnimationToggleButton(IAnimatedNode content, Sprite background) : ToggleButton(background, new AnimationOnOff(content))
{
}
