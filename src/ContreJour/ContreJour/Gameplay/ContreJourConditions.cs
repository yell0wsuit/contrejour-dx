namespace ContreJour.Gameplay;

public static class ContreJourConditions
{
    public static T Trial<T>(T trialValue, T value)
    {
        return !Constants.IsTrial ? value : trialValue;
    }
}
