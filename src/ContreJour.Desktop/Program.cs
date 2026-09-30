using System;

using ContreJour.Desktop.Platform.Audio;

using Mokus2D.Sound;

using SDL3;

namespace ContreJour.Desktop
{
    public static class Program
    {
        [STAThread]
        private static void Main()
        {
            // Content paths are relative to the install folder, as they were inside the appx package.
            Environment.CurrentDirectory = AppContext.BaseDirectory;
            Run();
            // The game and the audio backend each quit only the SDL subsystems they started; this
            // releases SDL's global state (hints, properties, thread data) once both are done.
            SDL.Quit();
        }

        private static void Run()
        {
            // Declared before the game, so the game is disposed first.
            using IAudioBackend audio = OpenAudio();
            using SdlApplication<ContreJourApplication> game = new(audio);
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
