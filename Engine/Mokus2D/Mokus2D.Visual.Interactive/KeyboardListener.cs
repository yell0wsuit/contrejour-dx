using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Mokus2D.Collections;
using Mokus2D.Util.Resources;

namespace Mokus2D.Visual.Interactive;

public class KeyboardListener : DisposableBase
{
	private bool _enabled = true;

	private readonly FactoryDictionary<Keys, List<Action<Keys, bool>>> _keysList = new FactoryDictionary<Keys, List<Action<Keys, bool>>>((Keys k) => new List<Action<Keys, bool>>());

	public bool Enabled
	{
		get
		{
			return _enabled;
		}
		set
		{
			if (!_enabled)
			{
				_enabled = value;
				if (_enabled)
				{
					AddListeners();
				}
				else
				{
					RemoveListeners();
				}
			}
		}
	}

	private void AddListeners()
	{
		foreach (KeyValuePair<Keys, List<Action<Keys, bool>>> keys in _keysList)
		{
			foreach (Action<Keys, bool> item in keys.Value)
			{
				Mokus2DGame.Keyboard.AddListener(keys.Key, item);
			}
		}
	}

	private void RemoveListeners()
	{
		foreach (KeyValuePair<Keys, List<Action<Keys, bool>>> keys in _keysList)
		{
			foreach (Action<Keys, bool> item in keys.Value)
			{
				Mokus2DGame.Keyboard.RemoveListener(keys.Key, item);
			}
		}
	}

	public void AddListener(Keys key, Action<Keys, bool> listener)
	{
		_keysList.GetOrCreate(key).Add(listener);
		if (Enabled)
		{
			Mokus2DGame.Keyboard.AddListener(key, listener);
		}
	}

	public void RemoveListener(Keys key, Action<Keys, bool> listener)
	{
		_keysList[key].Remove(listener);
		if (Enabled)
		{
			Mokus2DGame.Keyboard.RemoveListener(key, listener);
		}
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		Enabled = false;
	}
}
