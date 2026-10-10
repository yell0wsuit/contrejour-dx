namespace ContreJourDX.Gameplay
{
    public static class ContreJourDXConditions
    {
        public static T Trial<T>(T trialValue, T value)
        {
            return !Constants.IsTrial ? value : trialValue;
        }
    }
}
