using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Mokus2D.Interfaces;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Data;

namespace Mokus2D.Input;

public class KeysController : IUpdatable
{
    private static readonly Comparison<ActionPriority> Comparison = (first, second) => Comparisons.FloatComparizon(first.Priority, second.Priority);

    private readonly SortedCollection<ActionPriority> _backKeysListeners = new(64, Comparison);

    private readonly List<ActionPriority> _toRemove = new(64);

    private bool _isBackPressed;

    private bool _inUpdate;

    private bool _stoped;

    private readonly bool Enabled = true;

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
        ButtonState back = GamePad.GetState(PlayerIndex.One).Buttons.Back;
        if (back == ButtonState.Pressed && !_isBackPressed)
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
        _isBackPressed = back == ButtonState.Pressed;
        _stoped = false;
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
}
