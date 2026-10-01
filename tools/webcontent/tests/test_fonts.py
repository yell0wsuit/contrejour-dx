import json
from io import BytesIO
from pathlib import Path

from fontTools.ttLib import TTFont

from webcontent import fonts

REPO = Path(__file__).resolve().parents[3]


def _write(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def _tree(tmp_path: Path) -> Path:
    source = tmp_path / "GameContents"
    _write(
        source / "Content/fonts/fonts.json",
        json.dumps({"default": {"file": "Latin.ttf"}, "ru": {"file": "Cyr.ttf"}, "uk": {"file": "Cyr.ttf"}}),
    )
    _write(source / "Resources/values.xml", '<resources><string name="a">Play</string></resources>')
    _write(source / "Resources/values.de.xml", '<resources><string name="a">Spielen ü</string></resources>')
    _write(source / "Resources/values.ru.xml", '<resources><string name="a" title="Ж">Играть</string></resources>')
    _write(source / "Resources/values.uk.xml", '<resources><string name="a">Грати ї</string></resources>')
    return source


def test_each_face_gets_the_locales_that_use_it(tmp_path):
    source = _tree(tmp_path)

    faces = fonts.locales_by_face(source)

    # Sorted by file name: values.de.xml < values.ru.xml < values.uk.xml < values.xml ("" is the default locale).
    assert faces == {"Latin.ttf": ["de", ""], "Cyr.ttf": ["ru", "uk"]}


def test_charset_covers_text_attributes_and_ascii(tmp_path):
    source = _tree(tmp_path)

    charset = fonts.collect_charset(source / "Resources", ["ru", "uk"])

    assert {"И", "ї", "Ж", "A", "~", " "} <= charset
    assert "ü" in charset  # Latin-1 is always kept for generated text
    assert "…" in charset  # as is general punctuation


def test_subset_keeps_only_requested_glyphs():
    source = REPO / "GameContents/Content/fonts/PatrickHand-Regular.ttf"

    data = fonts.subset_font(source, {"A", "B"})

    cmap = TTFont(BytesIO(data)).getBestCmap()
    assert set(cmap) == {ord("A"), ord("B")}
