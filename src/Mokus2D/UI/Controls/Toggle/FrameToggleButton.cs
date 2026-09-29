using Mokus2D.Effects.OnOff;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls.Toggle
{
    public class FrameToggleButton(MovieClip content) : ToggleButton(content, new FrameOnOff(content))
    {
    }
}
