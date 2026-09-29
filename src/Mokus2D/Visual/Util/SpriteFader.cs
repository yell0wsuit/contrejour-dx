namespace Mokus2D.Visual.Util
{
    public class SpriteFader(Node target)
    {
        private readonly Node target = target;

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
}
