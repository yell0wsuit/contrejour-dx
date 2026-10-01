import io
from pathlib import Path

from PIL import Image

from webcontent import images


def _lossless(data: bytes):
    """A lossless encoder that records whether it was ever asked to run."""
    calls = []

    def encode() -> bytes:
        calls.append(1)
        return data

    encode.calls = calls
    return encode


def test_lossy_wins_when_comfortably_smaller():
    chosen, kind = images.pick_encoding(1000, b"x" * 100, _lossless(b"y" * 900))
    assert kind == "lossy"
    assert chosen == b"x" * 100


def test_lossless_wins_when_lossy_exceeds_ratio_and_lossless_is_smaller():
    chosen, kind = images.pick_encoding(1000, b"x" * 900, _lossless(b"y" * 500))
    assert kind == "lossless"
    assert chosen == b"y" * 500


def test_lossy_kept_when_over_ratio_but_still_smaller_than_lossless():
    chosen, kind = images.pick_encoding(1000, b"x" * 700, _lossless(b"y" * 950))
    assert kind == "lossy"


def test_boundary_at_exactly_the_ratio_keeps_lossy():
    chosen, kind = images.pick_encoding(1000, b"x" * 600, _lossless(b"y" * 100))
    assert kind == "lossy"


def test_lossless_is_not_encoded_when_lossy_already_wins():
    """The whole point of the callable: the expensive encode never runs."""
    encode = _lossless(b"y" * 100)
    images.pick_encoding(1000, b"x" * 100, encode)
    assert encode.calls == []

    images.pick_encoding(1000, b"x" * 900, encode)
    assert encode.calls == [1]


def _write_png(path: Path, size, color) -> None:
    Image.new("RGBA", size, color).save(path, "PNG")


def test_encode_webp_round_trips_dimensions(tmp_path):
    src = tmp_path / "a.png"
    _write_png(src, (64, 48), (10, 200, 30, 255))
    data, _ = images.encode_webp(src)
    decoded = Image.open(io.BytesIO(data))
    assert decoded.size == (64, 48)
    assert decoded.format == "WEBP"


def test_encode_webp_preserves_alpha_exactly(tmp_path):
    src = tmp_path / "a.png"
    Image.new("RGBA", (32, 32), (255, 0, 0, 0)).save(src, "PNG")
    data, _ = images.encode_webp(src)
    decoded = Image.open(io.BytesIO(data)).convert("RGBA")
    assert decoded.getchannel("A").getextrema() == (0, 0)


def test_convert_images_writes_webp_and_skips_second_run(tmp_path):
    content = tmp_path / "GameContents"
    (content / "Content" / "Graphics" / "sub").mkdir(parents=True)
    _write_png(content / "Content" / "Graphics" / "a.png", (16, 16), (1, 2, 3, 255))
    _write_png(content / "Content" / "Graphics" / "sub" / "b.png", (16, 16), (4, 5, 6, 255))
    out = tmp_path / "out"
    entries: dict[str, str] = {}

    converted, skipped = images.convert_images(content, out, entries)
    assert (converted, skipped) == (2, 0)
    assert (out / "Assets" / "Content" / "Graphics" / "a.webp").exists()
    assert (out / "Assets" / "Content" / "Graphics" / "sub" / "b.webp").exists()

    converted, skipped = images.convert_images(content, out, entries)
    assert (converted, skipped) == (0, 2)


def _png(path, size=(4, 4), color=(255, 0, 0, 128)):
    path.parent.mkdir(parents=True, exist_ok=True)
    Image.new("RGBA", size, color).save(path)


def test_jobs_cover_only_graphics_and_keep_scale_suffixes(tmp_path):
    source = tmp_path / "GameContents"
    _png(source / "Content/Graphics/menu/menu_x2.png")
    _png(source / "Content/Graphics/textures/tail.x0.5.png")
    _png(source / "30X30.scale-100.png")
    _png(source / "Win8/logo.png")

    outs = sorted(job.out_rel for job in images._jobs(source, tmp_path / "out"))

    assert outs == [
        "Assets/Content/Graphics/menu/menu_x2.webp",
        "Assets/Content/Graphics/textures/tail.x0.5.webp",
    ]
