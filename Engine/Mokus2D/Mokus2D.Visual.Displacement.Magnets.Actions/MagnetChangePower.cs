using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetChangePower : MagnetIntervalActionBase
{
    private float _targetPower;

    private float _startPower;

    public float StartPower
    {
        get => _startPower;
        set => _startPower = value;
    }

    public MagnetChangePower(GridMagnetBase gridMagnet, float timeout, float targetPower)
        : base(gridMagnet, timeout)
    {
        _startPower = gridMagnet.Power;
        _targetPower = targetPower;
    }

    protected override void UpdateMagnet(float ratio)
    {
        GridMagnet.Power = ratio.Lerp(_startPower, _targetPower);
    }
}
