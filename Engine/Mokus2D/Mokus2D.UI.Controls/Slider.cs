using System;

using Microsoft.Xna.Framework;

using Mokus2D.Input;
using Mokus2D.Input.Mouse;
using Mokus2D.Platforms.Input;
using Mokus2D.UI.Layout;
using Mokus2D.Util;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.UI.Controls;

public class Slider : UIComponent, IMouseWheelNode
{
    private readonly MouseWheelNodeController<Slider> _wheelController;

    private readonly LayoutOrientation Orientation;

    private readonly SliderButtons _buttons;

    private readonly float _maxWidth;

    private readonly Sprite _thumb;

    private float ClickAreaSpeed = 1000f;
    private bool _clickAreaStatic;

    private float? _clickAreaTargetPosition;

    private Touch _clickAreaTouch;

    private Vector2 _clickOffset;
    private float _value;

    public bool IsDragging { get; private set; }

    public Sprite MouseWheelArea
    {
        set => _wheelController.MouseWheelArea = value;
    }

    public float MouseWheelSpeed
    {
        get => _wheelController.WheelMult;
        set => _wheelController.WheelMult = value;
    }

    public float MouseWheelMinScrollSpeed
    {
        get => _wheelController.MinScrollSpeed;
        set => _wheelController.MinScrollSpeed = value;
    }

    public float? MouseWheelMaxScrollSpeed
    {
        get => _wheelController.MaxScrollSpeed;
        set => _wheelController.MaxScrollSpeed = value;
    }

    public float MouseWheelScrollSpeedMult
    {
        get => _wheelController.ScrollSpeedMult;
        set => _wheelController.ScrollSpeedMult = value;
    }

    public bool MouseWheelEnabled
    {
        get => _wheelController.Enabled;
        set => _wheelController.Enabled = value;
    }

    public Sprite ClickArea
    {
        get;
        set
        {
            if (field != value)
            {
                if (field != null)
                {
                    field.TouchBeginEvent -= OnClickAreaTouchBegin;
                    field.TouchEndEvent -= OnClickAreaTouchEnd;
                    field.TouchMoveEvent -= OnClickAreaTouchMove;
                    field.TouchOutEvent -= OnClickAreaTouchMove;
                }
                field = value;
                field.Clickable = true;
                field.TouchBeginEvent += OnClickAreaTouchBegin;
                field.TouchEndEvent += OnClickAreaTouchEnd;
                field.TouchMoveEvent += OnClickAreaTouchMove;
                field.TouchOutEvent += OnClickAreaTouchMove;
            }
        }
    }

