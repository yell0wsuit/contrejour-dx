using System.Collections.Generic;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetSequence(params MagnetAction[] actions) : MagnetAction(null)
{
    private readonly Queue<MagnetAction> _actions = new Queue<MagnetAction>(actions);

    public override bool Finished => _actions.Count == 0;

    public override void Update(float time)
    {
        base.Update(time);
        MagnetAction magnetAction = _actions.Peek();
        magnetAction.Update(time);
        if (magnetAction.Finished)
        {
            _ = _actions.Dequeue();
        }
    }
}
