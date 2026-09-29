using Mokus2D.Util.MathUtils;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetChangeRadius : MagnetIntervalActionBase
{
    private readonly float _targetRadius;

    private readonly float _startRadius;

    private readonly CircleMagnet CircleMagnet;

    public MagnetChangeRadius(CircleMagnet gridMagnet, float timeout, float targetRadius)
        : base(gridMagnet, timeout)
    {
        CircleMagnet = gridMagnet;
        _startRadius = CircleMagnet.Radius;
        _targetRadius = targetRadius;
    }

    protected override void UpdateMagnet(float ratio)
    {
        CircleMagnet.Radius = ratio.Lerp(_startRadius, _targetRadius);
    }
}
