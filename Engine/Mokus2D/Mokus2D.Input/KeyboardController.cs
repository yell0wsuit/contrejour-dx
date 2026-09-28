using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

using Mokus2D.Collections;
using Mokus2D.Interfaces;
using Mokus2D.Util;

namespace Mokus2D.Input;

public class KeyboardController : IUpdatable
{
    private readonly HashSet<Keys> _pressedKeys = new HashSet<Keys>();

    private readonly HashSet<Keys> _removedKeys = new HashSet<Keys>();

    private readonly HashSet<Keys> _newKeys = new HashSet<Keys>();

    private readonly Dictionary<Keys, List<Action<Keys, bool>>> _actions = new Dictionary<Keys, List<Action<Keys, bool>>>();

    private readonly FactoryDictionary<Keys, ForEachList<Action<Keys>>> _delayActions = new FactoryDictionary<Keys, ForEachList<Action<Keys>>>((Keys k) => new ForEachList<Action<Keys>>());

    private readonly List<Action<Keys, bool>> _currentActions = new List<Action<Keys, bool>>();

    private readonly KeyboardDelayListener _delayListener;

    private readonly Flag _stopDelayEvent = new Flag(on: false);

    private readonly HashSet<Keys> _currentPressedKeys = new HashSet<Keys>();

    public bool IsShiftPressed
    {
        get
        {
            if (!_currentPressedKeys.Contains(Keys.LeftShift))
            {
                return _currentPressedKeys.Contains(Keys.RightShift);
            }
            return true;
        }
    }

    public bool IsCapital
    {
        get
        {
            if (!IsShiftPressed)
            {
                return Keyboard.GetState().IsKeyDown(Keys.CapsLock);
            }
            return true;
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
            _removedKeys.Add(pressedKey);
        }
        Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
        Keys[] array = pressedKeys;
        foreach (Keys item in array)
        {
            if (!_pressedKeys.Contains(item))
            {
                _pressedKeys.Add(item);
                _newKeys.Add(item);
            }
            else
            {
                _removedKeys.Remove(item);
            }
        }
        foreach (Keys removedKey in _removedKeys)
        {
            _pressedKeys.Remove(removedKey);
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
            list = new List<Action<Keys, bool>>();
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
        _delayActions.TryGetValue(key)?.SafeRemove(action);
    }

    public void RemoveListener(Keys key, Action<Keys, bool> action)
    {
        List<Action<Keys, bool>> list = _actions[key];
        list.Remove(action);
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
        this.KeyPressedEvent.Dispatch(keys);
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
            _currentPressedKeys.Add(key);
        }
        else
        {
            _currentPressedKeys.Remove(key);
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
        this.KeyStateChangedEvent.Dispatch(key, value);
    }
}
