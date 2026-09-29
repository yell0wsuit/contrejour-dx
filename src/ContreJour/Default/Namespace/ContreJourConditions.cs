namespace Default.Namespace;

public static class ContreJourConditions
{
    public static T Trial<T>(T trialValue, T value)
    {
        return !Constants.IsTrial ? value : trialValue;
    }
}
