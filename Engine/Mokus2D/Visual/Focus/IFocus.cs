using System;

namespace Mokus2D.Visual.Focus;

public interface IFocus
{
    bool HasFocus { get; set; }

    event Action<IFocus> FocusInEvent;

    event Action<IFocus> FocusOutEvent;
}
