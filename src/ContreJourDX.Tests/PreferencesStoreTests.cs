using System.Collections.Generic;

using ContreJourDX.Saving;

using Xunit;

namespace ContreJourDX.Tests
{
    public class PreferencesStoreTests
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
        public void AReplacedStoreReceivesSavesAndServesLoads()
        {
            MemoryStore store = new();
            Preferences.Store = store;
            try
            {
                Preferences.Load();
                Preferences.Settings.SetInt("probe", 7);
                Preferences.RequestSave();
                Preferences.Update(force: true);

                Assert.Contains("probe", store.Blobs[Preferences.Settings.FileName]);

                Preferences.Settings.SetInt("probe", 0);
                Preferences.Load();
                Assert.Equal(7, Preferences.Settings.GetInt("probe"));
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
