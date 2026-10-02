"""WAV effects and FLAC songs to Ogg Opus.

Songs keep stereo at 192 kbps. Effects are short, mono originals (22.05 kHz iOS sounds), so 96 kbps
mono is transparent for them; libopus works at 48 kHz, so asking for it keeps one resample out of
the chain. The browser streams songs through an <audio> element and decodes effects up front.
"""

from __future__ import annotations

import functools
import subprocess
from collections.abc import Iterator
from pathlib import Path

from . import layout, pipeline, progress

SFX_SETTINGS = "ogg:opus:96k:mono:48000hz"
SONG_SETTINGS = "ogg:opus:192k:stereo"
REQUIRED_ENCODERS = ("libopus",)


def ogg_command(ffmpeg: Path, source: Path, dest: Path, song: bool) -> list[str]:
    """Builds the ffmpeg argv for one conversion. `-b:a`, because libopus takes no `-q:a` bitrate.

    `-vn`: the album FLACs embed their cover art as a video stream, which ffmpeg would otherwise try to
    encode into the Ogg.
    """
    command = [str(ffmpeg), "-y", "-v", "error", "-i", str(source), "-vn"]
    if not song:
        command += ["-ac", "1", "-ar", "48000"]
    command += ["-c:a", "libopus", "-b:a", "192k" if song else "96k", str(dest)]
    return command


def write_ogg(job: pipeline.Job, ffmpeg: Path) -> None:
    """Converts one job. Runs in a pool thread; ffmpeg is its own process."""
    subprocess.run(
        ogg_command(ffmpeg, job.source, job.out_path, song=job.settings == SONG_SETTINGS), check=True
    )


def _jobs(content_root: Path, out_root: Path) -> Iterator[pipeline.Job]:
    music = content_root / "Content" / "Music"
    sources = sorted([*music.glob("*.wav"), *music.glob("*.flac")])
    seen: dict[str, Path] = {}
    jobs = []
    for source in sources:
        relative = layout.output_relative(layout.relative_to(source, content_root)).with_suffix(".ogg")
        out_rel = relative.as_posix()
        if out_rel in seen:
            raise ValueError(f"{seen[out_rel].name} and {source.name} would both become {out_rel}")
        seen[out_rel] = source
        settings = SONG_SETTINGS if source.suffix == ".flac" else SFX_SETTINGS
        jobs.append(pipeline.Job(source, out_rel, out_root / relative, settings))
    yield from jobs


def convert_audio(
    content_root: Path,
    out_root: Path,
    entries: dict[str, str],
    ffmpeg: Path,
    report: progress.Reporter = progress.SILENT,
) -> tuple[int, int]:
    """Converts every WAV and FLAC under Content/Music, skipping unchanged outputs."""
    return pipeline.run_stage(
        "audio",
        _jobs(content_root, out_root),
        functools.partial(write_ogg, ffmpeg=ffmpeg),
        entries,
        report,
        cpu_bound=False,
    )
