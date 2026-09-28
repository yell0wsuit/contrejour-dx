using System.Collections.Generic;

namespace Mokus2D.Util;

public static class XBoxUtil
{
    private static List<string> awardedAchievements = new(64);

    private static bool achievementShown;

    public static void AwardAchievement(string achievement)
    {
        if (!Default.Namespace.Constants.IsTrial)
        {
            _ = awardedAchievements.Contains(achievement);
        }
    }
}
