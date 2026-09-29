using System;
using System.Collections.Generic;

using Mokus2D.Collections;
using Mokus2D.Interfaces;
using Mokus2D.Util;

namespace Mokus2D.Input
{
    public class KeyboardController : IUpdatable
    {
        private readonly HashSet<Key> _pressedKeys = [];

        private readonly HashSet<Key> _removedKeys = [];

        private readonly HashSet<Key> _newKeys = [];

        private readonly Dictionary<Key, List<Action<Key, bool>>> _actions = [];

        private readonly FactoryDictionary<Key, ForEachCollection<Action<Key>>> _delayActions = new(k => []);

        private readonly List<Action<Key, bool>> _currentActions = [];

        private readonly KeyboardDelayListener _delayListener;

        private readonly Flag _stopDelayEvent = new(on: false);

        private readonly HashSet<Key> _currentPressedKeys = [];

        private readonly List<Key> _polledKeys = [];

        public bool IsShiftPressed => _currentPressedKeys.Contains(Key.LeftShift) || _currentPressedKeys.Contains(Key.RightShift);

        public bool IsCapital => IsShiftPressed || Mokus2DGame.Input.IsKeyDown(Key.CapsLock);

        public event Action<Key, bool> KeyStateChangedEvent;

        public event Action<Key> KeyPressedEvent;

        private static void Initialize()
        {
        }

        private void DispatchKeyboardEvents()
        {
            _newKeys.Clear();
            _removedKeys.Clear();
            foreach (Key pressedKey in _pressedKeys)
            {
                _ = _removedKeys.Add(pressedKey);
            }
            Mokus2DGame.Input.GetPressedKeys(_polledKeys);
            foreach (Key item in _polledKeys)
            {
                if (!_pressedKeys.Contains(item))
                {
                    _ = _pressedKeys.Add(item);
                    _ = _newKeys.Add(item);
                }
                else
                {
                    _ = _removedKeys.Remove(item);
                }
            }
            foreach (Key removedKey in _removedKeys)
            {
                _ = _pressedKeys.Remove(removedKey);
            }
            foreach (Key removedKey2 in _removedKeys)
            {
                DispatchKeyEvent(removedKey2, value: false);
            }
            foreach (Key newKey in _newKeys)
            {
                DispatchKeyEvent(newKey, value: true);
            }
        }

        public static void OnGameDeactivated()
        {
        }

        public static void OnGameExit()
        {
        }

        public KeyboardController()
        {
            _delayListener = new KeyboardDelayListener(this);
            _delayListener.KeyPressedEvent += OnKeyPressedWithDelay;
            Initialize();
        }

        public void AddListener(Key key, Action<Key, bool> action)
        {
            List<Action<Key, bool>> list = _actions.GetValueOrDefault(key);
            if (list == null)
            {
                list = [];
                _actions.Add(key, list);
            }
            list.Add(action);
        }

        public bool IsPressed(Key key)
        {
            return _currentPressedKeys.Contains(key);
        }

        public void RemoveAllListeners()
        {
            foreach (KeyValuePair<Key, List<Action<Key, bool>>> action in _actions)
            {
                action.Value.Clear();
            }
        }

        public void RemoveListeners(Key key)
        {
            List<Action<Key, bool>> list = _actions[key];
            list.Clear();
        }

        public void AddDelayListener(Key key, Action<Key> action, bool addFirst = false)
        {
            ForEachCollection<Action<Key>> orCreate = _delayActions.GetOrCreate(key);
            if (addFirst)
            {
                orCreate.Insert(0, action);
            }
            else
            {
                orCreate.Add(action);
            }
        }

        public void RemoveDelayListener(Key key, Action<Key> action)
        {
            _ = _delayActions.GetValueOrDefault(key)?.Remove(action);
        }

        public void RemoveListener(Key key, Action<Key, bool> action)
        {
            List<Action<Key, bool>> list = _actions[key];
            _ = list.Remove(action);
        }

        public void Update(float time)
        {
            DispatchKeyboardEvents();
            _delayListener.Update(time);
        }

        public void StopDelayEventPropagation()
        {
            _stopDelayEvent.SetOn();
        }

        private void OnKeyPressedWithDelay(Key keys)
        {
            KeyPressedEvent.Dispatch(keys);
            if (_stopDelayEvent.Use())
            {
                return;
            }
            ForEachCollection<Action<Key>> forEachList = _delayActions.GetValueOrDefault(keys);
            if (forEachList == null)
            {
                return;
            }
            using (forEachList.Using())
            {
                foreach (Action<Key> item in forEachList)
                {
                    item(keys);
                    if (_stopDelayEvent.Use())
                    {
                        break;
                    }
                }
            }
        }

        private void DispatchKeyEvent(Key key, bool value)
        {
            _ = value ? _currentPressedKeys.Add(key) : _currentPressedKeys.Remove(key);
            List<Action<Key, bool>> list = _actions.GetValueOrDefault(key);
            if (list != null)
            {
                _currentActions.AddRange(list);
                foreach (Action<Key, bool> currentAction in _currentActions)
                {
                    currentAction(key, value);
                }
                _currentActions.Clear();
            }
            KeyStateChangedEvent.Dispatch(key, value);
        }
    }
}
