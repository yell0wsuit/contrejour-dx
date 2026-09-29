#!/usr/bin/env bash
# Compiles src/ContreJour.Desktop/MonoGame/Shaders/*.fx to OpenGL (*.ogl.mgfxo) for DesktopGL.
# MonoGame's effect compiler needs Wine + the real d3dcompiler_47.dll, so it runs in an x86_64 container.
set -euo pipefail

HERE="$(cd "$(dirname "$0")" && pwd)"
SHADERS="$HERE/../../src/ContreJour.Desktop/MonoGame/Shaders"
IMAGE=contrejour-mgfxc

docker build --platform linux/amd64 -t "$IMAGE" "$HERE"

for fx in "$SHADERS"/*.fx; do
    name="$(basename "$fx" .fx)"
    docker run --rm --platform linux/amd64 -v "$SHADERS":/work "$IMAGE" \
        mgcb /platform:DesktopGL /outputDir:/work/.out /intermediateDir:/tmp/obj \
             /importer:EffectImporter /processor:EffectProcessor "/build:$name.fx"
    # MonoGameRenderer loads raw MGFX bytes (new Effect(device, bytes)); unwrap them from the XNB container.
    python3 - "$SHADERS/.out/$name.xnb" "$SHADERS/$name.ogl.mgfxo" <<'PY'
import struct, sys
data = open(sys.argv[1], "rb").read()
start = data.index(b"MGFX")
length = struct.unpack("<i", data[start - 4:start])[0]
open(sys.argv[2], "wb").write(data[start:start + length])
PY
    echo "Built $name.ogl.mgfxo"
done

rm -rf "$SHADERS/.out"
