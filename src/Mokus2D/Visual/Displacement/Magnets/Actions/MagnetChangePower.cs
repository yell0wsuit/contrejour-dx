using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets.Actions
{
    public class MagnetChangePower(GridMagnetBase gridMagnet, float timeout, float targetPower) : MagnetIntervalActionBase(gridMagnet, timeout)
    {
        private readonly float _targetPower = targetPower;

        public float StartPower { get; set; } = gridMagnet.Power;

        protected override void UpdateMagnet(float ratio)
        {
            GridMagnet.Power = ratio.Lerp(StartPower, _targetPower);
        }
    }
}
