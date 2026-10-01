"""assets.json: what the browser host preloads, by kind.

Images and fonts go into the host's file loader; sounds are decoded by WebAudio; songs are kept as
bytes and streamed through an <audio> element. Songs and effects are both .ogg, so the encoder settings
recorded in the build manifest tell them apart.
"""

from __future__ import annotations

import json
from pathlib import Path

from . import audio


def build_catalog(entries: dict[str, str], out_root: Path) -> dict[str, list[str]]:
    groups: dict[str, list[str]] = {"images": [], "fonts": [], "sounds": [], "songs": []}
    for relative, stamp in sorted(entries.items()):
        if not (out_root / relative).is_file():
            continue
        settings = stamp.split("|", 1)[1] if "|" in stamp else ""
        if relative.endswith(".webp"):
            groups["images"].append(relative)
        elif relative.endswith(".ttf"):
            groups["fonts"].append(relative)
        elif settings == audio.SONG_SETTINGS:
            groups["songs"].append(relative)
        elif settings == audio.SFX_SETTINGS:
            groups["sounds"].append(relative)
    return groups


def write_catalog(path: Path, groups: dict[str, list[str]]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(groups, indent=2, sort_keys=True), encoding="utf-8")
