"""Exercise bundle assembly with a tiny publish payload and stand-in native tools."""
import os
import shutil
import subprocess
from pathlib import Path

import pytest

ROOT = Path(__file__).resolve().parents[2]


def test_bundle_preserves_content_in_resources_and_excludes_assembly_docs(tmp_path):
    if shutil.which("rsync") is None:
        pytest.skip("rsync is required by the macOS bundler")
    repo = tmp_path / "repo"
    distribution = repo / "distribution"
    shutil.copytree(ROOT / "distribution" / "templates", distribution / "templates")
    shutil.copy2(ROOT / "distribution" / "bundle_macos.sh", distribution / "bundle_macos.sh")
    tools = tmp_path / "tools"
    tools.mkdir()
    dotnet = tools / "dotnet"
    dotnet.write_text('''#!/usr/bin/env python3
import sys
from pathlib import Path
out = Path(sys.argv[sys.argv.index('-o') + 1])
for name, data in {
    'ContreJour.Desktop': b'game',
    'ContreJour.Desktop.xml': b'assembly docs',
    'ContreJour.Desktop.pdb': b'symbols',
    'ContreJour.Desktop.dSYM/Contents/Resources/DWARF/ContreJour.Desktop': b'native symbols',
    'Assets/Content/levels.xml': b'level data',
    'Resources/en.xml': b'localization',
    'Resources/ContreJourDXIcon.icns': b'icon',
}.items():
    dest = out / name
    dest.parent.mkdir(parents=True, exist_ok=True)
    dest.write_bytes(data)
''')
    dotnet.chmod(0o755)
    for name in ("codesign", "xattr", "hdiutil"):
        tool = tools / name
        tool.write_text("#!/bin/sh\nexit 0\n")
        tool.chmod(0o755)
    subprocess.run(
        ["sh", str(distribution / "bundle_macos.sh"), "1.0.0-prerelease+7"],
        env={**os.environ, "PATH": f"{tools}{os.pathsep}{os.environ['PATH']}", "USE_AOT": "false"},
        check=True, capture_output=True, text=True,
    )
    bundle = repo / "src/ContreJour.Desktop/bin/Publish/osx-arm64/ContreJour.Desktop.app"
    binaries = bundle / "Contents/MacOS"
    assert (binaries / "Assets").is_symlink()
    assert (binaries / "Resources").is_symlink()
    assert (binaries / "Assets/Content/levels.xml").read_bytes() == b"level data"
    assert (binaries / "Resources/en.xml").read_bytes() == b"localization"
    assert not (binaries / "ContreJour.Desktop.xml").exists()
    assert not (binaries / "ContreJour.Desktop.pdb").exists()
    assert not (binaries / "ContreJour.Desktop.dSYM").exists()
