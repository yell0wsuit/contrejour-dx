using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using Mokus2D.Sound;

using SDL3;

namespace ContreJour.Desktop.Platform.Audio
{
    // SDL_mixer implementation of the engine's audio backend (adapted from cuttherope-dx). Effects are
    // decoded once and played on a pool of reused voices; music streams from disk on one voice.
    // No native callbacks are installed, so nothing has to be kept alive for the mixer's thread.
    public sealed class SdlAudioBackend : IAudioBackend
    {
        // Every song ships at this rate and every effect is converted to it at load, so no voice
        // resamples while playing; SDL converts the mixed output to the device's rate in one stream.
        private const int MixerFrequency = 44100;

        private const int MixerChannels = 2;

        // SDL_mixer's built-in FLAC decoder (dr_flac).
        private const string FlacDecoder = "DRFLAC";

        private sealed class Clip(nint audio) : ISoundEffect, ISong
        {
            public nint Audio { get; } = audio;
        }

        private sealed class Voice(nint track)
        {
            public nint Track { get; } = track;

            // The volume the game asked for, restored when sounds are unmuted.
            public float Volume { get; set; }
        }

        private readonly nint _mixer;

        private readonly List<nint> _loadedAudio = [];

        private readonly List<Voice> _voices = [];

        private nint _musicTrack;

        private uint _musicOptions;

        private bool _disposed;

        private SdlAudioBackend(nint mixer)
        {
            _mixer = mixer;
        }

        // Opens the default playback device. A machine without one still runs the game silently, so
        // failure is reported through error rather than thrown.
        public static SdlAudioBackend TryOpen(out string error)
        {
            bool audioStarted = false;
            try
            {
                return Open(ref audioStarted, out error);
            }
            catch (Exception e) when (e is DllNotFoundException or EntryPointNotFoundException)
            {
                // Native libraries that fail to load are one more way for audio to be unavailable. The
                // loader's message lists every path it tried; its first line names the library.
                error = $"could not load the SDL audio libraries: {e.Message.Split('\n')[0]}";
                if (audioStarted)
                {
                    SDL.QuitSubSystem(SDL.InitFlags.Audio);
                }
                return null;
            }
        }

        private static SdlAudioBackend Open(ref bool audioStarted, out string error)
        {
            // Each failure reads SDL's error before cleanup, which can replace it.
            if (!SDL.InitSubSystem(SDL.InitFlags.Audio))
            {
                error = $"could not start SDL audio: {SDL.GetError()}";
                return null;
            }
            audioStarted = true;

            // The published SDL3_mixer library records no runtime search path, so its SDL3 dependency
            // resolves only against the SDL3 image InitSubSystem has already loaded.
            if (!Mixer.Init())
            {
                error = $"could not start SDL_mixer: {SDL.GetError()}";
                SDL.QuitSubSystem(SDL.InitFlags.Audio);
                return null;
            }

            nint mixer = CreateDeviceMixer();
            if (mixer == 0)
            {
                error = $"could not open the default playback device: {SDL.GetError()}";
                Mixer.Quit();
                SDL.QuitSubSystem(SDL.InitFlags.Audio);
                return null;
            }

            error = null;
            return new SdlAudioBackend(mixer);
        }

        private static nint CreateDeviceMixer()
        {
            SDL.AudioSpec spec = new()
            {
                Format = SDL.AudioFormat.AudioF32LE,
                Channels = MixerChannels,
                Freq = MixerFrequency
            };
            nint specPointer = Marshal.AllocHGlobal(Marshal.SizeOf<SDL.AudioSpec>());
            try
            {
                Marshal.StructureToPtr(spec, specPointer, false);
                return Mixer.CreateMixerDevice(SDL.AudioDeviceDefaultPlayback, specPointer);
            }
            finally
            {
                Marshal.FreeHGlobal(specPointer);
            }
        }

        public bool SoundsMuted
        {
            get; set
            {
                field = value;
                foreach (Voice voice in _voices)
                {
                    SetVoiceGain(voice);
                }
            }
        }

