using System.Collections.Generic;

namespace ContreJour.Utils
{
    public static class XBoxUtil
    {
        private static readonly List<string> awardedAchievements = new(64);

        public static void AwardAchievement(string achievement)
        {
            if (!Gameplay.Constants.IsTrial)
            {
                _ = awardedAchievements.Contains(achievement);
            }
        }
    }
}
