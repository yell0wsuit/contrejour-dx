namespace ContreJourDX.Gameplay
{
    public interface IPositionDepedent
    {
        void ProviderRemove(IVectorPositionProvider provider);

        void ProviderAdded(IVectorPositionProvider provider);
    }
}
