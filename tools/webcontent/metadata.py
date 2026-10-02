"""The metadata bundle: every XML and JSON file the game reads, in one request.

~750 small files (level XML for both aspect ratios, atlases, views, animations, fonts.json, the
values files) would otherwise be ~750 fetches before Play. The text is kept verbatim - no
reserializing, which would rewrite the level files' namespace prefixes - and the server's gzip does
the shrinking (the level XML compresses ~25x).
"""

from __future__ import annotations

import json
from pathlib import Path

from . import layout

EXCLUDED_PREFIX = "Content/fonts/licenses/"


def _sources(content_root: Path) -> list[Path]:
    found = []
    for top in ("Content", "Resources"):
        for pattern in ("*.xml", "*.json"):
            found += (content_root / top).rglob(pattern)
    return sorted(found)


def build_bundle(content_root: Path) -> dict[str, str]:
    bundle: dict[str, str] = {}
    for source in _sources(content_root):
        relative = layout.relative_to(source, content_root)
        if relative.as_posix().startswith(EXCLUDED_PREFIX):
            continue
        # utf-8, not utf-8-sig: a BOM survives as U+FEFF and re-encodes to the same bytes.
        bundle[layout.output_relative(relative).as_posix()] = source.read_bytes().decode("utf-8")
    return bundle


def write_bundle(bundle: dict[str, str], out_path: Path) -> int:
    out_path.parent.mkdir(parents=True, exist_ok=True)
    out_path.write_text(
        json.dumps(bundle, separators=(",", ":"), ensure_ascii=False, sort_keys=True), encoding="utf-8"
    )
    return out_path.stat().st_size
