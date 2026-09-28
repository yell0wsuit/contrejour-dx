using System;
using System.Collections.Generic;
using Mokus2D.Collections;
using Mokus2D.Input;
using Mokus2D.UI.Controls.Buttons;
using Mokus2D.Util;

namespace Mokus2D.UI.Controls.Toggle;

public class ToggleButtonGroup<T>
{
	private readonly List<ToggleButton> _buttons = new List<ToggleButton>();

	private ToggleButton _selectedButton;

	private readonly BiDictionary<ToggleButton, T> _data = new BiDictionary<ToggleButton, T>();

	public bool DeselectEnabled = true;

	public bool ToggleOnTouchBegin = true;

	private ToggleButton _selectingButton;

	public ToggleButton this[int index] => _buttons[index];

	public int Count => _buttons.Count;

	public T SelectedValue
	{
		get
		{
			return _data[_selectedButton];
		}
		set
		{
			SelectedButton = _data.GetKey(value);
		}
	}

	public int SelectedIndex
	{
		get
		{
			return _buttons.IndexOf(_selectedButton);
		}
		set
		{
			SelectedButton = _buttons[value];
		}
	}

	public ToggleButton SelectedButton
	{
		get
		{
			return _selectedButton;
		}
		set
		{
			_selectingButton = null;
			if (_selectedButton == value)
			{
				return;
			}
			_selectedButton = value;
			foreach (ToggleButton button in _buttons)
			{
				button.Toggle = button == _selectedButton;
				if (!button.Toggle)
				{
					button.Clear();
				}
			}
		}
	}

	public ToggleButton SelectingButton => _selectingButton;

	public event Action<ToggleButton, T> SelectedButtonChangeEvent;

	public void Add(ToggleButton button, T data)
	{
		Add(button);
		_data.Add(button, data);
	}

	public void SelectFirstButtonIfExists()
	{
		if (Count > 0)
		{
			SelectedButton = this[0];
		}
	}

	public void Add(ToggleButton button)
	{
		button.TouchBeginEvent += OnButtonTouchBegin;
		button.ToggleChangeEvent += OnButtonToggleChangeEvent;
		button.ToggleOnTouchBegin = ToggleOnTouchBegin;
		_buttons.Add(button);
	}

	public T GetValue(ToggleButton button)
	{
		return _data[button];
	}

	public T GetValue(int index)
	{
		return GetValue(this[index]);
	}

	private void OnButtonToggleChangeEvent(ToggleButton toggleButton)
	{
		if (toggleButton != SelectedButton)
		{
			SelectedButton = toggleButton;
			toggleButton.Toggle = true;
			T argument = _data.TryGetValue(toggleButton);
			this.SelectedButtonChangeEvent.Dispatch(SelectedButton, argument);
		}
		else if (DeselectEnabled)
		{
			_selectedButton = null;
			this.SelectedButtonChangeEvent.Dispatch(null, default(T));
		}
		else
		{
			toggleButton.Toggle = true;
		}
	}

	public void RemoveButton(ToggleButton button)
	{
		DoRemoveButton(button);
		if (_data.ContainsKey(button))
		{
			_data.Remove(button);
		}
	}

	public void RemoveValue(T value)
	{
		DoRemoveButton(_data[value]);
		_data.Remove(value);
	}

	private void DoRemoveButton(ToggleButton button)
	{
		button.ToggleChangeEvent -= OnButtonToggleChangeEvent;
		button.TouchBeginEvent -= OnButtonTouchBegin;
		_buttons.Remove(button);
		if (_selectedButton == button)
		{
			_selectedButton = null;
			if (!DeselectEnabled && !_buttons.Empty())
			{
				SelectedButton = _buttons.First();
			}
		}
	}

	private void OnButtonTouchBegin(Button button, Touch arg2)
	{
		_selectingButton = (ToggleButton)button;
	}
}
public class ToggleButtonGroup : ToggleButtonGroup<object>
{
}
