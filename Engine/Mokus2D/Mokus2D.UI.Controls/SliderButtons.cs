using System;

using Mokus2D.Interfaces;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls;

public class SliderButtons(Slider slider) : IUpdatable
{
    private readonly Slider _slider = slider;

    private Sprite _upButton;

    private Sprite _downButton;

    private bool _initialized;

    private int _scrollDirection;

    public float ScrollSpeed = 1f;

    public bool IsStaticScrollSpeed;

    public void Initialize(Sprite upButton, Sprite downButton)
    {
        if (_initialized)
        {
            throw new InvalidOperationException("SliderButtons is already initialized");
        }
        _upButton = upButton;
        _downButton = downButton;
        InitializeButton(_upButton, -1);
        InitializeButton(_downButton, 1);
        _initialized = true;
    }

    private void InitializeButton(Sprite button, int direction)
    {
        if (button != null)
        {
            button.Clickable = true;
            button.TouchBeginEvent += delegate
            {
                _scrollDirection = direction;
            };
            button.TouchEndEvent += delegate
            {
                _scrollDirection = 0;
            };
        }
    }

    public void Update(float time)
    {
        if (_scrollDirection != 0)
        {
            float num = _scrollDirection * ScrollSpeed * time;
            if (IsStaticScrollSpeed)
            {
                num *= _slider.Max - _slider.Min;
            }
            float value = _slider.Value + num;
            _slider.ChangeValueAndDispatch(value);
        }
    }
}