    public float Min
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                ClampValue();
                SetPropertiesDirty();
            }
        }
    }

    public float Max
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                ClampValue();
                SetPropertiesDirty();
            }
        }
    } = 1f;

    public float Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                ClampValue();
                SetPropertiesDirty();
            }
        }
    }

    public bool IsButtonsSpeedIsStatic
    {
        get => _buttons.IsStaticScrollSpeed;
        set => _buttons.IsStaticScrollSpeed = value;
    }

    public float ButtonsScrollSpeed
    {
        get => _buttons.ScrollSpeed;
        set => _buttons.ScrollSpeed = value;
    }

    public float MinMouseWheelValue => Min;

    public float MaxMouseWheelValue => Max;

    public float MouseWheelValue
    {
        get => Value;
        set
        {
            if (!value.Between(Min, Max))
            {
                _wheelController.Refresh();
            }
            ChangeValueAndDispatch(value);
        }
    }

    // Required by IMouseWheelNode; the slider never raises it.
    public event Action<float> MouseWheelValueChange
    {
        add { }
        remove { }
    }

    public event Action<Slider> ChangeEvent;

    public event Action<Slider> StartEvent;

    public event Action<Slider> EndEvent;

    public Slider(Sprite thumb, float maxWidth, LayoutOrientation orientation = LayoutOrientation.Horizontal)
    {
        _thumb = thumb;
        _thumb.TouchOutResult = true;
        _maxWidth = maxWidth;
        Orientation = orientation;
        if (thumb.Parent != null)
        {
            thumb.Parent.AddChild(this);
            Position = thumb.Position;
            thumb.RemoveFromParent();
            thumb.Position = Vector2.Zero;
        }
        AddChild(_thumb);
        _thumb.Clickable = true;
        thumb.TouchBeginEvent += ThumbOnTouchBeginEvent;
        thumb.TouchMoveEvent += ThumbOnTouchMoveEvent;
        thumb.TouchOutEvent += ThumbOnTouchMoveEvent;
        thumb.TouchEndEvent += ThumbOnTouchEndEvent;
        _buttons = new SliderButtons(this);
        _wheelController = new MouseWheelNodeController<Slider>(this, 1f, 20f, 1f);
        Controller = _wheelController;
        MouseWheelEnabled = false;
    }

    public static Slider CreateFromThumb(Sprite thumb, float maxWidth, LayoutOrientation orientation = LayoutOrientation.Horizontal)
    {
        Node parent = thumb.Parent;
        thumb.RemoveFromParent();
        Slider slider = new(thumb, maxWidth, orientation);
        parent.AddChild(slider);
        slider.Position = thumb.Position;
        thumb.Position = Vector2.Zero;
        return slider;
    }

    public void SetButtons(Sprite leftButton, Sprite rightButton)
    {
        _buttons.Initialize(leftButton, rightButton);
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (_clickAreaTargetPosition.HasValue)
        {
            float num = _clickAreaStatic ? _clickAreaTargetPosition.Value : _thumb.Position.GetValue(Orientation).StepTo(_clickAreaTargetPosition.Value, ClickAreaSpeed * time);
            if (num == _clickAreaTargetPosition)
            {
                _clickAreaStatic = true;
                _clickAreaTargetPosition = null;
            }
            ChangeValue(num);
        }
        _buttons.Update(time);
    }

    public void Refresh()
    {
        _wheelController.Refresh();
    }

    public void ClearTargetPosition()
    {
        _clickAreaTargetPosition = null;
    }

    private void ClampValue()
    {
        _value = _value.Clamp(Min, Max);
    }

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        float value = (Max != Min) ? ((Value - Min) / (Max - Min) * _maxWidth) : Min;
        _thumb.Position = _thumb.Position.Change(value, Orientation);
    }

    private void ThumbOnTouchMoveEvent(TouchArguments touchArguments)
    {
        Vector2 vector = GlobalToLocal(touchArguments.Touch.Position);
        Vector2 targetThumbPosition = _clickOffset + vector;
        ChangeValue(targetThumbPosition);
    }

    private void ChangeValue(Vector2 targetThumbPosition)
    {
        float value = targetThumbPosition.GetValue(Orientation);
        ChangeValue(value);
    }

    private void ChangeValue(float offset)
    {
        float num = offset.Clamp(0f, _maxWidth);
        ChangeValueAndDispatch((num / _maxWidth).Lerp(Min, Max));
    }

    internal void ChangeValueAndDispatch(float value)
    {
        Value = value;
        ChangeEvent.Dispatch(this);
    }

    private void ThumbOnTouchBeginEvent(TouchArguments touchArguments)
    {
        _clickAreaTargetPosition = null;
        _clickAreaTouch = null;
        IsDragging = true;
        touchArguments.Touch.StopPropagation();
        Vector2 vector = GlobalToLocal(touchArguments.Touch.InitialPosition);
        _clickOffset = _thumb.Position - vector;
        _wheelController.Refresh();
        StartEvent.Dispatch(this);
    }

    private void ThumbOnTouchEndEvent(TouchArguments touchArguments)
    {
        IsDragging = false;
        EndEvent.Dispatch(this);
    }

    private void OnClickAreaTouchBegin(TouchArguments arguments)
    {
        if (_clickAreaTouch == null && !IsDragging)
        {
            _clickAreaStatic = false;
            _clickAreaTouch = arguments.Touch;
            RefreshClickAreaPosition();
            _wheelController.Refresh();
            StartEvent.Dispatch(this);
        }
    }

    private void OnClickAreaTouchEnd(TouchArguments arguments)
    {
        if (_clickAreaTouch == arguments.Touch)
        {
            _clickAreaTouch = null;
            EndEvent.Dispatch(this);
        }
    }

    private void OnClickAreaTouchMove(TouchArguments arguments)
    {
        if (_clickAreaTouch == arguments.Touch)
        {
            RefreshClickAreaPosition();
        }
    }

    private void RefreshClickAreaPosition()
    {
        Vector2 value = GlobalToLocal(_clickAreaTouch.Position);
        _clickAreaTargetPosition = value.GetValue(Orientation).Clamp(0f, _maxWidth);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}
