using System;

using ContreJour.Desktop.MonoGame;
using ContreJour.Desktop.Platform.Audio;

using Mokus2D.Sound;

namespace ContreJour.Desktop
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Content paths are relative to the install folder, as they were inside the appx package.
            Environment.CurrentDirectory = AppContext.BaseDirectory;
            // Declared before the game, so the game is disposed first.
            using IAudioBackend audio = OpenAudio();
            using MonoGameApplication<ContreJourApplication> game = new(audio);
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
            Console.Error.WriteLine($"Audio unavailable: {error}");
            return new NullAudioBackend();
        }
    }
}
