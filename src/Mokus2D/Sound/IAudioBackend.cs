using System;

namespace Mokus2D.Sound
{
    // A loaded sound effect. Opaque: only the backend that loaded it can play it, and it frees it.
    public interface ISoundEffect
    {
    }

    // A loaded song, streamed while it plays. Opaque, like ISoundEffect.
    public interface ISong
    {
    }

    // The platform half of audio. SoundManager keeps the caching and the music/enable logic; this is
    // only what needs a platform audio API. Paths are absolute, with extension.
    public interface IAudioBackend : IDisposable
    {
        // Decodes the whole file. Throws FileNotFoundException for a missing file and
        // InvalidDataException for one that cannot be decoded.
        ISoundEffect LoadSound(string path);

        // Prepares the file for streaming. Throws as LoadSound does.
        ISong LoadSong(string path);

        // Fire-and-forget; any number of effects may overlap. Volume is 0-1.
        void PlaySound(ISoundEffect sound, float volume);

        void StopAllSounds();

        // Silences every effect, playing and future, without stopping them.
        bool SoundsMuted { get; set; }

        // There is one music voice: this stops whatever it plays and starts the song from the
        // beginning, unpaused, looping forever.
        void PlaySong(ISong song);

        // Fades the music voice to silence over the given time, then stops it.
        void FadeOutSong(float seconds);

        // Pauses or resumes the music voice; does nothing when it is not playing.
        bool SongPaused { get; set; }
    }
}
