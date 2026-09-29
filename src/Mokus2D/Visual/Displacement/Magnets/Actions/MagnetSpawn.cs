using System.Collections.Generic;

using Mokus2D.Util.Extensions;

namespace Mokus2D.Visual.Displacement.Magnets.Actions
{
    public class MagnetSpawn(params MagnetAction[] actions) : MagnetAction(null)
    {
        private readonly List<MagnetAction> _actions = [.. actions];

        private readonly List<MagnetAction> _toRemove = [];

        public override bool Finished => _actions.Empty();

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
}
