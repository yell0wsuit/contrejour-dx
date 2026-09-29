using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets.Actions
{
    public class MagnetMoveBy(GridMagnetBase gridMagnet, float timeout, Vector2 offset) : MagnetMoveTo(gridMagnet, timeout, gridMagnet.Position + offset)
    {
    }
}
