using System.Collections.Generic;
using System.IO;

using Mokus2D.Content;
using Mokus2D.Diagnostics;
using Mokus2D.Sound;

using Xunit;

namespace Mokus2D.Tests
{
    public class SoundManagerFileTypeTests
    {
        private sealed class RecordingBackend : IAudioBackend
        {
            private sealed class Clip : ISoundEffect, ISong
            {
            }

            public List<string> Sounds { get; } = [];

            public List<string> Songs { get; } = [];

            public bool SoundsMuted { get; set; }

            public bool SongPaused { get; set; }

            public ISoundEffect LoadSound(string path)
            {
                Sounds.Add(path);
                return new Clip();
            }

            public ISong LoadSong(string path)
            {
                Songs.Add(path);
                return new Clip();
            }

            public void PlaySound(ISoundEffect sound, float volume)
            {
            }

            public void StopAllSounds()
            {
            }

            public void PlaySong(ISong song)
            {
            }

            public void FadeOutSong(float seconds)
            {
            }

            public void Dispose()
            {
            }
        }

        [Fact]
        public void EffectsLoadWavAndSongsLoadFlac()
        {
            using RecordingLoggerFactory factory = new();
            Log.Factory = factory;
            RecordingBackend backend = new();
            string previousPath = SoundManager.MusicPath;
            SoundManager.Backend = backend;
            SoundManager.MusicPath = Path.Combine("root", "Music");
            try
            {
                SoundManager.PreloadSound("leapOn1");
                SoundManager.PreloadSong("chapter1");

                Assert.Equal([Path.Combine("root", "Music", "leapOn1.wav")], backend.Sounds);
                Assert.Equal([Path.Combine("root", "Music", "chapter1.flac")], backend.Songs);
                SoundManager.PreloadSound("leapOn1");
                SoundManager.PreloadSong("chapter1");
                Assert.Equal(2, factory.Entries.Count);
                Assert.All(factory.Entries, entry => Assert.Equal(LogCategories.Audio, entry.Category));
            }
            finally
            {
                Log.Factory = null;
                SoundManager.Backend = new NullAudioBackend();
                SoundManager.MusicPath = previousPath;
            }
        }

        [Fact]
        public void EffectsAndSongsUseTheHostFormats()
        {
            RecordingBackend backend = new();
            string previousPath = SoundManager.MusicPath;
            SoundManager.Backend = backend;
            SoundManager.Formats = new ContentFormats(".webp", ".ogg", ".opus");
            SoundManager.MusicPath = "Assets/Content/Music";
            try
            {
                SoundManager.PreloadSound("leapOn1");
                SoundManager.PreloadSong("chapter1");

                Assert.Equal([Path.Combine("Assets/Content/Music", "leapOn1.ogg")], backend.Sounds);
                Assert.Equal([Path.Combine("Assets/Content/Music", "chapter1.opus")], backend.Songs);
            }
            finally
            {
                SoundManager.Formats = ContentFormats.Desktop;
                SoundManager.Backend = new NullAudioBackend();
                SoundManager.MusicPath = previousPath;
            }
        }
    }
}
