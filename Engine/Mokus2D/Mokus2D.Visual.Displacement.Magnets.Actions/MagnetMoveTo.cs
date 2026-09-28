using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetMoveTo : MagnetIntervalActionBase
{
    private readonly Vector2 _targetPosition;

    private readonly Vector2 _startPosition;

    public MagnetMoveTo(GridMagnetBase gridMagnet, float timeout, Vector2 targetPosition)
        : base(gridMagnet, timeout)
    {
        _startPosition = gridMagnet.Position;
        _targetPosition = targetPosition;
    }

    protected override void UpdateMagnet(float ratio)
    {
        GridMagnet.Position = Vector2.Lerp(_startPosition, _targetPosition, ratio);
    }
}
