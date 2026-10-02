"""Font subsetting to the characters the game can render with each face.

Content/fonts/fonts.json picks one face per locale ("default" for every locale without its own
entry; values.xml is the default locale). Each face is subset to the text of the values files of the
locales that use it, plus printable ASCII, Latin-1 and general punctuation, which cover numbers,
scores and other text built at runtime. Output stays sfnt .ttf under the same name: SkiaSharp's
WebAssembly FreeType has no Brotli, so it cannot read WOFF2.
"""

from __future__ import annotations

import functools
import hashlib
import io
import json
import logging
from collections.abc import Iterator, Sequence
from pathlib import Path
from xml.etree import ElementTree

from fontTools.subset import Options, Subsetter
from fontTools.ttLib import TTFont

from . import layout, pipeline, progress

SETTINGS = "sfnt:subset:v1"

# fontTools warns once per table it has no subsetter for; dropping those tables is intended.
logging.getLogger("fontTools.subset").setLevel(logging.ERROR)

ALWAYS = (
    {chr(code) for code in range(0x20, 0x7F)}
    | {chr(code) for code in range(0xA0, 0x100)}
    | {chr(code) for code in range(0x2010, 0x2027)}
)


def _locales(resources: Path) -> list[str]:
    """Every locale with a values file: "" for values.xml, then each values.<code>.xml."""
    codes = []
    for path in sorted(resources.glob("values*.xml")):
        parts = path.name.split(".")
        codes.append(parts[1] if len(parts) == 3 else "")
    return codes


def locales_by_face(content_root: Path) -> dict[str, list[str]]:
    config = json.loads((content_root / "Content/fonts/fonts.json").read_text(encoding="utf-8"))
    config = {key.lower(): value for key, value in config.items()}
    faces: dict[str, list[str]] = {}
    for locale in _locales(content_root / "Resources"):
        entry = config.get(locale) or config["default"]
        faces.setdefault(entry["file"], []).append(locale)
    return faces


def collect_charset(resources: Path, locales: Sequence[str]) -> set[str]:
    charset = set(ALWAYS)
    for locale in locales:
        path = resources / (f"values.{locale}.xml" if locale else "values.xml")
        for element in ElementTree.parse(path).iter():
            charset.update(element.text or "")
            for value in element.attrib.values():
                charset.update(value)
    charset.discard("\n")
    charset.discard("\r")
    charset.discard("\t")
    return charset


def subset_font(source: Path, charset: set[str]) -> bytes:
    """Subsets a typeface to `charset` and returns it as TTF."""
    font = TTFont(source)
    options = Options()
    options.layout_features = ["*"]
    options.notdef_outline = True
    subsetter = Subsetter(options=options)
    subsetter.populate(text="".join(sorted(charset)))
    subsetter.subset(font)

    buffer = io.BytesIO()
    font.save(buffer)
    return buffer.getvalue()


def _settings_for_charset(charset: set[str]) -> str:
    digest = hashlib.sha256("".join(sorted(charset)).encode("utf-8")).hexdigest()
    return f"{SETTINGS}:{len(charset)}:{digest}"


def write_subset(job: pipeline.Job, resources: Path, locales: dict[str, list[str]]) -> None:
    """Subsets one job's typeface. Runs in a pool worker."""
    charset = collect_charset(resources, locales[job.source.name])
    job.out_path.write_bytes(subset_font(job.source, charset))


def _jobs(content_root: Path, out_root: Path, faces: dict[str, list[str]]) -> Iterator[pipeline.Job]:
    for face, locales in sorted(faces.items()):
        source = content_root / "Content/fonts" / face
        relative = layout.output_relative(layout.relative_to(source, content_root))
        charset = collect_charset(content_root / "Resources", locales)
        yield pipeline.Job(source, relative.as_posix(), out_root / relative, _settings_for_charset(charset))


def convert_fonts(
    content_root: Path,
    out_root: Path,
    entries: dict[str, str],
    report: progress.Reporter = progress.SILENT,
) -> tuple[int, int]:
    """Subsets every face fonts.json uses, skipping unchanged outputs."""
    faces = locales_by_face(content_root)
    return pipeline.run_stage(
        "fonts",
        _jobs(content_root, out_root, faces),
        functools.partial(write_subset, resources=content_root / "Resources", locales=faces),
        entries,
        report,
    )
