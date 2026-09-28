namespace Default.Namespace;

public static class ContreJourConditions
{
    public static T Trial<T>(T trialValue, T value)
    {
        if (!Constants.IsTrial)
        {
            return value;
        }
        return trialValue;
    }
}
