#!/usr/bin/env python3
"""Publishes threaded and fallback browser runtimes in parallel and optionally serves them locally.

`dotnet run --project src/ContreJour.Browser` is the quick path, on the interpreter. This is the build
that plays at full speed: AOT-compiled and trimmed, served as static files.

Usage:
    python3 tools/publish_browser.py --serve          # publish with AOT, serve on http://127.0.0.1:8080
    python3 tools/publish_browser.py --no-aot --serve # minutes faster; physics runs on the interpreter
"""

from __future__ import annotations

import argparse
import base64
import hashlib
import json
import re
import functools
import shutil
import subprocess
import sys
import tempfile
import threading
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
PROJECT = REPO_ROOT / "src" / "ContreJour.Browser" / "ContreJour.Browser.csproj"
CONTENT_CATALOG = (
    REPO_ROOT / "src" / "ContreJour.Browser" / "wwwroot" / "content" / "assets.json"
)
DEFAULT_PORT = 8080
FRAMEWORK = "_framework"
SINGLE_FRAMEWORK = "_framework-single"
WORKER = "coi-sw.js"
ASSETS_MANIFEST = "service-worker-assets.js"
VERSION_COMMENT = re.compile(r"^/\* Manifest version: [^*]* \*/\r?\n")


def parse_args(argv: list[str] | None) -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument("--output", type=Path, default=REPO_ROOT / "dist" / "browser")
    parser.add_argument(
        "--no-aot", dest="aot", action="store_false", help="Skip AOT compilation."
    )
    parser.add_argument(
        "--serve",
        type=int,
        nargs="?",
        const=DEFAULT_PORT,
        default=None,
        metavar="PORT",
        help=f"Serve the published site on 127.0.0.1 (default port {DEFAULT_PORT}).",
    )
    return parser.parse_args(argv)


def publish_command(
    output: Path, aot: bool, single_threaded: bool = False
) -> list[str]:
    # Each mode has different native objects; never reuse an obj/native tree between them.
    command = [
        "dotnet",
        "publish",
        str(PROJECT),
        "-c",
        "Release",
        "-o",
        str(output),
        "--artifacts-path",
        str(output.parent / (output.name + "-artifacts")),
        f"-p:RunAOTCompilation={'true' if aot else 'false'}",
        f"-p:CjSingleThreaded={'true' if single_threaded else 'false'}",
    ]
    if single_threaded:
        command.append("-p:CjSkipContent=true")
    return command


def publish_all(publishes: list[tuple[Path, list[str]]]) -> None:
    """Runs every publish at once, prefixing each output line with the tree it builds.

    All of them are waited for even after one fails, so no build is left writing into a
    tree the next run is about to delete.
    """
    for destination, _ in publishes:
        if destination.exists():
            shutil.rmtree(destination)

    lock = threading.Lock()
    width = max(len(destination.name) for destination, _ in publishes)

    def relay(name: str, stream) -> None:
        for line in stream:
            with lock:
                sys.stdout.write(f"[{name:<{width}}] {line}")
                sys.stdout.flush()

    running = []
    for destination, command in publishes:
        print(f"publishing {destination.name}", flush=True)
        process = subprocess.Popen(
            command,
            cwd=REPO_ROOT,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            encoding="utf-8",
            errors="replace",
        )
        reader = threading.Thread(
            target=relay, args=(destination.name, process.stdout), daemon=True
        )
        reader.start()
        running.append((destination.name, process, reader))

    failed = []
    for name, process, reader in running:
        process.wait()
        reader.join()
        if process.returncode != 0:
            failed.append(f"{name} (exit {process.returncode})")
    if failed:
        raise SystemExit(f"publish failed: {', '.join(failed)}")


def merge_fallback(site: Path, fallback: Path) -> None:
    """Keep fallback runtime bytes intact and reconcile the offline manifest."""
    source = fallback / "_framework"
    if not source.is_dir():
        raise RuntimeError(f"fallback publish has no framework directory: {source}")
    destination = site / "_framework-single"
    if destination.exists():
        shutil.rmtree(destination)
    # Module and resource URLs resolve relative to dotnet.js in this SDK. Copy bytes,
    # including compressed sidecars, unchanged to preserve fingerprints and integrity.
    shutil.copytree(source, destination)
    merged, version = merge_manifests(
        read_manifest(site / ASSETS_MANIFEST),
        read_manifest(fallback / ASSETS_MANIFEST),
    )
    write_manifest(site / ASSETS_MANIFEST, merged, version)
    stamp_worker(site / WORKER, version)


def read_manifest(path: Path) -> dict:
    """Reads `self.assetsManifest = {...};` as the object it assigns."""
    text = path.read_text(encoding="utf-8")
    return json.loads(text[text.index("{") : text.rindex("}") + 1])


