using System;
using System.Collections.Generic;

using Mokus2D.Collections;
using Mokus2D.Input;
using Mokus2D.UI.Controls.Buttons;
using Mokus2D.Util;

namespace Mokus2D.UI.Controls.Toggle
{
    public class ToggleButtonGroup<T>
    {
        private readonly List<ToggleButton> _buttons = [];

        private ToggleButton _selectedButton;

        private readonly BiDictionary<ToggleButton, T> _data = [];

        private readonly bool DeselectEnabled = true;

        private readonly bool ToggleOnTouchBegin = true;

        public ToggleButton this[int index] => _buttons[index];

        public int Count => _buttons.Count;

        public T SelectedValue
        {
            get => _data[_selectedButton];
            set => SelectedButton = _data.GetKey(value);
        }

        public int SelectedIndex
        {
            get => _buttons.IndexOf(_selectedButton);
            set => SelectedButton = _buttons[value];
        }

        public ToggleButton SelectedButton
        {
            get => _selectedButton;
            set
            {
                SelectingButton = null;
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

        public ToggleButton SelectingButton { get; private set; }

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
                _ = _data.TryGetValue(toggleButton, out T argument);
                SelectedButtonChangeEvent.Dispatch(SelectedButton, argument);
            }
            else if (DeselectEnabled)
            {
                _selectedButton = null;
                SelectedButtonChangeEvent.Dispatch(null, default);
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
                _ = _data.Remove(button);
            }
        }

        public void RemoveValue(T value)
        {
            DoRemoveButton(_data[value]);
            _ = _data.Remove(value);
        }

        private void DoRemoveButton(ToggleButton button)
        {
            button.ToggleChangeEvent -= OnButtonToggleChangeEvent;
            button.TouchBeginEvent -= OnButtonTouchBegin;
            _ = _buttons.Remove(button);
            if (_selectedButton == button)
            {
                _selectedButton = null;
                if (!DeselectEnabled && _buttons.Count != 0)
                {
                    SelectedButton = _buttons[0];
                }
            }
        }

        private void OnButtonTouchBegin(Button button, Touch arg2)
        {
            SelectingButton = (ToggleButton)button;
        }
    }
    public class ToggleButtonGroup : ToggleButtonGroup<object>
    {
    }
}
