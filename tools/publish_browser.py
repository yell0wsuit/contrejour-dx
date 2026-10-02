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
    """Keep the fallback framework beside the threaded one without altering SDK assets."""
    source = fallback / "_framework"
    if not source.is_dir():
        raise RuntimeError(f"fallback publish has no framework directory: {source}")
    destination = site / "_framework-single"
    if destination.exists():
        shutil.rmtree(destination)
    # Module and resource URLs resolve relative to dotnet.js in this SDK. Copy bytes,
    # including compressed sidecars, unchanged to preserve fingerprints and integrity.
    shutil.copytree(source, destination)


class Handler(SimpleHTTPRequestHandler):
    """Static files with the types the runtime needs and no caching, so a republish shows up on reload."""

    extensions_map = {
        **SimpleHTTPRequestHandler.extensions_map,
        ".wasm": "application/wasm",
        ".js": "text/javascript",
        ".mjs": "text/javascript",
        ".json": "application/json",
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
