using System;

namespace Mokus2D.Input;

internal readonly struct ActionPriority(Action action, int priority)
{
    public readonly Action Action = action;

    public readonly int Priority = priority;
}
