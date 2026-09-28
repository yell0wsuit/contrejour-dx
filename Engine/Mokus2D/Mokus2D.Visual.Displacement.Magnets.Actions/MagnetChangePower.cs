using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetChangePower : MagnetIntervalActionBase
{
    private readonly float _targetPower;

    public float StartPower { get; set; }

    public MagnetChangePower(GridMagnetBase gridMagnet, float timeout, float targetPower)
        : base(gridMagnet, timeout)
    {
        StartPower = gridMagnet.Power;
        _targetPower = targetPower;
    }

    protected override void UpdateMagnet(float ratio)
    {
        GridMagnet.Power = ratio.Lerp(StartPower, _targetPower);
    }
}
