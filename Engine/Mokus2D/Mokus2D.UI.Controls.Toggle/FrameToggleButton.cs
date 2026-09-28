using Mokus2D.Effects.OnOff;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls.Toggle;

public class FrameToggleButton : ToggleButton
{
    public FrameToggleButton(MovieClip content)
        : base(content, new FrameOnOff(content))
    {
    }
}
