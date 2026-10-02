using System;
using System.IO;

using ContreJour.Saving;

using Microsoft.Extensions.Logging;

using Mokus2D.Diagnostics;
using Mokus2D.Tests;

using Xunit;

namespace ContreJour.Tests
{
    public sealed class PreferenceLoggingTests
    {
        private sealed class FailingStore : IPreferenceStore
        {
            public Exception Failure { get; } = new IOException("disk refused the save");
            public string Read(string name)
            {
                return "{invalid json";
            }
            public void Write(string name, string contents)
            {
                throw Failure;
            }
        }

        [Fact]
        public void CorruptLoadsAndExhaustedSaveRetriesKeepTheirDiagnostics()
        {
            using RecordingLoggerFactory factory = new();
            FailingStore store = new();
            Log.Factory = factory;
            Preferences.Store = store;
            try
            {
                Preferences.Load();
                Assert.Equal(2, factory.Entries.FindAll(entry => entry.Level == LogLevel.Warning).Count);
                Assert.All(factory.Entries.FindAll(entry => entry.Level == LogLevel.Warning), entry =>
                {
                    Assert.Equal(LogCategories.Preferences, entry.Category);
                    Assert.Equal(LogLevel.Warning, entry.Level);
                    Assert.NotNull(entry.Exception);
                });
                Preferences.Settings.SetInt("probe", 7);
                Preferences.RequestSave();
                for (int attempt = 0; attempt < 5; attempt++)
                {
                    Preferences.Update(force: true);
                }
                Assert.False(Preferences.SaveRequested);
                Assert.True(Preferences.Settings.Dirty);
                Assert.Contains(factory.Entries, entry => entry.Level == LogLevel.Error && entry.Message.Contains('5'));
                Assert.Contains(factory.Entries, entry => ReferenceEquals(store.Failure, entry.Exception));
            }
            finally
            {
                Log.Factory = null;
                Preferences.Store = null;
                Preferences.Settings.Clear();
                Preferences.GameSave.Clear();
            }
        }
    }
}
