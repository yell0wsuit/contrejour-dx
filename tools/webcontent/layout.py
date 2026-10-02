"""Where each GameContents file lands in the browser payload.

The payload keeps the desktop install's layout, so the engine opens the same relative paths on both
hosts and only the extensions differ: GameContents/Content/** -> Assets/Content/**, and
GameContents/Resources/** -> Resources/**. Nothing else in GameContents (the Win8 tiles and logos) is
ever opened by the game.
"""

from __future__ import annotations

from pathlib import Path, PurePosixPath


def output_relative(source_relative: PurePosixPath) -> PurePosixPath | None:
    """Maps a GameContents-relative path to its payload path, or None for files the game never opens."""
    parts = source_relative.parts
    if not parts:
        return None
    if parts[0] == "Content":
        return PurePosixPath("Assets", *parts)
    if parts[0] == "Resources":
        return source_relative
    return None


def relative_to(source: Path, root: Path) -> PurePosixPath:
    return PurePosixPath(source.relative_to(root).as_posix())
