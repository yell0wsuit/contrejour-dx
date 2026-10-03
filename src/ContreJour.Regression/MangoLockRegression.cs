using System;
using System.Linq;
using System.Numerics;

using ContreJour.Gameplay;

using Mokus2D.Visual;
using Mokus2D.Visual.Text;

namespace ContreJour.Regression
{
    internal static class MangoLockRegression
    {
        public static void Run(LevelsMenu currentMenu)
        {
            UserData data = UserData.Instance;
            int chapters = data.UnlockedChapters;
            int levels = data.GetUnlockedLevels(Constants.BonusChapter);
            try
            {
                Verify(currentMenu, data.AvailableBonusLevels, levels);
                // Previously saved level progress must still respect every chapter gate.
                data.SetUnlockedLevelsChapter(20, Constants.BonusChapter);
                for (int unlockedChapters = 1; unlockedChapters <= 6; unlockedChapters++)
                {
                    data.UnlockedChapters = unlockedChapters;
                    using LevelsMenu menu = new(Constants.BonusChapter, Vector2.Zero);
                    int available = Math.Clamp(unlockedChapters - 1, 1, 4) * 5;
                    Verify(menu, available, 20);
                }
            }
            finally
            {
                data.UnlockedChapters = chapters;
                data.SetUnlockedLevelsChapter(levels, Constants.BonusChapter);
            }
        }

        private static void Verify(LevelsMenu menu, int available, int unlockedLevels)
        {
            LevelItem[] items = [.. menu.Children.OfType<LevelItem>().OrderBy(item => item.Index)];
            if (items.Length != 20)
            {
                throw new InvalidOperationException("Mango menu did not create all twenty level items.");
            }
            foreach (LevelItem item in items)
            {
                bool gated = item.Index >= available;
                if (item.Enabled != (!gated && item.Index <= unlockedLevels)
                    || (gated && (item.OpacityFloat != 0.5f || item.Children.Any(child => child.Visible))))
                {
                    throw new InvalidOperationException($"Mango level {item.Index + 1} did not respect its chapter gate.");
                }
            }
            Sprite banner = menu.Children.OfType<Sprite>().SingleOrDefault(sprite =>
                sprite.Children.OfType<Label>().Any(label => label.TextString == "COMPLETE_MORE_CHAPTERS".Localize()));
            bool hasBanner = banner != null;
            if (hasBanner != (available < 20))
            {
                throw new InvalidOperationException("Mango unlock message did not match the locked rows.");
            }
        }
    }
}
