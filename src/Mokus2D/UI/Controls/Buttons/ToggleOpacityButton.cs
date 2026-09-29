using Mokus2D.Effects.OnOff;
using Mokus2D.UI.Controls.Toggle;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls.Buttons
{
    public class ToggleOpacityButton : ToggleButton
    {
        public ToggleOpacityButton(Node effectTarget, Sprite clickable, float onOpacity, float offOpacity, float duration)
            : base(clickable, new FadeEffect(effectTarget, onOpacity, offOpacity, duration))
        {
            effectTarget.OpacityFloat = offOpacity;
        }

        public ToggleOpacityButton(Node effectTarget, Sprite clickable, float offOpacity, float duration)
            : this(effectTarget, clickable, 1f, offOpacity, duration)
        {
        }
    }
}
