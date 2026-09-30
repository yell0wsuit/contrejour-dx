using System;
using System.Reflection;

using ContreJour.Desktop.Platform;
using ContreJour.Desktop.Platform.Audio;
using ContreJour.Desktop.Platform.Diagnostics;
using ContreJour.Saving;

using Microsoft.Extensions.Logging;

using Mokus2D.Sound;

using SDL3;

namespace ContreJour.Desktop
{
    public static class Program
    {
        private const string ProductName = "Contre Jour";

        private static string Version =>
            typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";

        [STAThread]
        private static int Main(string[] args)
        {
            // Content paths are relative to the install folder, as they were inside the appx package.
            Environment.CurrentDirectory = AppContext.BaseDirectory;
            LogLevel? level;
            DesktopOptions options;
            try
            {
                level = LoggingSetup.ParseLevel(args);
                options = DesktopOptions.Parse(args);
            }
            catch (ArgumentException failure)
            {
                Console.Error.WriteLine(failure.Message);
                return 2;
            }
            string saveDirectory = Preferences.SaveDirectory;
            ILoggerFactory factory = LoggingSetup.Create(saveDirectory, level, LoggingSetup.ComposeHeader(ProductName, Version));
            Log.Factory = factory;
            CrashDialog.Title = ProductName;
            CrashDialog.LogDirectory = LoggingSetup.DirectoryFor(saveDirectory);
            // A scripted run has nobody to press the dialog's button.
            CrashDialog.Enabled = options.QuitAfterFrames == null;
            CrashHandlers.Configure(factory);
            CrashHandlers.Install();
            try
            {
                Run(options);
                return 0;
            }
            catch (Exception failure)
            {
                CrashHandlers.ReportFatal(failure);
                return 1;
            }
            finally
            {
                Log.Factory = null;
                factory.Dispose();
                // The game and the audio backend each quit only the SDL subsystems they started; this
                // releases SDL's global state (hints, properties, thread data) once both are done.
                SDL.Quit();
            }
        }

        private static void Run(DesktopOptions options)
        {
            // Declared before the game, so the game is disposed first.
            using IAudioBackend audio = OpenAudio();
            using SdlApplication<ContreJourApplication> game = new(audio, options);
            game.Run();
        }

        // A machine without a usable audio device still runs the game, silently.
        private static IAudioBackend OpenAudio()
        {
            SdlAudioBackend audio = SdlAudioBackend.TryOpen(out string error);
            if (audio != null)
            {
                return audio;
            }
            HostLog.AudioUnavailable(Log.For(LogCategories.Audio), error);
            return new NullAudioBackend();
        }
    }
}
