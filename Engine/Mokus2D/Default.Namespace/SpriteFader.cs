using Mokus2D.Visual;

namespace Default.Namespace;

public class SpriteFader(Node _target)
{
    protected ushort disabledOpacity = 255;

    protected float duration = 0.15f;

    protected bool enabled;

    protected ushort enabledOpacity = 0;

    protected Node target = _target;

    public ushort EnabledOpacity
    {
        get => enabledOpacity;
        set => enabledOpacity = value;
    }

    public ushort DisabledOpacity
    {
        get => disabledOpacity;
        set => disabledOpacity = value;
    }

    public bool Enabled
    {
        get => enabled;
        set
        {
            if (enabled != value)
            {
                enabled = value;
                _ = target.Tweener.StartSequence(duration).Tween(NodeValues.OpacityFloat, enabled ? enabledOpacity : disabledOpacity);
            }
        }
    }

    public float Duration
    {
        get => duration;
        set => duration = value;
    }
}
