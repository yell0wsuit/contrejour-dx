using System;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetIntervalAction(GridMagnetBase gridMagnet, float timeout, Action<float> action) : MagnetIntervalActionBase(gridMagnet, timeout)
{
    private readonly Action<float> _action = action;

    protected override void UpdateMagnet(float ratio)
    {
        _action(ratio);
    }
}
