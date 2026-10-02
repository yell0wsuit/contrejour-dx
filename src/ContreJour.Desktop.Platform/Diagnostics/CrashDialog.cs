using System;
using System.IO;

using SDL3;

namespace ContreJour.Desktop.Platform.Diagnostics
{
    // What a player sees when the game stops on its own: what happened, and the log folder, which
    // is what makes the failure reportable.
    public static class CrashDialog
    {
        private const int CloseButton = 0;

        private const int OpenLogButton = 1;

        // Off for scripted runs: nobody is there to press a button, and a modal box would hang them.
        public static bool Enabled { get; set; } = true;

        public static string Title { get; set; } = "Contre Jour";

        // This run's log folder and actual file, if logging did not fall back to the console.
        public static string LogDirectory { get; set; }

        public static string LogFilePath { get; set; }

        internal static bool HasFileLog => !string.IsNullOrEmpty(LogFilePath) && File.Exists(LogFilePath);

        public static void Show(string message)
        {
            if (!Enabled)
            {
                return;
            }
            bool offerLog = HasFileLog && !string.IsNullOrEmpty(LogDirectory) && Directory.Exists(LogDirectory);
            // Enter and Escape both only dismiss, so neither opens a folder by accident.
            NativeMessageBox.Button close = new(CloseButton, "Close",
                SDL.MessageBoxButtonFlags.ReturnkeyDefault | SDL.MessageBoxButtonFlags.EscapekeyDefault);
            int pressed = offerLog
                ? NativeMessageBox.Show(SDL.MessageBoxFlags.Error, Title, message, CloseButton,
                    new NativeMessageBox.Button(OpenLogButton, "Open log folder"), close)
                : NativeMessageBox.Show(SDL.MessageBoxFlags.Error, Title, message, CloseButton, close);
            if (pressed == OpenLogButton && offerLog)
            {
                // Best effort: the game has already failed, and a desktop without a file browser is
                // not a reason to fail again on the way out.
                _ = SDL.OpenURL(new Uri(Path.GetFullPath(LogDirectory)).AbsoluteUri);
            }
        }
    }
}
