using System.Collections.Generic;

namespace Mokus2D.Visual.Displacement.Magnets.Actions;

public class MagnetSpawn : MagnetAction
{
    private readonly List<MagnetAction> _actions = new List<MagnetAction>();

    private readonly List<MagnetAction> _toRemove = new List<MagnetAction>();

    public override bool Finished => _actions.Empty();

    public MagnetSpawn(params MagnetAction[] actions)
        : base(null)
    {
        _actions = new List<MagnetAction>(actions);
    }

    public override void Update(float time)
    {
        base.Update(time);
        foreach (MagnetAction action in _actions)
        {
            action.Update(time);
            if (action.Finished)
            {
                _toRemove.Add(action);
            }
        }
        _actions.RemoveListNoGarbage(_toRemove);
        _toRemove.Clear();
    }
}
