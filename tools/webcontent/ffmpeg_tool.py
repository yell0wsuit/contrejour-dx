"""Finds an ffmpeg that can encode what the pipeline needs.

`CJ_FFMPEG` names an exact binary; otherwise the one on PATH is used. Either way it is checked for the
encoders the pipeline asks for before anything is converted: an ffmpeg missing one is common, and using
it would produce content that is broken in a way nothing downstream notices.
"""

from __future__ import annotations

import os
import shutil
import subprocess
from collections.abc import Iterable
from pathlib import Path

BINARY_ENV = "CJ_FFMPEG"


class FfmpegNotFoundError(Exception):
    """No ffmpeg could be located."""


class MissingEncoderError(Exception):
    """The located ffmpeg lacks an encoder this pipeline requires."""


def find_ffmpeg(environ: dict[str, str] | None = None) -> Path:
    environ = os.environ if environ is None else environ
    pinned = environ.get(BINARY_ENV)
    if pinned:
        path = Path(pinned)
        if not path.is_file():
            raise FfmpegNotFoundError(f"{BINARY_ENV} points at {pinned}, which does not exist")
        return path
    found = shutil.which("ffmpeg")
    if found is None:
        raise FfmpegNotFoundError(f"no ffmpeg on PATH; install one with libopus or set {BINARY_ENV}")
    return Path(found)


def available_encoders(ffmpeg: Path) -> set[str]:
    output = subprocess.run(
        [str(ffmpeg), "-hide_banner", "-encoders"], capture_output=True, text=True, check=True
    ).stdout
    names = set()
    for line in output.splitlines():
        fields = line.split()
        # Encoder rows start with a six-letter capability column such as "A....D".
        if len(fields) >= 2 and len(fields[0]) == 6 and fields[0][0] in "VAS":
            names.add(fields[1])
    return names


def require_encoders(ffmpeg: Path, required: Iterable[str]) -> None:
    missing = sorted(set(required) - available_encoders(ffmpeg))
    if missing:
        raise MissingEncoderError(f"{ffmpeg} lacks encoder(s): {', '.join(missing)}")
