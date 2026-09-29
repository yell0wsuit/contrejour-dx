namespace Default.Namespace;

public interface IPositionDepedent
{
    void ProviderRemove(IVectorPositionProvider provider);

    void ProviderAdded(IVectorPositionProvider provider);
}
