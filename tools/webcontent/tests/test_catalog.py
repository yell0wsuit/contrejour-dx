import json

from webcontent import audio, catalog


def test_groups_by_kind_and_skips_missing_outputs(tmp_path):
    for rel in [
        "Assets/Content/Graphics/menu/menu.webp",
        "Assets/Content/fonts/Neucha.ttf",
        "Assets/Content/Music/leapOn1.ogg",
        "Assets/Content/Music/menu.ogg",
        "Assets/Content/fonts/licenses/Neucha-OFL.txt",
    ]:
        (tmp_path / rel).parent.mkdir(parents=True, exist_ok=True)
        (tmp_path / rel).write_bytes(b"x")
    entries = {
        "Assets/Content/Graphics/menu/menu.webp": "h|webp",
        "Assets/Content/Graphics/gone.webp": "h|webp",
        "Assets/Content/fonts/Neucha.ttf": "h|sfnt",
        "Assets/Content/Music/leapOn1.ogg": "h|" + audio.SFX_SETTINGS,
        "Assets/Content/Music/menu.ogg": "h|" + audio.SONG_SETTINGS,
        "Assets/Content/fonts/licenses/Neucha-OFL.txt": "h|copy",
    }

    groups = catalog.build_catalog(entries, tmp_path)

    assert groups == {
        "images": ["Assets/Content/Graphics/menu/menu.webp"],
        "fonts": ["Assets/Content/fonts/Neucha.ttf"],
        "sounds": ["Assets/Content/Music/leapOn1.ogg"],
        "songs": ["Assets/Content/Music/menu.ogg"],
    }
    catalog.write_catalog(tmp_path / "assets.json", groups)
    assert json.loads((tmp_path / "assets.json").read_text()) == groups
