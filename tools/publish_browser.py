#!/usr/bin/env python3
"""Publishes the browser build and, with --serve, serves it on localhost.

`dotnet run --project src/ContreJour.Browser` is the quick path, on the interpreter. This is the build
that plays at full speed: AOT-compiled and trimmed, served as static files.

Usage:
    python3 tools/publish_browser.py --serve          # publish with AOT, serve on http://127.0.0.1:8080
    python3 tools/publish_browser.py --no-aot --serve # minutes faster; physics runs on the interpreter
"""

from __future__ import annotations

import argparse
import functools
import subprocess
import sys
from http.server import SimpleHTTPRequestHandler, ThreadingHTTPServer
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
PROJECT = REPO_ROOT / "src" / "ContreJour.Browser" / "ContreJour.Browser.csproj"
CONTENT_CATALOG = REPO_ROOT / "src" / "ContreJour.Browser" / "wwwroot" / "content" / "assets.json"
DEFAULT_PORT = 8080


def parse_args(argv: list[str] | None) -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--output", type=Path, default=REPO_ROOT / "dist" / "browser")
    parser.add_argument("--no-aot", dest="aot", action="store_false", help="Skip AOT compilation.")
    parser.add_argument(
        "--serve", type=int, nargs="?", const=DEFAULT_PORT, default=None, metavar="PORT",
        help=f"Serve the published site on 127.0.0.1 (default port {DEFAULT_PORT}).",
    )
    return parser.parse_args(argv)


def publish_command(output: Path, aot: bool) -> list[str]:
    command = ["dotnet", "publish", str(PROJECT), "-c", "Release", "-o", str(output)]
    if aot:
        command.append("-p:RunAOTCompilation=true")
    return command


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
        super().end_headers()


def serve(root: Path, port: int) -> None:
    server = ThreadingHTTPServer(("127.0.0.1", port), functools.partial(Handler, directory=str(root)))
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
        print(f"error: {CONTENT_CATALOG} is missing; run tools/build_web_content.py first", file=sys.stderr)
        return 1
    subprocess.run(publish_command(args.output, args.aot), check=True)
    if args.serve is not None:
        serve(args.output / "wwwroot", args.serve)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
