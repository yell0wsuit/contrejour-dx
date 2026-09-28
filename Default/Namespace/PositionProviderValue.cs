namespace Default.Namespace;

public class PositionProviderValue(IVectorPositionProvider _provider, float _value)
{
    protected IVectorPositionProvider provider = _provider;

    protected float value = _value;

    public float Value => value;

    public IVectorPositionProvider Provider => provider;
}
