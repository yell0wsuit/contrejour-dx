namespace Default.Namespace;

public class PositionProviderValue(IVectorPositionProvider _provider, float _value)
{
    public float Value { get; } = _value;

    public IVectorPositionProvider Provider { get; } = _provider;
}
