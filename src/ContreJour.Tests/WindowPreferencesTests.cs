using System.Collections.Generic;

using ContreJour.Desktop;
using ContreJour.Desktop.Platform;
using ContreJour.Saving;

using Xunit;

namespace ContreJour.Tests
{
    public class WindowPreferencesTests
    {
        private sealed class MemoryStore : IPreferenceStore
        {
            public Dictionary<string, string> Blobs { get; } = [];
            public string Read(string name)
            {
                return Blobs.GetValueOrDefault(name);
            }
            public void Write(string name, string contents)
            {
                Blobs[name] = contents;
            }
        }

        [Fact]
        public void MissingDesktopPreferencesKeepTheFirstLaunchFullscreenDefault()
        {
            Preferences.Settings.Clear();
            (WindowPlacement placement, bool fullScreen) = WindowPreferences.Read();
            Assert.True(fullScreen);
            Assert.False(placement.Maximized);
            Assert.Equal((0, 0), (placement.Width, placement.Height));
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public void WindowPlacementSurvivesSavingAndReloading(bool fullscreen, bool maximized)
        {
            MemoryStore store = new();
            Preferences.Store = store;
            try
            {
                Preferences.Load();
                Preferences.Settings.SetBool("SOUND_ON", false);
                WindowPreferences.Save(new WindowPlacement(1000, 700, maximized), fullscreen);
                Assert.True(Preferences.SaveRequested);
                Preferences.Update(force: true);
                Preferences.Load();
                (WindowPlacement placement, bool fullScreen) = WindowPreferences.Read();
                Assert.Equal(fullscreen, fullScreen);
                Assert.Equal(maximized, placement.Maximized);
                Assert.Equal((1000, 700), (placement.Width, placement.Height));
                Assert.False(Preferences.Settings.GetBool("SOUND_ON", true));
            }
            finally
            {
                Preferences.Store = null;
                Preferences.Settings.Clear();
                Preferences.GameSave.Clear();
            }
        }
    }
}
