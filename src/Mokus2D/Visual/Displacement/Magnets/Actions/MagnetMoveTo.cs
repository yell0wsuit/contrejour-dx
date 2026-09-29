using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets.Actions
{
    public class MagnetMoveTo(GridMagnetBase gridMagnet, float timeout, Vector2 targetPosition) : MagnetIntervalActionBase(gridMagnet, timeout)
    {
        private readonly Vector2 _targetPosition = targetPosition;

        private readonly Vector2 _startPosition = gridMagnet.Position;

        protected override void UpdateMagnet(float ratio)
        {
            GridMagnet.Position = Vector2.Lerp(_startPosition, _targetPosition, ratio);
        }
    }
}
