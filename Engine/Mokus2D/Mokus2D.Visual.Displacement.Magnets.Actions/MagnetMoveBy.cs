using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetMoveBy : MagnetMoveTo
{
	public MagnetMoveBy(GridMagnetBase gridMagnet, float timeout, Vector2 offset)
		: base(gridMagnet, timeout, gridMagnet.Position + offset)
	{
	}
}
