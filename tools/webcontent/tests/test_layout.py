from pathlib import PurePosixPath

from webcontent import layout


def test_content_moves_under_assets():
    assert layout.output_relative(PurePosixPath("Content/Graphics/menu/menu.png")) == PurePosixPath(
        "Assets/Content/Graphics/menu/menu.png"
    )


def test_resources_keep_their_place():
    assert layout.output_relative(PurePosixPath("Resources/values.ru.xml")) == PurePosixPath("Resources/values.ru.xml")


def test_win8_tiles_are_not_shipped():
    assert layout.output_relative(PurePosixPath("30X30.scale-100.png")) is None
    assert layout.output_relative(PurePosixPath("Win8/150X150.scale-100.png")) is None
