using Mokus2D.Behaviour;
using Mokus2D.Input.Mouse;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;
using Mokus2D.Visual.Interactive;

namespace Mokus2D.Platforms.Input;

public class MouseWheelNodeController<T> : NodeController<T> where T : Node, IMouseWheelNode
{
    public float WheelMult = 1f;

    public float MinScrollSpeed = 1f;

    public float ScrollSpeedMult = 1f;

    public float? MaxScrollSpeed = null;

    private float? _targetValue;

    public bool Enabled = true;

    private Sprite _mouseWheelArea;

    public Sprite MouseWheelArea
    {
        set => _mouseWheelArea = value;
    }

    public MouseWheelNodeController(T node, float wheelMult, float minScrollSpeed, float scrollSpeedMult)
        : this(node)
    {
        WheelMult = wheelMult;
        MinScrollSpeed = minScrollSpeed;
        ScrollSpeedMult = scrollSpeedMult;
    }

    public MouseWheelNodeController(T node)
        : base(node)
    {
        node.MouseWheelValueChange += OnMouseWheelValueChange;
    }

    public override void Update(float time)
    {
        base.Update(time);
        if (_targetValue.HasValue)
        {
            float scrollSpeedMult = ScrollSpeedMult;
            T node = Node;
            float value = scrollSpeedMult * (node.MouseWheelValue - _targetValue.Value).Abs();
            value = value.Clamp(MinScrollSpeed, MaxScrollSpeed ?? float.PositiveInfinity);
            T node2 = Node;
            T node3 = Node;
            node2.MouseWheelValue = node3.MouseWheelValue.StepTo(_targetValue.Value, value * time);
            T node4 = Node;
            if (node4.MouseWheelValue == _targetValue)
            {
                _targetValue = null;
            }
        }
    }

    public override void OnAddedToStage()
    {
        base.OnAddedToStage();
        MouseController.ScrollEvent += OnScroll;
    }

    public override void OnRemovedFromStage()
    {
        base.OnRemovedFromStage();
        MouseController.ScrollEvent -= OnScroll;
    }

    protected virtual void OnScroll(int value)
    {
        T node = Node;
        if (!node.RootVisible || !Enabled || (_mouseWheelArea != null && !_mouseWheelArea.ContainsGlobalPosition(MouseController.CursorPosition)))
        {
            return;
        }
        if (_targetValue.HasValue)
        {
            float num = value.Sign() * WheelMult.Sign();
            float value2 = _targetValue.Value;
            T node2 = Node;
            if (num != (value2 - node2.MouseWheelValue).Sign())
            {
                _targetValue = null;
                return;
            }
        }
        float? targetValue = _targetValue;
        float num2;
        if (!targetValue.HasValue)
        {
            T node3 = Node;
            num2 = node3.MouseWheelValue;
        }
        else
        {
            num2 = targetValue.GetValueOrDefault();
        }
        _targetValue = num2 + (value * WheelMult);
    }

    private void OnMouseWheelValueChange(float obj)
    {
        _targetValue = null;
    }

    public void Refresh()
    {
        _targetValue = null;
    }
}
