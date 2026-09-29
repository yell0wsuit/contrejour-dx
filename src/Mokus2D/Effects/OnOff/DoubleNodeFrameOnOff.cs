using Mokus2D.Visual;

namespace Mokus2D.Effects.OnOff
{
    public class DoubleNodeFrameOnOff : FrameOnOff
    {
        private readonly IAnimatedNode _targetB;

        public DoubleNodeFrameOnOff(IAnimatedNode targetA, IAnimatedNode targetB, float offFrame = 0f, float onFrame = 1f)
            : base(targetA, offFrame, onFrame)
        {
            _targetB = targetB;
            SetOff();
        }

        protected override void SetOn()
        {
            base.SetOn();
            _targetB.GotoAndStop(OnFrame);
        }

        protected override void SetOff()
        {
            base.SetOff();
            _targetB?.GotoAndStop(OffFrame);
        }
    }
}
