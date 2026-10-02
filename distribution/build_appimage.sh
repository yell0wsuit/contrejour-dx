#!/bin/bash

# Build script for creating an AppImage for Contre Jour DX
# Usage: `./build_appimage.sh [version]` or `bash build_appimage.sh [version]`
#
# Requirements:
#   - .NET 10.0 SDK
#   - wget (for downloading appimagetool if not present)

set -e

# Configuration
APP_NAME="ContreJour-DX"
APP_ID="page.yell0wsuit.contrejour.dx"
APP_DISPLAY_NAME="Contre Jour DX"
EXEC_NAME="ContreJour-DX"
DESCRIPTION="Contre Jour DX, a desktop port of Contre Jour."

# Directories
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
PROJECT="$PROJECT_ROOT/src/ContreJour.Desktop/ContreJour.Desktop.csproj"
BUILD_DIR="$PROJECT_ROOT/src/ContreJour.Desktop/bin/appimage_build"
PUBLISH_DIR="$PROJECT_ROOT/src/ContreJour.Desktop/bin/Publish/linux-x64"
APPDIR="$BUILD_DIR/$APP_NAME.AppDir"
TOOLS_DIR="$PROJECT_ROOT/src/ContreJour.Desktop/bin/distribution_tools"
TEMPLATES_DIR="$SCRIPT_DIR/templates/linux"

# Resolve version (from arg or csproj)
VERSION="$1"
if [ -z "$VERSION" ]; then
    echo "Error: version is required. Usage: $0 <version>"
    exit 1
fi

# "+" in a prerelease version is not kept in GitHub asset names, so file names use "_".
FILE_VERSION="${VERSION//+/_}"

echo "=== Building Contre Jour DX v$VERSION AppImage ==="

# Step 1: Build the application
echo "[1/5] Building Linux x64 release..."
rm -rf "$PUBLISH_DIR"
dotnet publish "$PROJECT" \
    -c Release \
    -r linux-x64 \
    -f net10.0 \
    -p:PublishAot=false \
    --self-contained true \
    -p:VersionPrefix="$VERSION" -p:VersionSuffix= \
    -o "$PUBLISH_DIR"


# Step 2: Create AppDir structure
echo "[2/5] Creating AppDir structure..."
rm -rf "$BUILD_DIR"
mkdir -p "$APPDIR/usr/bin"
mkdir -p "$APPDIR/usr/share/applications"
mkdir -p "$APPDIR/usr/share/icons/hicolor/512x512/apps"

# Step 3: Copy application files
echo "[3/5] Copying application files..."

# Copy all published files to usr/bin (this includes the executable, content, etc.)
cp -r "$PUBLISH_DIR"/* "$APPDIR/usr/bin/"
chmod +x "$APPDIR/usr/bin/$EXEC_NAME"

# Create AppRun script
cat > "$APPDIR/AppRun" << 'EOF'
#!/bin/bash
SELF=$(readlink -f "$0")
HERE=${SELF%/*}
export PATH="${HERE}/usr/bin:${PATH}"
cd "${HERE}/usr/bin"
exec "./__EXEC_NAME__" "$@"
EOF
sed -i "s/__EXEC_NAME__/$EXEC_NAME/g" "$APPDIR/AppRun"
chmod +x "$APPDIR/AppRun"

# Step 4: Create metadata files
echo "[4/5] Creating metadata files..."

# Desktop entry (in root of AppDir for appimagetool)
sed -e "s/{{APP_DISPLAY_NAME}}/$APP_DISPLAY_NAME/g" \
    -e "s/{{DESCRIPTION}}/$DESCRIPTION/g" \
    -e "s/{{EXEC_NAME}}/$EXEC_NAME/g" \
    -e "s/{{APP_NAME}}/$APP_NAME/g" \
    -e "s/{{VERSION}}/$VERSION/g" \
    "$TEMPLATES_DIR/appimage.desktop" > "$APPDIR/$APP_NAME.desktop"

# Also copy to standard location
cp "$APPDIR/$APP_NAME.desktop" "$APPDIR/usr/share/applications/"

# Copy icon to root of AppDir (required by appimagetool)
if [ -f "$SCRIPT_DIR/icons/ContreJourDXIcon_512.png" ]; then
    cp "$SCRIPT_DIR/icons/ContreJourDXIcon_512.png" "$APPDIR/$APP_NAME.png"
    cp "$SCRIPT_DIR/icons/ContreJourDXIcon_512.png" "$APPDIR/usr/share/icons/hicolor/512x512/apps/$APP_NAME.png"
    # Create .DirIcon symlink (optional but nice for file managers)
    ln -sf "$APP_NAME.png" "$APPDIR/.DirIcon"
else
    echo "Warning: Icon not found at icons/ContreJourDXIcon_512.png"
fi

# Create AppStream metadata
mkdir -p "$APPDIR/usr/share/metainfo"
RELEASE_DATE=$(date +%Y-%m-%d)
sed -e "s/{{APP_ID}}/$APP_ID/g" \
    -e "s/{{APP_DISPLAY_NAME}}/$APP_DISPLAY_NAME/g" \
    -e "s/{{APP_NAME}}/$APP_NAME/g" \
    -e "s/{{EXEC_NAME}}/$EXEC_NAME/g" \
    -e "s/{{VERSION}}/$VERSION/g" \
    -e "s/{{RELEASE_DATE}}/$RELEASE_DATE/g" \
    "$TEMPLATES_DIR/appimage.metainfo.xml" > "$APPDIR/usr/share/metainfo/$APP_ID.metainfo.xml"

# Step 5: Build AppImage
echo "[5/5] Building AppImage..."

# Download appimagetool if not available
APPIMAGETOOL="$TOOLS_DIR/appimagetool-x86_64.AppImage"
if [ ! -f "$APPIMAGETOOL" ]; then
    echo "Downloading appimagetool..."
    mkdir -p "$TOOLS_DIR"
    wget -q --show-progress -O "$APPIMAGETOOL" \
        "https://github.com/AppImage/appimagetool/releases/download/continuous/appimagetool-x86_64.AppImage"
    chmod +x "$APPIMAGETOOL"
fi

# Build the AppImage. appimagetool is itself an AppImage, so it needs FUSE to
# self-mount; CI runners have none, hence the extract-and-run fallback.
APPIMAGE_FILE="$PUBLISH_DIR/ContreJourDX-v${FILE_VERSION}-x86_64.AppImage"
ARCH=x86_64 "$APPIMAGETOOL" --appimage-extract-and-run "$APPDIR" "$APPIMAGE_FILE"

# Cleanup build directory
rm -rf "$BUILD_DIR"

APPIMAGE_SIZE=$(du -h "$APPIMAGE_FILE" | cut -f1)

# Copy to release_github
RELEASE_DIR="$PROJECT_ROOT/src/ContreJour.Desktop/bin/release_github"
mkdir -p "$RELEASE_DIR"
cp "$APPIMAGE_FILE" "$RELEASE_DIR/"

echo ""
echo "=== Build complete! ==="
echo "AppImage created: $APPIMAGE_FILE ($APPIMAGE_SIZE)"
echo "Copied to:        $RELEASE_DIR/"
echo ""
echo "To run: chmod +x $APPIMAGE_FILE && $APPIMAGE_FILE"
echo "Or simply double-click the AppImage file in your file manager."
