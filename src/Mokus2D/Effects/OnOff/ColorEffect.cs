using Mokus2D.Graphics;
using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff
{
    public class ColorEffect(Node target, float duration, Color onColor, Color offColor) : OnOffTweenEffect<Color>(target, duration, NodeValues.Color, onColor, offColor)
    {
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
}
