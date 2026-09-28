namespace Default.Namespace;

public class PositionProviderValue(IVectorPositionProvider _provider, float _value)
{
    private IVectorPositionProvider provider = _provider;

    private float value = _value;

    public float Value => value;

    public IVectorPositionProvider Provider => provider;
}
