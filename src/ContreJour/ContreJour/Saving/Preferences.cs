using System;
using System.IO;

using Microsoft.Extensions.Logging;

using Mokus2D.Diagnostics;

namespace ContreJour.Saving
{
    /// <summary>
    /// Owns the save files and when they are written: settings in <c>contrejour_preferences.json</c>,
    /// progress for every chapter in <c>contrejour_gamesave.json</c>. Changes mark their file dirty;
    /// <see cref="RequestSave"/> asks for a write, which <see cref="Update"/> performs from the game loop,
    /// retrying with a backoff if the disk refuses it.
    /// </summary>
    internal static class Preferences
    {
        private const string SaveFolderName = "ContreJourDX_SaveData";

        /// <summary>How many consecutive write failures are tolerated before giving up.</summary>
        private const int MaxSaveAttempts = 5;

        /// <summary>Delay before the first retry; each further attempt doubles it.</summary>
        private const long FirstRetryDelayMs = 250;
        private static int saveAttempts;

        private static long retryAfterTicks;

        /// <summary>Gets the settings file (sound, music, first-launch flags).</summary>
        public static PreferenceFile Settings { get; } = new("contrejour_preferences.json");

        /// <summary>Gets the progress file (unlocks, scores, stars and stats for every chapter).</summary>
        public static PreferenceFile GameSave { get; } = new("contrejour_gamesave.json");

        /// <summary>Gets a value indicating whether a write is pending.</summary>
        public static bool SaveRequested { get; private set; }

        /// <summary>
        /// Gets or sets the save directory. Resolved on first use: next to the executable (portable),
        /// then Documents, then LocalApplicationData. Setting it (tests) replaces the store.
        /// </summary>
        public static string SaveDirectory
        {
            get => field ??= DetermineSaveDirectory();
            set
            {
                field = value;
                Store = null;
            }
        }

        /// <summary>
        /// Gets or sets where the files live. Defaults to JSON files in <see cref="SaveDirectory"/>; a host
        /// without a file system (the browser) sets its own before <see cref="Load"/>. Null restores the default.
        /// </summary>
        internal static IPreferenceStore Store { get => field ??= new FilePreferenceStore(SaveDirectory); set; }

        /// <summary>Loads both files from the store; a missing or unreadable file starts empty.</summary>
        public static void Load()
        {
            LoadFile(Settings);
            LoadFile(GameSave);
            ILogger logger = Log.For(LogCategories.Preferences);
            if (logger.IsEnabled(LogLevel.Information))
            {
                PreferenceLog.Loaded(logger, Store.GetType().Name);
            }
        }

        /// <summary>Requests a write on the next <see cref="Update"/>.</summary>
        public static void RequestSave()
        {
            SaveRequested = true;
        }

        /// <summary>Marks both files as needing a write, whether or not they changed.</summary>
        public static void MarkAllDirty()
        {
            Settings.Dirty = true;
            GameSave.Dirty = true;
        }

        /// <summary>
        /// Writes pending changes if a save was requested. Called by the game loop; <paramref name="force"/>
        /// skips the retry backoff (on exit).
        /// </summary>
        /// <param name="force"><see langword="true"/> to write now even while backing off.</param>
        public static void Update(bool force = false)
        {
            if (!SaveRequested)
            {
                return;
            }

            if (!force && saveAttempts > 0 && Environment.TickCount64 < retryAfterTicks)
            {
                return;
            }

            try
            {
                WriteFiles();
                SaveRequested = false;
                saveAttempts = 0;
            }
            catch (IOException failure)
            {
                RetryLater(failure);
            }
            catch (UnauthorizedAccessException failure)
            {
                RetryLater(failure);
            }
        }

        private static void RetryLater(Exception failure)
        {
            saveAttempts++;
            ILogger logger = Log.For(LogCategories.Preferences);
            if (saveAttempts >= MaxSaveAttempts)
            {
                PreferenceLog.SaveAbandoned(logger, saveAttempts, failure);
                // The request is dropped so a permanently failing disk isn't retried all session. The
                // dirty marks stay: the next save request picks the change up again.
                SaveRequested = false;
                saveAttempts = 0;
                return;
            }

            PreferenceLog.SaveFailed(logger, saveAttempts, failure);
            retryAfterTicks = Environment.TickCount64 + (FirstRetryDelayMs << (saveAttempts - 1));
        }

        private static void WriteFiles()
        {
            // Each file is marked clean as it lands, so a failure part way through doesn't rewrite what
            // already succeeded when the save is retried.
            foreach (PreferenceFile file in (ReadOnlySpan<PreferenceFile>)[Settings, GameSave])
            {
                if (file.Dirty)
                {
                    Store.Write(file.FileName, file.ToJson());
                    file.Dirty = false;
                    ILogger logger = Log.For(LogCategories.Preferences);
                    PreferenceLog.Saved(logger, file.FileName);
                }
            }
        }

        private static void LoadFile(PreferenceFile file)
        {
            file.Clear();
            try
            {
                string json = Store.Read(file.FileName);
                if (json != null)
                {
                    file.LoadJson(json);
                }
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Text.Json.JsonException)
            {
                file.Clear();
                ILogger logger = Log.For(LogCategories.Preferences);
                PreferenceLog.LoadFailed(logger, file.FileName, e);
            }
        }

        private static string DetermineSaveDirectory()
        {
            // The executable's own directory first (portable). A macOS .app is read-only and
            // code-signed, so a bundled run saves under Documents instead.
            string exeDir = AppContext.BaseDirectory;
            if (!IsInsideMacAppBundle(exeDir))
            {
                string exeSaveDir = Path.Combine(exeDir, SaveFolderName);
                if (TryCreateDirectory(exeSaveDir))
                {
                    return exeSaveDir;
                }
            }

            string documentsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SaveFolderName);
            if (TryCreateDirectory(documentsDir))
            {
                return documentsDir;
            }

            string localAppDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), SaveFolderName);
            return TryCreateDirectory(localAppDataDir) ? localAppDataDir : ".";
        }

        /// <summary>Creates the directory if needed and checks it can be written to.</summary>
        private static bool TryCreateDirectory(string path)
        {
            try
            {
                _ = Directory.CreateDirectory(path);
                string testFile = Path.Combine(path, ".write_test_" + Guid.NewGuid().ToString("N"));
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException or NotSupportedException)
            {
                return false;
            }
        }

        /// <summary>Returns whether <paramref name="path"/> is inside a macOS bundle (*.app/Contents/MacOS).</summary>
        private static bool IsInsideMacAppBundle(string path)
        {
            for (DirectoryInfo dir = new(path); dir != null; dir = dir.Parent)
            {
                if (dir.Name.Equals("MacOS", StringComparison.OrdinalIgnoreCase)
                    && dir.Parent?.Name.Equals("Contents", StringComparison.OrdinalIgnoreCase) == true
                    && dir.Parent.Parent?.Name.EndsWith(".app", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