        public bool SongPaused
        {
            get => _musicTrack != 0 && Mixer.TrackPaused(_musicTrack);
            set
            {
                if (_musicTrack != 0)
                {
                    _ = value ? Mixer.PauseTrack(_musicTrack) : Mixer.ResumeTrack(_musicTrack);
                }
            }
        }

        // Pauses the playback device the mixer feeds, below every voice, so the game's own song pause
        // and mute stay as they were. The desktop host suspends audio while its window is away.
        public bool Suspended
        {
            get;
            set
            {
                if (field == value)
                {
                    return;
                }
                uint device = (uint)SDL.GetNumberProperty(Mixer.GetMixerProperties(_mixer), Mixer.Props.MixerDeviceNumber, 0);
                if (device == 0 || !(value ? SDL.PauseAudioDevice(device) : SDL.ResumeAudioDevice(device)))
                {
                    return;
                }
                field = value;
            }
        }

        public ISoundEffect LoadSound(string path)
        {
            // Effects are short, replayed constantly and often overlap, so they are decoded once.
            return new Clip(LoadEffect(path));
        }

        public ISong LoadSong(string path)
        {
            // Songs are minutes long, so they stream.
            return new Clip(Load(path, predecode: false));
        }

        // Converts an effect that ships at another rate to the mixer's once, here: a voice that
        // resamples while playing mixes its first buffer short by the resampler's lookahead and leaves
        // the rest silent, which pops about 21ms into every effect (fixed the same way in cuttherope-dx).
        private nint LoadEffect(string path)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Audio file not found: {path}", path);
            }
            if (!SDL.LoadWAV(path, out SDL.AudioSpec source, out nint samples, out uint length))
            {
                throw new InvalidDataException($"Could not load audio '{path}': {SDL.GetError()}");
            }
            try
            {
                if (source.Freq == MixerFrequency)
                {
                    return Load(path, predecode: true);
                }
                SDL.AudioSpec target = new()
                {
                    Format = SDL.AudioFormat.AudioS16LE,
                    Channels = source.Channels,
                    Freq = MixerFrequency
                };
                if (!SDL.ConvertAudioSamples(in source, samples, (int)length, in target, out nint converted, out int convertedLength))
                {
                    throw new InvalidDataException($"Could not resample audio '{path}': {SDL.GetError()}");
                }
                try
                {
                    return LoadConverted(path, target, converted, convertedLength);
                }
                finally
                {
                    SDL.Free(converted);
                }
            }
            finally
            {
                SDL.Free(samples);
            }
        }

        private unsafe nint LoadConverted(string path, SDL.AudioSpec spec, nint samples, int length)
        {
            byte[] wav = WavFile.Wrap16Bit(spec.Channels, spec.Freq, new ReadOnlySpan<byte>((void*)samples, length));
            // Predecoding copies the samples out before the load returns, so the buffer only has to
            // stay pinned for the call.
            fixed (byte* data = wav)
            {
                nint stream = SDL.IOFromConstMem((nint)data, (nuint)wav.Length);
                return stream == 0
                    ? throw new InvalidDataException($"Could not open audio '{path}': {SDL.GetError()}")
                    : Load(stream, path, predecode: true);
            }
        }

        private nint Load(string path, bool predecode)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Audio file not found: {path}", path);
            }
            nint stream = SDL.IOFromFile(path, "rb");
            return stream == 0
                ? throw new InvalidDataException($"Could not open audio '{path}': {SDL.GetError()}")
                : Load(stream, path, predecode);
        }

        // Loads from stream, which the mixer closes; path names the file for errors and picks its decoder.
        private nint Load(nint stream, string path, bool predecode)
        {
            uint props = SDL.CreateProperties();
            nint audio;
            try
            {
                // The mixer closes the stream itself whether or not the load succeeds.
                _ = SDL.SetPointerProperty(props, Mixer.Props.AudioLoadIOStreamPointer, stream);
                _ = SDL.SetBooleanProperty(props, Mixer.Props.AudioLoadCloseIOBoolean, true);
                _ = SDL.SetBooleanProperty(props, Mixer.Props.AudioLoadPreDecodeBoolean, predecode);
                _ = SDL.SetPointerProperty(props, Mixer.Props.AudioLoadPreferredMixerPointer, _mixer);
                // Left to choose, the mixer prefers libFLAC, which discards the first 4096 frames of
                // every stream: each song would start about 93ms in, with a click. The built-in
                // decoder plays from the first frame.
                if (path.EndsWith(".flac", StringComparison.OrdinalIgnoreCase))
                {
                    _ = SDL.SetStringProperty(props, Mixer.Props.AudioDecoderString, FlacDecoder);
                }
                audio = Mixer.LoadAudioWithProperties(props);
            }
            finally
            {
                SDL.DestroyProperties(props);
            }
            if (audio == 0)
            {
                throw new InvalidDataException($"Could not load audio '{path}': {SDL.GetError()}");
            }
            _loadedAudio.Add(audio);
            return audio;
        }

        public void PlaySound(ISoundEffect sound, float volume)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            Voice voice = IdleVoice();
            voice.Volume = volume;
            _ = Mixer.SetTrackAudio(voice.Track, ((Clip)sound).Audio);
            SetVoiceGain(voice);
            // No options: play once from the start.
            _ = Mixer.PlayTrack(voice.Track, 0);
        }

        // A voice that has finished is reused; the pool only grows to the most effects heard at once.
        private Voice IdleVoice()
        {
            foreach (Voice voice in _voices)
            {
                if (!Mixer.TrackPlaying(voice.Track) && !Mixer.TrackPaused(voice.Track))
                {
                    return voice;
                }
            }
            nint track = Mixer.CreateTrack(_mixer);
            if (track == 0)
            {
                throw new InvalidOperationException($"Could not create an audio track: {SDL.GetError()}");
            }
            Voice created = new(track);
            _voices.Add(created);
            return created;
        }

        // Muting is per voice rather than an SDL_mixer tag gain, which would overwrite each voice's own
        // volume.
        private void SetVoiceGain(Voice voice)
        {
            _ = Mixer.SetTrackGain(voice.Track, SoundsMuted ? 0f : voice.Volume);
        }

        public void StopAllSounds()
        {
            foreach (Voice voice in _voices)
            {
                _ = Mixer.StopTrack(voice.Track, 0);
            }
        }

        public void PlaySong(ISong song)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_musicTrack == 0)
            {
                _musicTrack = Mixer.CreateTrack(_mixer);
                if (_musicTrack == 0)
                {
                    throw new InvalidOperationException($"Could not create the music track: {SDL.GetError()}");
                }
                // Starting a track takes its loop count from these options; a count set on the track
                // beforehand is discarded.
                _musicOptions = SDL.CreateProperties();
                _ = SDL.SetNumberProperty(_musicOptions, Mixer.Props.PlayLoopsNumber, -1);
            }
            // Stopping first also clears a pause, so the new song always starts playing.
            _ = Mixer.StopTrack(_musicTrack, 0);
            _ = Mixer.SetTrackAudio(_musicTrack, ((Clip)song).Audio);
            _ = Mixer.PlayTrack(_musicTrack, _musicOptions);
        }

        public void FadeOutSong(float seconds)
        {
            if (_musicTrack != 0)
            {
                _ = Mixer.StopTrack(_musicTrack, Mixer.TrackMSToFrames(_musicTrack, (long)(seconds * 1000f)));
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            // Voices read their audio on the mixer's thread, so they go first, then the audio, and only
            // then the mixer that owns both.
            foreach (Voice voice in _voices)
            {
                Mixer.DestroyTrack(voice.Track);
            }
            _voices.Clear();
            if (_musicTrack != 0)
            {
                Mixer.DestroyTrack(_musicTrack);
                _musicTrack = 0;
                SDL.DestroyProperties(_musicOptions);
            }
            foreach (nint audio in _loadedAudio)
            {
                Mixer.DestroyAudio(audio);
            }
            _loadedAudio.Clear();
            Mixer.DestroyMixer(_mixer);
            Mixer.Quit();
            SDL.QuitSubSystem(SDL.InitFlags.Audio);
        }

    }
}
