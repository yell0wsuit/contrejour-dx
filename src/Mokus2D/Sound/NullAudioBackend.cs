namespace Mokus2D.Sound
{
    // Loads and plays nothing: the regression harness, and machines with no usable audio device.
    public sealed class NullAudioBackend : IAudioBackend
    {
        private sealed class Clip : ISoundEffect, ISong
        {
        }

        private static readonly Clip SilentClip = new();

        public bool SoundsMuted { get; set; }

        public bool SongPaused { get; set; }

        public ISoundEffect LoadSound(string path)
        {
            return SilentClip;
        }

        public ISong LoadSong(string path)
        {
            return SilentClip;
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
}
