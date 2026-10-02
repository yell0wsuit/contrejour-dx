#!/bin/sh
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"

# =========================
# App metadata
# =========================
APP_NAME="ContreJour.Desktop"
BUNDLE_ID="page.yell0wsuit.contrejour.dx"

# =========================
# Project / publish paths
# =========================
PROJECT="$PROJECT_ROOT/src/ContreJour.Desktop/ContreJour.Desktop.csproj"
PUBLISH_DIR="$PROJECT_ROOT/src/ContreJour.Desktop/bin/Publish/osx-arm64"
APP_DIR="$PUBLISH_DIR/$APP_NAME.app"
ICON_SOURCE="$PUBLISH_DIR/Resources/ContreJourDXIcon.icns"
TEMPLATES_DIR="$SCRIPT_DIR/templates/macos"

# =========================
# Resolve version (from arg or csproj)
# =========================
VERSION="$1"
if [ -z "$VERSION" ]; then
    echo "Error: version is required. Usage: $0 <version>"
    exit 1
fi

# NativeAOT: honour USE_AOT when preset (CI), otherwise ask if there's a terminal.
if [ -z "$USE_AOT" ]; then
    if [ -t 0 ]; then
        printf "Use NativeAOT? [Y/n]: "
        read -r AOT_INPUT
    else
        AOT_INPUT=""
    fi
    case "$AOT_INPUT" in
        [nN]) USE_AOT="false" ;;
        *)    USE_AOT="true" ;;
    esac
fi

echo "=== Building Contre Jour DX v$VERSION for macOS (NativeAOT: $USE_AOT) ==="

# =========================
# Step 1: Build the application
# =========================
echo "[1/4] Building macOS arm64 release..."
rm -rf "$PUBLISH_DIR"
dotnet publish "$PROJECT" \
    -c Release \
    -f net10.0 \
    -p:PublishAot="$USE_AOT" \
    -r osx-arm64 \
    --self-contained true \
    -p:VersionPrefix="$VERSION" -p:VersionSuffix= \
    -o "$PUBLISH_DIR"

# =========================
# Step 2: Create .app bundle
# =========================
echo "[2/4] Creating .app bundle structure..."
mkdir -p "$APP_DIR/Contents/MacOS"
mkdir -p "$APP_DIR/Contents/Resources"

# Copy runtime files
# Resources/ is excluded because the publish puts the .icns there and a bundle keeps its
# resources in Contents/Resources, not beside the executable. codesign treats a Resources
# directory under Contents/MacOS as a nested code object it cannot seal, and refuses to
# sign the bundle at all; the icon is copied to its proper home a few lines below.
rsync -a \
  --exclude '*.app' \
  --exclude 'Resources' \
  --exclude 'Assets' \
  --exclude '*.pdb' \
  --exclude '*.dSYM' \
  --exclude '/*.xml' \
  "$PUBLISH_DIR/" \
  "$APP_DIR/Contents/MacOS/"

# Resources must live in Contents/Resources for codesign. The game reads both
# Assets/ and Resources/ beside its executable; the symlink preserves that layout.
rsync -a "$PUBLISH_DIR/Resources/" "$APP_DIR/Contents/Resources/"
rsync -a "$PUBLISH_DIR/Assets/" "$APP_DIR/Contents/Resources/Assets/"
ln -s ../Resources/Assets "$APP_DIR/Contents/MacOS/Assets"
ln -s ../Resources "$APP_DIR/Contents/MacOS/Resources"
chmod +x "$APP_DIR/Contents/MacOS/$APP_NAME"
cp "$ICON_SOURCE" "$APP_DIR/Contents/Resources/$APP_NAME.icns"

# Write Info.plist
sed -e "s/{{APP_NAME}}/$APP_NAME/g" \
    -e "s/{{BUNDLE_ID}}/$BUNDLE_ID/g" \
    -e "s/{{VERSION}}/$VERSION/g" \
    "$TEMPLATES_DIR/Info.plist" > "$APP_DIR/Contents/Info.plist"


# =========================
# Step 3: Finalize
# =========================
echo "[3/4] Finalizing..."

# Dev convenience: remove quarantine attribute
xattr -dr com.apple.quarantine "$APP_DIR" || true

# Sign native libraries before the bundle so Apple Silicon can load them.
echo "Codesigning dylibs..."
find "$APP_DIR" -name '*.dylib' -print0 | xargs -0 -I {} codesign --force --sign - '{}'
echo "Codesigning .app bundle..."
codesign --force --sign - "$APP_DIR"

# =========================
# Step 4: Package .dmg
# =========================
echo "[4/4] Packaging .dmg archive..."

RELEASE_DIR="$PROJECT_ROOT/src/ContreJour.Desktop/bin/release_github"
mkdir -p "$RELEASE_DIR"
# "+" in a prerelease version is not kept in GitHub asset names, so file names use "_".
FILE_VERSION=$(printf '%s' "$VERSION" | tr '+' '_')
ARCHIVE_NAME="ContreJourDX-v${FILE_VERSION}-macOS-arm64.dmg"
ARCHIVE_PATH="$RELEASE_DIR/$ARCHIVE_NAME"

# Remove old archive if exists
rm -f "$ARCHIVE_PATH"

hdiutil create -volname "$APP_NAME" -srcfolder "$APP_DIR" -ov -format UDZO "$ARCHIVE_PATH"

echo ""
echo "=== Build complete! ==="
echo "App bundle: $APP_DIR"
echo "DMG:        $ARCHIVE_PATH"
