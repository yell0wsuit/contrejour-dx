from pathlib import Path

import publish_browser


def test_defaults():
    args = publish_browser.parse_args([])
    assert args.output == publish_browser.REPO_ROOT / "dist" / "browser"
    assert args.aot is True
    assert args.serve is None


def test_serve_without_port_uses_default():
    assert publish_browser.parse_args(["--serve"]).serve == publish_browser.DEFAULT_PORT
    assert publish_browser.parse_args(["--serve", "9000"]).serve == 9000


def test_publish_command_turns_aot_on_only_when_asked():
    out = Path("/tmp/out")
    aot = publish_browser.publish_command(out, aot=True)
    plain = publish_browser.publish_command(out, aot=False)
    assert aot[:2] == ["dotnet", "publish"]
    assert "-p:RunAOTCompilation=true" in aot
    assert "-p:RunAOTCompilation=true" not in plain
    assert aot[aot.index("-o") + 1] == str(out)
    assert aot[aot.index("-c") + 1] == "Release"


def test_server_knows_the_wasm_and_content_types():
    types = publish_browser.Handler.extensions_map
    assert types[".wasm"] == "application/wasm"
    assert types[".webp"] == "image/webp"
    assert types[".ogg"] == "audio/ogg"
    assert types[".js"] == "text/javascript"


def test_missing_content_stops_before_dotnet(tmp_path, monkeypatch, capsys):
    calls = []
    monkeypatch.setattr(publish_browser, "CONTENT_CATALOG", tmp_path / "assets.json")
    monkeypatch.setattr(publish_browser.subprocess, "run", lambda *a, **k: calls.append(a))

    assert publish_browser.main([]) == 1
    assert calls == []
    assert "build_web_content.py" in capsys.readouterr().err
