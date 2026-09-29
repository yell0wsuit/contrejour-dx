namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetDelay(GridMagnetBase gridMagnet, float timeout) : MagnetIntervalActionBase(gridMagnet, timeout)
{
    protected override void UpdateMagnet(float ratio)
    {
    }
}