def merge_manifests(threaded: dict, single: dict) -> tuple[dict, str]:
    """Returns the two manifests as one, and the version that stands for the pair.

    The fallback's assets are readdressed on the way in, because they were published at
    `_framework/` and now live at `_framework-single/`. Anything else it lists - the page,
    the scripts - is the same file the threaded publish already contributed, so only its
    runtime crosses over.

    The version is derived from the two the SDK calculated rather than recalculated from
    the assets. Each of those already changes when anything in its own tree does, which is
    the whole property the cache name needs, and inheriting it avoids keeping a private
    copy of how the SDK arrives at one.
    """
    assets = list(threaded["assets"])
    known = {asset["url"] for asset in assets}
    for asset in single["assets"]:
        if not asset["url"].startswith(f"{FRAMEWORK}/"):
            continue
        moved = dict(asset)
        moved["url"] = f"{SINGLE_FRAMEWORK}/{asset['url'][len(FRAMEWORK) + 1 :]}"
        if moved["url"] in known:
            continue
        known.add(moved["url"])
        assets.append(moved)

    combined = f"{threaded['version']}{single['version']}".encode("utf-8")
    version = base64.b64encode(hashlib.sha256(combined).digest()).decode()[:8]
    return {"version": version, "assets": assets}, version


def write_manifest(path: Path, manifest: dict, version: str) -> None:
    manifest = {"version": version, "assets": manifest["assets"]}
    body = json.dumps(manifest, indent=2)
    path.write_text(f"self.assetsManifest = {body};\n", encoding="utf-8")
    drop_stale_copies(path)


def stamp_worker(path: Path, version: str) -> None:
    """Replaces the version the SDK stamped into the worker with the merged one.

    The comment is what makes the worker's own bytes change between publishes. A browser
    compares them to decide whether a new worker exists at all, so a stale one here means
    a deployment nobody is offered.
    """
    text = path.read_text(encoding="utf-8")
    text = VERSION_COMMENT.sub("", text, count=1)
    path.write_text(f"/* Manifest version: {version} */\n{text}", encoding="utf-8")
    drop_stale_copies(path)


def drop_stale_copies(path: Path) -> None:
    """Removes the .br and .gz beside a file this script rewrote.

    The SDK compressed the version it published, and those copies now decode to something
    the site no longer serves. A server that picks one by Accept-Encoding would hand out
    the pre-merge manifest to exactly the visitors whose browsers ask for it.
    """
    for suffix in (".br", ".gz"):
        compressed = path.with_name(path.name + suffix)
        if compressed.exists():
            compressed.unlink()


class Handler(SimpleHTTPRequestHandler):
    """Static files with the types the runtime needs and no caching, so a republish shows up on reload."""

    extensions_map = {
        **SimpleHTTPRequestHandler.extensions_map,
        ".wasm": "application/wasm",
        ".js": "text/javascript",
        ".mjs": "text/javascript",
        ".json": "application/json",
        ".webmanifest": "application/manifest+json",
        ".webp": "image/webp",
        ".ogg": "audio/ogg",
        ".ttf": "font/ttf",
        ".dat": "application/octet-stream",
    }

    def end_headers(self) -> None:
        self.send_header("Cache-Control", "no-cache")
        self.send_header("Cross-Origin-Opener-Policy", "same-origin")
        self.send_header("Cross-Origin-Embedder-Policy", "require-corp")
        super().end_headers()


def serve(root: Path, port: int) -> None:
    server = ThreadingHTTPServer(
        ("127.0.0.1", port), functools.partial(Handler, directory=str(root))
    )
    print(f"Serving {root} on http://127.0.0.1:{port}/ (Ctrl+C to stop)", flush=True)
    try:
        server.serve_forever()
    except KeyboardInterrupt:
        pass
    finally:
        server.server_close()


def main(argv: list[str] | None = None) -> int:
    args = parse_args(argv)
    if not CONTENT_CATALOG.is_file():
        print(
            f"error: {CONTENT_CATALOG} is missing; run tools/build_web_content.py first",
            file=sys.stderr,
        )
        return 1
    # Publish into fresh staging trees so removed static files and old native output
    # cannot survive a subsequent publish. Leave the current site intact if a build fails.
    with tempfile.TemporaryDirectory(prefix="cj-browser-") as temporary:
        stage = Path(temporary)
        threaded = stage / "threaded"
        single = stage / "single"
        publish_all([
            (threaded, publish_command(threaded, args.aot, single_threaded=False)),
            (single, publish_command(single, args.aot, single_threaded=True)),
        ])
        merge_fallback(threaded / "wwwroot", single / "wwwroot")
        if args.output.exists():
            shutil.rmtree(args.output)
        shutil.copytree(threaded, args.output)
    if args.serve is not None:
        serve(args.output / "wwwroot", args.serve)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
