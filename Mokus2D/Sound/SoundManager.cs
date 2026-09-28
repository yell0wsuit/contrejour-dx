using System;
using System.Collections.Generic;

namespace Mokus2D.Sound;

// Silent stand-in for the original FMOD Ex (fmodexWSA81) backed implementation.
// Keeps the public surface and enabled/control state so callers behave the same;
// no audio is loaded or played until a desktop audio backend is wired in.
public static class SoundManager
{
    public static string MusicPath = "";

    public static float SongChangePause = 1f;

    private static bool musicEnabled = true;

    private static bool soundEnabled = true;

    private static bool loop = true;

    private static bool _hasControl;

    public static bool SoundEnabled
    {
        get => soundEnabled;
        set => soundEnabled = value;
    }

    public static bool MusicEnabled
    {
        get => musicEnabled;
        set => musicEnabled = value;
    }

    public static bool Loop
    {
        get => loop;
        set => loop = value;
    }

    public static bool HasControl
    {
        get => _hasControl;
        set => _hasControl = value;
    }

    public static event Action MusicDisableEvent;

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
    }

    public static void PreloadSounds(string[] paths)
    {
    }

    public static void PreloadSong(string path)
    {
    }

    public static void PreloadSound(string path)
    {
    }

    public static void StopAllSounds()
    {
    }

    public static void StopMusic()
    {
    }

    public static void RemoveCurrentMusic()
    {
    }

    public static void PlayRandomSound(string[] files, float volume = 1f)
    {
    }

    public static void PlayRandomSound(List<string> files, float volume = 1f)
    {
    }

    public static void PlaySound(string path, float volume = 1f, float pitch = 0f, float pan = 0f)
    {
    }

    public static void PlayMusic(string path)
    {
    }
}
