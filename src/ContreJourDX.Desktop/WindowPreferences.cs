using ContreJourDX.Desktop.Platform;
using ContreJourDX.Saving;

namespace ContreJourDX.Desktop
{
    // Desktop-only keys in the existing settings file; game settings and progress share its store.
    internal static class WindowPreferences
    {
        private const string WidthKey = "PREFS_WINDOW_WIDTH";
        private const string HeightKey = "PREFS_WINDOW_HEIGHT";
        private const string MaximizedKey = "PREFS_WINDOW_MAXIMIZED";
        private const string FullScreenKey = "PREFS_WINDOW_FULLSCREEN";

        public static (WindowPlacement Placement, bool FullScreen) Read()
        {
            PreferenceFile settings = Preferences.Settings;
            return (new WindowPlacement(settings.GetInt(WidthKey), settings.GetInt(HeightKey), settings.GetBool(MaximizedKey)),
                settings.GetBool(FullScreenKey, true));
        }

        public static void Save(WindowPlacement placement, bool fullScreen)
        {
            PreferenceFile settings = Preferences.Settings;
            if (settings.Contains(WidthKey) && settings.Contains(HeightKey)
                && settings.Contains(MaximizedKey) && settings.Contains(FullScreenKey)
                && settings.GetInt(WidthKey) == placement.Width && settings.GetInt(HeightKey) == placement.Height
                && settings.GetBool(MaximizedKey) == placement.Maximized && settings.GetBool(FullScreenKey) == fullScreen)
            {
                return;
            }
            settings.SetInt(WidthKey, placement.Width);
            settings.SetInt(HeightKey, placement.Height);
            settings.SetBool(MaximizedKey, placement.Maximized);
            settings.SetBool(FullScreenKey, fullScreen);
            Preferences.RequestSave();
        }
    }
}
