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

        // This run's log folder. The button to open it is offered only when it exists: logging can
        // have fallen back to the console.
        public static string LogDirectory { get; set; }

        public static void Show(string message)
        {
            if (!Enabled)
            {
                return;
            }
            bool offerLog = !string.IsNullOrEmpty(LogDirectory) && Directory.Exists(LogDirectory);
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
