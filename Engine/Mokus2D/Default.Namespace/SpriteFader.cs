using Mokus2D.Visual;

namespace Default.Namespace;

public class SpriteFader(Node _target)
{
    private readonly Node target = _target;

    public ushort EnabledOpacity { get; set; }

    public ushort DisabledOpacity { get; set; } = 255;

    public bool Enabled
    {
        get; set
        {
            if (field != value)
            {
                field = value;
                _ = target.Tweener.StartSequence(Duration).Tween(NodeValues.OpacityFloat, field ? EnabledOpacity : DisabledOpacity);
            }
        }
    }

    public float Duration { get; set; } = 0.15f;
}
