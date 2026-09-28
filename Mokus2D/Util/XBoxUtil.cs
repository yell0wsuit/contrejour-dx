using System.Collections.Generic;

using Default.Namespace;

namespace Mokus2D.Util;

public static class XBoxUtil
{
    private static List<string> awardedAchievements = new List<string>(64);

    private static bool achievementShown;

    public static void AwardAchievement(string achievement)
    {
        if (!Default.Namespace.Constants.IsTrial)
        {
            awardedAchievements.Contains(achievement);
        }
    }
}
