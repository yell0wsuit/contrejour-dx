namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetDelay : MagnetIntervalActionBase
{
	public MagnetDelay(GridMagnetBase gridMagnet, float timeout)
		: base(gridMagnet, timeout)
	{
	}

	protected override void UpdateMagnet(float ratio)
	{
	}
}
