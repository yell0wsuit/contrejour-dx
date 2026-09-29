using System;

namespace Mokus2D.Effects.Tweening;

public struct TargetAndAction(object target, Action<object> action)
{
    private readonly object Target = target;

    private readonly Action<object> Action = action;

    public readonly void Execute()
    {
        Action(Target);
    }
}
