using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace ContreJour.Browser
{
    // audio.js: WebAudio effects and the streamed music voice.
    internal static partial class AudioInterop
    {
        public static Task ImportAsync()
        {
            return JSHost.ImportAsync("audio", "../audio.js");
        }

        // 1 once decoded, 0 when the file could not be fetched.
        [JSImport("decodeEffect", "audio")]
        public static partial Task<int> DecodeEffect(string key, string url);

        // 1 once the bytes are held, 0 when the file could not be fetched.
        [JSImport("loadSong", "audio")]
        public static partial Task<int> LoadSong(string key, string url);

        [JSImport("playEffect", "audio")]
        public static partial void PlayEffect(string key, double volume);

        [JSImport("stopEffects", "audio")]
        public static partial void StopEffects();

        [JSImport("setEffectsMuted", "audio")]
        public static partial void SetEffectsMuted(bool muted);

        [JSImport("playSong", "audio")]
        public static partial void PlaySong(string key);

        [JSImport("fadeOutSong", "audio")]
        public static partial void FadeOutSong(double seconds);

        [JSImport("setSongPaused", "audio")]
        public static partial void SetSongPaused(bool paused);

        [JSImport("setSuspended", "audio")]
        public static partial void SetSuspended(bool suspended);
    }
}
