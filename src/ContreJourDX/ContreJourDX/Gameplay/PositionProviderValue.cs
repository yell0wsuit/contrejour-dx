namespace ContreJourDX.Gameplay
{
    public class PositionProviderValue(IVectorPositionProvider provider, float value)
    {
        public float Value { get; } = value;

        public IVectorPositionProvider Provider { get; } = provider;
    }
}
