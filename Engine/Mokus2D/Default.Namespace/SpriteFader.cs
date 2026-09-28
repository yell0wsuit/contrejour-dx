using Mokus2D.Visual;

namespace Default.Namespace;

public class SpriteFader
{
    protected ushort disabledOpacity;

    protected float duration;

    protected bool enabled;

    protected ushort enabledOpacity;

    protected Node target;

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

    public SpriteFader(Node _target)
    {
        target = _target;
        enabledOpacity = 0;
        disabledOpacity = 255;
        duration = 0.15f;
    }
}
