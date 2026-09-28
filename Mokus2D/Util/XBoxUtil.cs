using System.Collections.Generic;

namespace Mokus2D.Util;

public static class XBoxUtil
{
    private static readonly List<string> awardedAchievements = new(64);

    public static void AwardAchievement(string achievement)
    {
        if (!Default.Namespace.Constants.IsTrial)
        {
            _ = awardedAchievements.Contains(achievement);
        }
    }
}
