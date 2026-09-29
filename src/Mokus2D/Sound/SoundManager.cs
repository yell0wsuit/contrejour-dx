using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Mokus2D.Sound
{
    // The original FMOD Ex (fmodexWSA81) implementation, ported onto IAudioBackend. Its write-only
    // fields and unused private helpers are left out; behaviour is otherwise the original's.
    public static class SoundManager
    {
        private const string Extension = "mp3";

        private static readonly Dictionary<string, ISong> _songs = [];

        private static readonly Dictionary<string, ISoundEffect> _sounds = [];

        private static ISong _currentMusic;

        // Stands in for the original's music channel, which existed once a song had been started.
        private static bool _songStarted;

        public static string MusicPath { get; set; } = "";

        public static readonly float SongChangePause = 1f;

        // Set by ApplicationController from the backend the host supplies.
        internal static IAudioBackend Backend
        {
            get; set
            {
                field = value;
                _sounds.Clear();
                _songs.Clear();
                _currentMusic = null;
                _songStarted = false;
                field.SoundsMuted = !SoundEnabled;
            }
        } = new NullAudioBackend();

        public static bool SoundEnabled
        {
            get; set
            {
                field = value;
                Backend.SoundsMuted = !value;
            }
        } = true;

        public static bool MusicEnabled
        {
            get; set
            {
                field = value;
                if (HasControl)
                {
                    if (value && _currentMusic != null)
                    {
                        if (_songStarted)
                        {
                            Backend.SongPaused = false;
                        }
                    }
                    else
                    {
                        Pause();
                    }
                }
                else if (value)
                {
                    DoPlayCurrentSong();
                }
            }
        } = true;

        public static bool Loop { get; set; } = true;

        public static bool HasControl
        {
            get; set
            {
                if (field != value)
                {
                    field = value;
                    if (MusicEnabled && !field)
                    {
                        Pause();
                    }
                    else if (field && MusicEnabled && _songStarted)
                    {
                        Backend.SongPaused = false;
                    }
                }
            }
        }

        // Subscribed to by the game but never raised, as in the original. Kept as empty accessors: a
        // field-like event nothing raises is warning CS0067.
        public static event Action MusicDisableEvent
        {
            add { }
            remove { }
        }

        // FMOD needed a per-frame update; SDL_mixer does not.
        public static void Update()
        {
        }

        public static void OnResume()
        {
        }

        public static void OnGameActivated()
        {
        }

        public static void PreloadSongs(string[] paths)
        {
            foreach (string path in paths)
            {
                PreloadSong(path);
            }
        }

        public static void PreloadSounds(string[] paths)
        {
            foreach (string path in paths)
            {
                PreloadSound(path);
            }
        }

        public static void PreloadSong(string path)
        {
            if (!_songs.ContainsKey(path))
            {
                _songs.Add(path, Backend.LoadSong(FilePath(path)));
            }
        }

        public static void PreloadSound(string path)
        {
            if (!_sounds.ContainsKey(path))
            {
                _sounds.Add(path, Backend.LoadSound(FilePath(path)));
            }
        }

        public static void StopAllSounds()
        {
            Backend.StopAllSounds();
        }

        // Like the original, this forgets the current song without silencing it; the next PlayMusic
        // replaces it on the music voice.
        public static void StopMusic()
        {
            StopScheduledMusic();
            _currentMusic = null;
        }

        public static void RemoveCurrentMusic()
        {
            _currentMusic = null;
        }

        // Which variant plays has no gameplay effect, so it does not draw from the per-level seeded
        // Maths.RandomGenerator stream.
        public static void PlayRandomSound(string[] files, float volume = 1f)
        {
            PlaySound(files[Random.Shared.Next(files.Length)], volume);
        }

        public static void PlayRandomSound(List<string> files, float volume = 1f)
        {
            PlaySound(files[Random.Shared.Next(files.Count)], volume);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "The original ignored pitch and pan too; callers keep passing them.")]
        public static void PlaySound(string path, float volume = 1f, float pitch = 0f, float pan = 0f)
        {
            PreloadSound(path);
            Backend.PlaySound(_sounds[path], volume);
        }

        public static void PlayMusic(string path)
        {
            PreloadSong(path);
            PlayMusic(_songs[path]);
        }

        private static void PlayMusic(ISong song)
        {
            if (_currentMusic == null)
            {
                _currentMusic = song;
                DoPlayCurrentSong();
            }
            else if (song != _currentMusic)
            {
                Backend.FadeOutSong(1f);
                _currentMusic = song;
                StopScheduledMusic();
                Mokus2DGame.Instance.Scheduler.Schedule(DoPlayCurrentSong, SongChangePause);
            }
        }

        private static void StopScheduledMusic()
        {
            Mokus2DGame.Instance.Scheduler.Cancel(DoPlayCurrentSong);
        }

        private static void DoPlayCurrentSong()
        {
            // The original would have thrown here with no current song.
            if (_currentMusic == null)
            {
                return;
            }
            Backend.PlaySong(_currentMusic);
            _songStarted = true;
            if (!MusicEnabled)
            {
                Pause();
            }
        }

        private static void Pause()
        {
            if (_songStarted)
            {
                Backend.SongPaused = true;
            }
        }

        private static string FilePath(string name)
        {
            return Path.ChangeExtension(Path.Combine(MusicPath, name), Extension);
        }
    }
}
