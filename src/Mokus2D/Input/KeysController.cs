using System;
using System.Collections.Generic;
using System.Linq;

using Mokus2D.Interfaces;
using Mokus2D.Util.Extensions;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;

namespace Mokus2D.Input
{
    public class KeysController : IUpdatable
    {
        private static readonly Comparison<ActionPriority> Comparison = (first, second) => Comparisons.FloatComparizon(first.Priority, second.Priority);

        private readonly SortedCollection<ActionPriority> _backKeysListeners = new(64, Comparison);

        private readonly List<ActionPriority> _toRemove = new(64);

        private readonly Dictionary<Key, List<Action>> _keyListeners = [];

        private readonly HashSet<Key> _heldKeys = [];

        private readonly List<Key> _polledKeys = [];

        private readonly List<Action> _pressedListeners = [];

        private bool _isBackPressed;

        private bool _inUpdate;

        private bool _stoped;

        private readonly bool Enabled = true;

        private readonly Func<IInputSource> _input;

        public KeysController()
            : this(() => Mokus2DGame.Input)
        {
        }

        internal KeysController(Func<IInputSource> input)
        {
            _input = input;
        }

        public void StopPropagation()
        {
            _stoped = true;
        }

        public void Update(float time)
        {
            if (!Enabled)
            {
                return;
            }
            _inUpdate = true;
            bool back = _input().IsBackPressed;
            if (back && !_isBackPressed)
            {
                foreach (ActionPriority backKeysListener in _backKeysListeners)
                {
                    backKeysListener.Action();
                    if (_stoped)
                    {
                        break;
                    }
                }
            }
            _isBackPressed = back;
            _stoped = false;
            DispatchKeyPresses();
            _inUpdate = false;
            _backKeysListeners.RemoveListNoGarbage(_toRemove);
            _toRemove.Clear();
        }

        public bool ContainsListener(Action action)
        {
            ActionPriority item = FindItem(action);
            return _backKeysListeners.Contains(item) && !_toRemove.Contains(item);
        }

        private ActionPriority FindItem(Action action)
        {
            return _backKeysListeners.FirstOrDefault(item => item.Action == action);
        }

        public void RemoveBackKeyListener(Action action)
        {
            if (_inUpdate)
            {
                _toRemove.Add(FindItem(action));
            }
            else
            {
                _ = _backKeysListeners.Remove(FindItem(action));
            }
        }

        public void AddBackKeyListener(Action action)
        {
            _backKeysListeners.Add(new ActionPriority(action, 0));
        }

        // Fires once when the key goes down; holding it or the OS key repeat does not fire again.
        public void AddKeyListener(Key key, Action action)
        {
            if (!_keyListeners.TryGetValue(key, out List<Action> listeners))
            {
                listeners = [];
                _keyListeners.Add(key, listeners);
            }
            listeners.Add(action);
        }

        public void RemoveKeyListener(Key key, Action action)
        {
            _ = _keyListeners.GetValueOrDefault(key)?.Remove(action);
        }

        private void DispatchKeyPresses()
        {
            _input().GetPressedKeys(_polledKeys);
            foreach (Key key in _polledKeys)
            {
                if (!_heldKeys.Contains(key) && _keyListeners.TryGetValue(key, out List<Action> listeners))
                {
                    _pressedListeners.AddRange(listeners);
                }
            }
            _heldKeys.Clear();
            _heldKeys.UnionWith(_polledKeys);
            // Listeners may add or remove listeners, so they run from a copy.
            foreach (Action listener in _pressedListeners)
            {
                listener();
            }
            _pressedListeners.Clear();
        }
    }
}
