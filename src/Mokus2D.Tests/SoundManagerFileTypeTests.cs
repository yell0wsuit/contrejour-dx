using System.Collections.Generic;
using System.IO;

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
            }
            finally
            {
                SoundManager.Backend = new NullAudioBackend();
                SoundManager.MusicPath = previousPath;
            }
        }
    }
}
