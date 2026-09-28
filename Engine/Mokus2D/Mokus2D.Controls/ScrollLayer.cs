using System;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.Controls;

public class ScrollLayer : ClickableLayer, ITouchListener
{
    public const float MinReturnStep = 1f;

    public const float InertiaChange = 2f;

    public const float InertiaChangeOffLimit = 3f;

    public const float MaxInertia = 100f;

    public const float MaxInertiaStep = 20f;

    public Vector2 MinPosition = Vector2.Zero;

    public Vector2 MaxPosition = Vector2.Zero;

    public bool InertiaHorizontal;

    public bool InertiaVertical = true;

    private Touch _scrollTouch;

    private Vector2 _initialPosition;

    private Vector2 _initialTouchPosition;

    private Vector2 _previousTouchPosition;

    private Vector2 _inertia = Vector2.Zero;

    private float _returnCoeff = 0.2f;

    public float ReturnCoeff
    {
        get => _returnCoeff;
        set
        {
            if (_returnCoeff != value)
            {
                _returnCoeff = value;
            }
        }
    }

    public bool Scrolling => _scrollTouch != null;

    public void RefreshBorders()
    {
        MinPosition = MaxPosition = Position;
    }

    public override bool TouchMove(Touch touch)
    {
        if (_scrollTouch == touch)
        {
            Vector2 touchPosition = GetTouchPosition();
            Vector2 inertia = _inertia;
            _inertia = touchPosition - _previousTouchPosition;
            _inertia.X = FixInertiaValue(inertia.X, _inertia.X);
            _inertia.Y = FixInertiaValue(inertia.Y, _inertia.Y);
            _previousTouchPosition = touchPosition;
            Position = _initialPosition + touchPosition - _initialTouchPosition;
            EnsureBounds();
        }
        return true;
    }

    private float FixInertiaValue(float old, float current)
    {
        float value = current;
        if (current.Sign() == old.Sign() && old.Abs() > current.Abs())
        {
            value = old.StepTo(current, 20f);
        }
        return value.Clamp(-100f, 100f);
    }

    public override bool TouchBegin(Touch touch)
    {
        if (_scrollTouch == null)
        {
            _scrollTouch = touch;
            _initialPosition = Position;
            _initialTouchPosition = GetTouchPosition();
            _previousTouchPosition = _initialTouchPosition;
        }
        return true;
    }

    private Vector2 GetTouchPosition()
    {
        return Parent.GlobalToLocal(_scrollTouch.Position);
    }

    public new void TouchEnd(Touch touch)
    {
        if (_scrollTouch == touch)
        {
            _scrollTouch = null;
        }
    }

    private float GetTimeValue(float value, float time)
    {
        return value * 60f * time;
    }

    public override void Update(float time)
    {
        if (_scrollTouch != null)
        {
            return;
        }
        if (InertiaHorizontal)
        {
            X += _inertia.X;
            _inertia.X = _inertia.X.StepTo(0f, GetTimeValue(2f, time));
            if (!X.Between(MinPosition.X, MaxPosition.X))
            {
                X = GetReturnValue(X, MinPosition.X, MaxPosition.X, time);
                _inertia.X = _inertia.X.StepTo(0f, GetTimeValue(3f, time));
            }
        }
        if (InertiaVertical)
        {
            Y += _inertia.Y;
            _inertia.Y = _inertia.Y.StepTo(0f, GetTimeValue(2f, time));
            if (!Y.Between(MinPosition.Y, MaxPosition.Y))
            {
                Y = GetReturnValue(Y, MinPosition.Y, MaxPosition.Y, time);
                _inertia.Y = _inertia.Y.StepTo(0f, GetTimeValue(3f, time));
            }
        }
    }

    private float GetReturnValue(float current, float min, float max, float time)
    {
        return current < min ? StepToBorder(current, min, time) : current > max ? StepToBorder(current, max, time) : current;
    }

    private float StepToBorder(float current, float max, float time)
    {
        float value = Math.Max(Math.Abs(current - max) * _returnCoeff, 1f);
        return current.StepTo(max, GetTimeValue(value, time));
    }

    private void EnsureBounds()
    {
        if (!InertiaHorizontal)
        {
            X = X.Clamp(MinPosition.X, MaxPosition.X);
        }
        if (!InertiaVertical)
        {
            Y = Y.Clamp(MinPosition.Y, MaxPosition.Y);
        }
    }
}
