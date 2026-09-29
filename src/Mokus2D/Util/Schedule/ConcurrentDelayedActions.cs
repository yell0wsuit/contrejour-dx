using System;
using System.Collections.Concurrent;

using Mokus2D.Util.Resources;

namespace Mokus2D.Util.Schedule
{
    public class ConcurrentDelayedActions : DisposableBase
    {
        private readonly ConcurrentQueue<Action> _actions = new();

        public void Add(Action action)
        {
            _actions.Enqueue(action);
        }

        public void Execute()
        {
            do
            {
                if (_actions.TryDequeue(out Action result))
                {
                    result();
                }
            }
            while (!_actions.IsEmpty);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            while (!_actions.IsEmpty)
            {
                _ = _actions.TryDequeue(out _);
            }
        }
    }
}
