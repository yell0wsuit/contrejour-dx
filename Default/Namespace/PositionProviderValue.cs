namespace Default.Namespace;

public class PositionProviderValue
{
    protected IVectorPositionProvider provider;

    protected float value;

    public float Value => value;

    public IVectorPositionProvider Provider => provider;

    public PositionProviderValue(IVectorPositionProvider _provider, float _value)
    {
        provider = _provider;
        value = _value;
    }
}
