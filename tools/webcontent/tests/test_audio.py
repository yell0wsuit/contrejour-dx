from pathlib import Path

import pytest

from webcontent import audio


def _touch(path: Path) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(b"x")


def test_effects_and_songs_map_to_ogg_with_their_settings(tmp_path):
    source = tmp_path / "GameContents"
    _touch(source / "Content/Music/leapOn1.wav")
    _touch(source / "Content/Music/chapter1.flac")

    jobs = {job.out_rel: job.settings for job in audio._jobs(source, tmp_path / "out")}

    assert jobs == {
        "Assets/Content/Music/chapter1.ogg": audio.SONG_SETTINGS,
        "Assets/Content/Music/leapOn1.ogg": audio.SFX_SETTINGS,
    }


def test_two_sources_for_one_output_are_refused(tmp_path):
    source = tmp_path / "GameContents"
    _touch(source / "Content/Music/menu.wav")
    _touch(source / "Content/Music/menu.flac")

    with pytest.raises(ValueError, match="menu.ogg"):
        list(audio._jobs(source, tmp_path / "out"))


def test_effect_command_is_mono_48k_opus():
    command = audio.ogg_command(Path("ffmpeg"), Path("a.wav"), Path("a.ogg"), song=False)
    assert command == [
        "ffmpeg", "-y", "-v", "error", "-i", "a.wav", "-vn",
        "-ac", "1", "-ar", "48000", "-c:a", "libopus", "-b:a", "96k", "a.ogg",
    ]


def test_song_command_keeps_stereo_at_192k():
    command = audio.ogg_command(Path("ffmpeg"), Path("s.flac"), Path("s.ogg"), song=True)
    assert "-ac" not in command
    assert command[-4:] == ["libopus", "-b:a", "192k", "s.ogg"]


def test_cover_art_is_dropped():
    # The album FLACs embed their cover as a video stream, which Ogg cannot carry.
    assert "-vn" in audio.ogg_command(Path("ffmpeg"), Path("s.flac"), Path("s.ogg"), song=True)
