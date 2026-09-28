namespace Mokus2D.Visual.Util;

public static class ResourcesUtil
{
    private const char Separator = '/';

    public static string GetShortObjectName(string objectId)
    {
        return objectId[(objectId.IndexOf('/') + 1)..];
    }
}
