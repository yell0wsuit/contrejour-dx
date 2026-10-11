#!/usr/bin/env python3
"""Converts GameContents/ into the browser payload.

Images to WebP, effects and songs to Ogg Opus, fonts subset, licenses copied, all XML/JSON bundled
into metadata.json, and assets.json listing what the host preloads. Every stage is incremental, so a
rerun after changing one asset reconverts only that asset.

Usage:
    python3 tools/build_web_content.py
    python3 tools/build_web_content.py --skip-audio   # no ffmpeg; the browser then runs silent
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

from webcontent import (  # noqa: E402
    audio,
    catalog,
    ffmpeg_tool,
    fonts,
    images,
    licenses,
    manifest,
    metadata,
    progress,
)

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_SOURCE = REPO_ROOT / "GameContents"
DEFAULT_OUT = REPO_ROOT / "src" / "ContreJourDX.Browser" / "wwwroot" / "content"
MANIFEST_NAME = ".build-manifest.json"


def _parse_args(argv: list[str] | None) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--source", type=Path, default=DEFAULT_SOURCE)
    parser.add_argument("--out", type=Path, default=DEFAULT_OUT)
    parser.add_argument("--skip-audio", action="store_true", help="Skip audio (no ffmpeg with libopus).")
    parser.add_argument("--no-progress", action="store_true", help="Only print per-stage totals.")
    return parser.parse_args(argv)


def _say(message: str) -> None:
    print(message, flush=True)


def main(argv: list[str] | None = None) -> int:
    args = _parse_args(argv)
    source: Path = args.source
    out: Path = args.out
    if not source.is_dir():
        print(f"error: content source not found: {source}", file=sys.stderr)
        return 1

    manifest_path = out / MANIFEST_NAME
    entries = manifest.load_manifest(manifest_path)
    report = progress.SILENT if args.no_progress else progress.LiveReporter()

    converted, skipped = images.convert_images(source, out, entries, report)
    _say(f"images: {converted} converted, {skipped} skipped")

    if args.skip_audio:
        _say("audio: skipped")
    else:
        try:
            ffmpeg = ffmpeg_tool.find_ffmpeg()
            ffmpeg_tool.require_encoders(ffmpeg, audio.REQUIRED_ENCODERS)
        except (ffmpeg_tool.FfmpegNotFoundError, ffmpeg_tool.MissingEncoderError) as error:
            print(f"error: {error} (or pass --skip-audio)", file=sys.stderr)
            return 2
        converted, skipped = audio.convert_audio(source, out, entries, ffmpeg, report)
        _say(f"audio: {converted} converted, {skipped} skipped")

    converted, skipped = fonts.convert_fonts(source, out, entries, report)
    _say(f"fonts: {converted} converted, {skipped} skipped")

    converted, skipped = licenses.copy_licenses(source, out, entries, report)
    _say(f"licenses: {converted} copied, {skipped} skipped")

    size = metadata.write_bundle(metadata.build_bundle(source), out / "metadata.json")
    _say(f"metadata: {size / 1024 / 1024:.1f} MB")

    manifest.save_manifest(manifest_path, entries)
    catalog.write_catalog(out / "assets.json", catalog.build_catalog(entries, out))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
