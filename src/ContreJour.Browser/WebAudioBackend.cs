using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using ContreJour.Browser.Platform;

using Mokus2D.Sound;

namespace ContreJour.Browser
{
    // IAudioBackend over audio.js. Everything is fetched before Play (effects decoded, songs held as bytes),
    // so loading only resolves a path to its preloaded key; a path that was never preloaded is missing.
    internal sealed class WebAudioBackend : IAudioBackend
    {
        private sealed record Clip(string Key) : ISoundEffect, ISong;

        // Normalized path -> the catalog's own spelling, which is the key audio.js holds.
        private readonly Dictionary<string, string> _effects = new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, string> _songs = new(StringComparer.OrdinalIgnoreCase);

        public bool SoundsMuted
        {
            get;
            set
            {
                field = value;
                AudioInterop.SetEffectsMuted(value);
            }
        }

        public bool SongPaused
        {
            get;
            set
            {
                field = value;
                AudioInterop.SetSongPaused(value);
            }
        }

        // Held still while the page is away, as the desktop host suspends its audio device.
        public bool Suspended
        {
            get;
            set
            {
                field = value;
                AudioInterop.SetSuspended(value);
            }
        }

        public async Task PreloadAsync(string baseUrl, ContentCatalog catalog)
        {
            ArgumentNullException.ThrowIfNull(catalog);
            await ContentPreloader.RunAsync("sounds", catalog.Sounds, async path =>
            {
                Require(await AudioInterop.DecodeEffect(path, baseUrl + path), path);
                _effects[BrowserFileLoader.Normalize(path)] = path;
            });
            await ContentPreloader.RunAsync("music", catalog.Songs, async path =>
            {
                Require(await AudioInterop.LoadSong(path, baseUrl + path), path);
                _songs[BrowserFileLoader.Normalize(path)] = path;
            });
        }

        public ISoundEffect LoadSound(string path)
        {
            return new Clip(Resolve(_effects, path));
        }

        public ISong LoadSong(string path)
        {
            return new Clip(Resolve(_songs, path));
        }

        public void PlaySound(ISoundEffect sound, float volume)
        {
            AudioInterop.PlayEffect(((Clip)sound).Key, volume);
        }

        public void StopAllSounds()
        {
            AudioInterop.StopEffects();
        }

        public void PlaySong(ISong song)
        {
            SongPaused = false;
            AudioInterop.PlaySong(((Clip)song).Key);
        }

        public void FadeOutSong(float seconds)
        {
            AudioInterop.FadeOutSong(seconds);
        }

        public void Dispose()
        {
        }

        private static string Resolve(Dictionary<string, string> preloaded, string path)
        {
            return preloaded.TryGetValue(BrowserFileLoader.Normalize(path), out string key)
                ? key
                : throw new FileNotFoundException($"Audio '{path}' was not preloaded.", path);
        }

        private static void Require(int loaded, string path)
        {
            if (loaded == 0)
            {
                throw new InvalidOperationException($"Could not load audio '{path}'. Run tools/build_web_content.py first.");
            }
        }
    }
}
