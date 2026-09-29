namespace Default.Namespace;

public class PositionProviderValue(IVectorPositionProvider _provider, float _value)
{
    private readonly IVectorPositionProvider provider = _provider;

    private readonly float value = _value;

    public float Value => value;

    public IVectorPositionProvider Provider => provider;
}
