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
	private static readonly Comparison<ActionPriority> Comparison = (ActionPriority first, ActionPriority second) => Comparisons.FloatComparizon(first.Priority, second.Priority);

	private readonly SortedList<ActionPriority> _backKeysListeners = new SortedList<ActionPriority>(64, Comparison);

	private readonly List<ActionPriority> _toRemove = new List<ActionPriority>(64);

	private bool _isBackPressed;

	private bool _inUpdate;

	private bool _stoped;

	public bool Enabled = true;

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
		if (_backKeysListeners.Contains(item))
		{
			return !_toRemove.Contains(item);
		}
		return false;
	}

	private ActionPriority FindItem(Action action)
	{
		return _backKeysListeners.FirstOrDefault((ActionPriority item) => item.Action == action);
	}

	public void RemoveBackKeyListener(Action action)
	{
		if (_inUpdate)
		{
			_toRemove.Add(FindItem(action));
		}
		else
		{
			_backKeysListeners.Remove(FindItem(action));
		}
	}

	public void AddBackKeyListener(Action action, int priority = 0)
	{
		_backKeysListeners.Add(new ActionPriority(action, 0));
	}
}
