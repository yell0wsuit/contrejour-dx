using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using Mokus2D.Collections;
using Mokus2D.Interfaces;
using Mokus2D.Util;

namespace Mokus2D.Input;

public class KeyboardController : IUpdatable
{
    private readonly HashSet<Keys> _pressedKeys = [];

    private readonly HashSet<Keys> _removedKeys = [];

    private readonly HashSet<Keys> _newKeys = [];

    private readonly Dictionary<Keys, List<Action<Keys, bool>>> _actions = [];

    private readonly FactoryDictionary<Keys, ForEachList<Action<Keys>>> _delayActions = new(k => []);

    private readonly List<Action<Keys, bool>> _currentActions = [];

    private readonly KeyboardDelayListener _delayListener;

    private readonly Flag _stopDelayEvent = new(on: false);

    private readonly HashSet<Keys> _currentPressedKeys = [];

    public bool IsShiftPressed
    {
        get
        {
            return !_currentPressedKeys.Contains(Keys.LeftShift) ? _currentPressedKeys.Contains(Keys.RightShift) : true;
        }
    }

    public bool IsCapital
    {
        get
        {
            return !IsShiftPressed ? Keyboard.GetState().IsKeyDown(Keys.CapsLock) : true;
        }
    }

    public event Action<Keys, bool> KeyStateChangedEvent;

    public event Action<Keys> KeyPressedEvent;

    private void Initialize()
    {
    }

    private void DispatchKeyboardEvents()
    {
        _newKeys.Clear();
        _removedKeys.Clear();
        foreach (Keys pressedKey in _pressedKeys)
        {
            _ = _removedKeys.Add(pressedKey);
        }
        Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
        Keys[] array = pressedKeys;
        foreach (Keys item in array)
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
        foreach (Keys removedKey in _removedKeys)
        {
            _ = _pressedKeys.Remove(removedKey);
        }
        foreach (Keys removedKey2 in _removedKeys)
        {
            DispatchKeyEvent(removedKey2, value: false);
        }
        foreach (Keys newKey in _newKeys)
        {
            DispatchKeyEvent(newKey, value: true);
        }
    }

    public void OnGameDeactivated()
    {
    }

    public void OnGameExit()
    {
    }

    public KeyboardController()
    {
        _delayListener = new KeyboardDelayListener(this);
        _delayListener.KeyPressedEvent += OnKeyPressedWithDelay;
        Initialize();
    }

    public void AddListener(Keys key, Action<Keys, bool> action)
    {
        List<Action<Keys, bool>> list = _actions.TryGetValue(key);
        if (list == null)
        {
            list = [];
            _actions.Add(key, list);
        }
        list.Add(action);
    }

    public bool IsPressed(Keys key)
    {
        return _currentPressedKeys.Contains(key);
    }

    public void RemoveAllListeners()
    {
        foreach (KeyValuePair<Keys, List<Action<Keys, bool>>> action in _actions)
        {
            action.Value.Clear();
        }
    }

    public void RemoveListeners(Keys key)
    {
        List<Action<Keys, bool>> list = _actions[key];
        list.Clear();
    }

    public void AddDelayListener(Keys key, Action<Keys> action, bool addFirst = false)
    {
        ForEachList<Action<Keys>> orCreate = _delayActions.GetOrCreate(key);
        if (addFirst)
        {
            orCreate.Insert(0, action);
        }
        else
        {
            orCreate.Add(action);
        }
    }

    public void RemoveDelayListener(Keys key, Action<Keys> action)
    {
        _ = (_delayActions.TryGetValue(key)?.SafeRemove(action));
    }

    public void RemoveListener(Keys key, Action<Keys, bool> action)
    {
        List<Action<Keys, bool>> list = _actions[key];
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

    private void OnKeyPressedWithDelay(Keys keys)
    {
        KeyPressedEvent.Dispatch(keys);
        if (_stopDelayEvent.Use())
        {
            return;
        }
        ForEachList<Action<Keys>> forEachList = _delayActions.TryGetValue(keys);
        if (forEachList == null)
        {
            return;
        }
        using (forEachList.Using())
        {
            foreach (Action<Keys> item in forEachList)
            {
                item(keys);
                if (_stopDelayEvent.Use())
                {
                    break;
                }
            }
        }
    }

    private void DispatchKeyEvent(Keys key, bool value)
    {
        if (value)
        {
            _ = _currentPressedKeys.Add(key);
        }
        else
        {
            _ = _currentPressedKeys.Remove(key);
        }
        List<Action<Keys, bool>> list = _actions.TryGetValue(key);
        if (list != null)
        {
            _currentActions.AddItemsNoGarbage(list);
            foreach (Action<Keys, bool> currentAction in _currentActions)
            {
                currentAction(key, value);
            }
            _currentActions.Clear();
        }
        KeyStateChangedEvent.Dispatch(key, value);
    }
}
