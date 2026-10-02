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


def test_merge_preserves_runtime_hashes_and_versions():
    threaded = {"version": "thread", "assets": [{"url": "index.html", "hash": "shell"}, {"url": "_framework/a.wasm", "hash": "thread-hash"}]}
    single = {"version": "single", "assets": [{"url": "index.html", "hash": "ignored"}, {"url": "_framework/a.wasm", "hash": "single-hash"}]}
    merged, version = publish_browser.merge_manifests(threaded, single)
    assert merged["assets"] == threaded["assets"] + [{"url": "_framework-single/a.wasm", "hash": "single-hash"}]
    assert merged["version"] == version
    assert publish_browser.merge_manifests(threaded, single)[1] == version
    assert publish_browser.merge_manifests(threaded, {**single, "version": "changed"})[1] != version
    assert publish_browser.merge_manifests({**threaded, "version": "changed"}, single)[1] != version


def test_merge_fallback_rewrites_manifest_worker_and_sidecars(tmp_path):
    site, fallback = tmp_path / "site", tmp_path / "single"
    site.mkdir()
    (fallback / "_framework").mkdir(parents=True)
    (fallback / "_framework" / "a.wasm").write_bytes(b"runtime")
    (fallback / "_framework" / "a.wasm.br").write_bytes(b"compressed")
    (site / "service-worker-assets.js").write_text('self.assetsManifest = {"version":"t","assets":[]};')
    (fallback / "service-worker-assets.js").write_text('self.assetsManifest = {"version":"s","assets":[{"url":"_framework/a.wasm","hash":"sha256-fallback"}]};')
    (site / "coi-sw.js").write_text('/* Manifest version: old */\nworker();\n')
    for name in ("coi-sw.js", "service-worker-assets.js"):
        for suffix in (".br", ".gz"):
            (site / (name + suffix)).write_bytes(b"stale")
    publish_browser.merge_fallback(site, fallback)
    merged = publish_browser.read_manifest(site / "service-worker-assets.js")
    assert merged["assets"][0]["url"] == "_framework-single/a.wasm"
    assert (site / "_framework-single/a.wasm").read_bytes() == b"runtime"
    assert (site / "_framework-single/a.wasm.br").read_bytes() == b"compressed"
    worker = (site / "coi-sw.js").read_text()
    assert worker.startswith(f"/* Manifest version: {merged['version']} */")
    assert "old" not in worker
    for name in ("coi-sw.js", "service-worker-assets.js"):
        assert not (site / (name + ".br")).exists()
        assert not (site / (name + ".gz")).exists()
