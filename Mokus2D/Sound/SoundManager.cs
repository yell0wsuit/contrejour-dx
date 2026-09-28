using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Default.Namespace;
using FMOD;
using Mokus2D.Integration.Fmod.Utils;
using Mokus2D.Util.MathUtils;

namespace Mokus2D.Sound;

public static class SoundManager
{
    private const string SoundsGroupName = "sounds";

    private const string MusicExtension = "mp3";

    private const string SoundExtension = "mp3";

    private static Action m_MusicDisableEvent;

    private static readonly Dictionary<string, FmodSound> _songs = new Dictionary<string, FmodSound>();

    private static readonly Dictionary<string, FmodSound> _sounds = new Dictionary<string, FmodSound>();

    public static string MusicPath = "";

    public static float SongChangePause = 1f;

    private static FmodSound _currentMusic;

    private static FmodChannel _currentMusicChannel;

    private static bool musicEnabled = true;

    private static bool paused;

    private static bool soundEnabled = true;

    private static bool loop = true;

    private static bool hasControl;

    private static bool pausing;

    private static bool refreshSong;

    private static readonly FmodChannelGroup SoundsGroup;

    private static bool _hasControl;

    public static bool SoundEnabled
    {
        get
        {
            return soundEnabled;
        }
        set
        {
            soundEnabled = value;
            SoundsGroup.Volume = (value ? 1 : 0);
        }
    }

    public static bool MusicEnabled
    {
        get
        {
            return musicEnabled;
        }
        set
        {
            musicEnabled = value;
            if (HasControl)
            {
                if (value && _currentMusic != null)
                {
                    _currentMusicChannel.Paused = false;
                    refreshSong = false;
                }
                else
                {
                    Pause();
                }
            }
            else if (value)
            {
                GetControlAndPlay();
            }
        }
    }

    public static bool Loop
    {
        get
        {
            return loop;
        }
        set
        {
            loop = value;
        }
    }

    public static bool HasControl
    {
        get
        {
            return _hasControl;
        }
        set
        {
            if (_hasControl != value)
            {
                _hasControl = value;
                if (MusicEnabled && !_hasControl)
                {
                    Pause();
                }
                else if (_hasControl && MusicEnabled && _currentMusicChannel != null)
                {
                    _currentMusicChannel.Paused = false;
                }
            }
        }
    }

    public static event Action MusicDisableEvent
    {
        add
        {
            Action action = SoundManager.m_MusicDisableEvent;
            Action action2;
            do
            {
                action2 = action;
                action = Interlocked.CompareExchange(ref SoundManager.m_MusicDisableEvent, (Action)Delegate.Combine(action2, value), action2);
            }
            while ((object)action != action2);
        }
        remove
        {
            Action action = SoundManager.m_MusicDisableEvent;
            Action action2;
            do
            {
                action2 = action;
                action = Interlocked.CompareExchange(ref SoundManager.m_MusicDisableEvent, (Action)Delegate.Remove(action2, value), action2);
            }
            while ((object)action != action2);
        }
    }

    static SoundManager()
    {
        Loop = true;
        FmodSystem.Initialize();
        SoundsGroup = FmodSystem.GetChannelGroup("sounds");
    }

    public static void Update()
    {
        FmodSystem.Update();
    }

    private static void OnGamePaused(object sender, EventArgs e)
    {
        _sounds.Clear();
    }

    public static void OnResume()
    {
        refreshSong = true;
    }

    private static void OnMediaStateChanged(object sender, EventArgs eventArgs)
    {
    }

    private static void GetControlAndPlay()
    {
        hasControl = true;
        DoPlayCurrentSong();
    }

    public static void OnGameActivated()
    {
        hasControl = false;
    }

    public static void PreloadSongs(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            PreloadSong(paths[i]);
        }
    }

    public static void PreloadSounds(string[] paths)
    {
        for (int i = 0; i < paths.Length; i++)
        {
            PreloadSound(paths[i]);
        }
    }

    public static void PreloadSong(string path)
    {
        if (!_songs.ContainsKey(path))
        {
            FmodSound fmodSound = FmodSystem.CreateSoundStream(Path.ChangeExtension(Path.Combine(new string[2] { MusicPath, path }), "mp3"));
            fmodSound.InfiniteLoop = true;
            _songs.Add(path, fmodSound);
        }
    }

    public static void PreloadSound(string path)
    {
        if (!_sounds.ContainsKey(path))
        {
            FmodSound value = FmodSystem.CreateSound(Path.ChangeExtension(Path.Combine(new string[2] { MusicPath, path }), "mp3"), "sounds");
            _sounds.Add(path, value);
        }
    }

    public static void StopAllSounds()
    {
        SoundsGroup.Stop();
    }

    public static void StopMusic()
    {
        StopScheduledMusic();
        _currentMusic = null;
    }

    public static void RemoveCurrentMusic()
    {
        _currentMusic = null;
    }

    public static void PlayRandomSound(string[] files, float volume = 1f)
    {
        PlaySound(files.RandomItem(), volume);
    }

    public static void PlayRandomSound(List<string> files, float volume = 1f)
    {
        PlaySound(files[Maths.Random(files.Count)], volume);
    }

    public static void PlaySound(string path, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
        PreloadSound(path);
        _sounds[path].Play(paused: false, volume);
    }

    public static void PlayMusic(FmodSound song)
    {
        if (_currentMusic == null)
        {
            _currentMusic = song;
            DoPlayCurrentSong();
        }
        else if (_currentMusic != null && song != _currentMusic)
        {
            _currentMusicChannel.FadeOutAndStop(1f);
            _currentMusic = song;
            StopScheduledMusic();
            Mokus2DGame.Instance.Scheduler.Schedule(DoPlayCurrentSong, SongChangePause);
        }
    }

    private static void StopScheduledMusic()
    {
        Mokus2DGame.Instance.Scheduler.Cancel(DoPlayCurrentSong);
    }

    public static void PlayMusic(string path)
    {
        PreloadSong(path);
        PlayMusic(_songs[path]);
    }

    private static void DoPlayCurrentSong()
    {
        _currentMusicChannel = _currentMusic.Play();
        if (!MusicEnabled)
        {
            Pause();
        }
    }

    private static void Pause()
    {
        if (_currentMusicChannel != null)
        {
            _currentMusicChannel.Paused = true;
        }
    }
}
