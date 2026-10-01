import json
from pathlib import Path

from webcontent import metadata


def _write(path: Path, data: bytes) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(data)


def test_bundle_is_verbatim_and_keyed_by_output_path(tmp_path):
    source = tmp_path / "GameContents"
    level = b'\xef\xbb\xbf<?xml version="1.0"?>\n<level xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"> </level>\n'
    _write(source / "Content/Levels/16X9/level0iPhone.xml", level)
    _write(source / "Content/Graphics/menu/menu.json", '{"meta":{"image":"menu.png"},"t":"日本"}'.encode())
    _write(source / "Resources/values.ja.xml", "<r>日本語</r>".encode())
    _write(source / "Content/fonts/licenses/PatrickHand-OFL.txt", b"license")
    _write(source / "Content/fonts/licenses/notes.json", b"{}")
    _write(source / "Content/Graphics/menu/menu.png", b"png")

    bundle = metadata.build_bundle(source)

    assert sorted(bundle) == [
        "Assets/Content/Graphics/menu/menu.json",
        "Assets/Content/Levels/16X9/level0iPhone.xml",
        "Resources/values.ja.xml",
    ]
    assert bundle["Assets/Content/Levels/16X9/level0iPhone.xml"].encode("utf-8") == level


def test_written_bundle_round_trips(tmp_path):
    bundle = {"Resources/values.xml": "﻿<r>é</r>"}
    out = tmp_path / "metadata.json"

    metadata.write_bundle(bundle, out)

    assert json.loads(out.read_text(encoding="utf-8")) == bundle
