using Mokus2D.Visual;

namespace Default.Namespace;

public class SpriteFader(Node _target)
{
    private ushort disabledOpacity = 255;

    private float duration = 0.15f;

    private bool enabled;

    private ushort enabledOpacity;

    private Node target = _target;

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
