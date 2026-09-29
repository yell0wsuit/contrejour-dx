using System;

namespace Mokus2D.Input.Mouse
{
    public interface IMouseWheelNode
    {
        float MouseWheelValue { get; set; }

        float MinMouseWheelValue { get; }

        float MaxMouseWheelValue { get; }

        event Action<float> MouseWheelValueChange;
    }
}
