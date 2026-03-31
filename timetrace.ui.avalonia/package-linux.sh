#!/bin/bash
# TimeTrace Linux UI - Local Packaging Script
# Creates self-contained builds and AppImage for local testing

set -e

VERSION="${1:-0.1.0-alpha.local}"
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
OUTPUT_DIR="${PROJECT_DIR}/dist"
PUBLISH_DIR="${PROJECT_DIR}/publish"

echo "========================================="
echo "TimeTrace Linux UI Packaging Script"
echo "Version: ${VERSION}"
echo "========================================="

# Check prerequisites
command -v dotnet >/dev/null 2>&1 || { echo "Error: .NET SDK not found"; exit 1; }

# Clean previous builds
rm -rf "${OUTPUT_DIR}" "${PUBLISH_DIR}"
mkdir -p "${OUTPUT_DIR}" "${PUBLISH_DIR}"

# Build and publish for linux-x64
echo ""
echo "Building for linux-x64..."
dotnet publish "${PROJECT_DIR}/timetrace.ui.avalonia.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:EnableCompressionInSingleFile=true \
    -p:Version="${VERSION}" \
    -o "${PUBLISH_DIR}/linux-x64"

# Create tar.gz archive
echo "Creating tar.gz archive..."
cd "${PUBLISH_DIR}/linux-x64"
tar -czvf "${OUTPUT_DIR}/TimeTrace-Linux-x64-${VERSION}.tar.gz" *
cd "${PROJECT_DIR}"

# Build for linux-arm64
echo ""
echo "Building for linux-arm64..."
dotnet publish "${PROJECT_DIR}/timetrace.ui.avalonia.csproj" \
    -c Release \
    -r linux-arm64 \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:EnableCompressionInSingleFile=true \
    -p:Version="${VERSION}" \
    -o "${PUBLISH_DIR}/linux-arm64"

# Create tar.gz archive
cd "${PUBLISH_DIR}/linux-arm64"
tar -czvf "${OUTPUT_DIR}/TimeTrace-Linux-arm64-${VERSION}.tar.gz" *
cd "${PROJECT_DIR}"

# Create AppImage (x64 only)
echo ""
echo "Creating AppImage..."

# Publish non-single-file for AppImage
dotnet publish "${PROJECT_DIR}/timetrace.ui.avalonia.csproj" \
    -c Release \
    -r linux-x64 \
    --self-contained true \
    -p:PublishSingleFile=false \
    -p:Version="${VERSION}" \
    -o "${PUBLISH_DIR}/AppDir/usr/bin"

# Create AppDir structure
mkdir -p "${PUBLISH_DIR}/AppDir/usr/share/applications"
mkdir -p "${PUBLISH_DIR}/AppDir/usr/share/icons/hicolor/256x256/apps"

# Create desktop entry
cat > "${PUBLISH_DIR}/AppDir/usr/share/applications/timetrace.desktop" << EOF
[Desktop Entry]
Name=TimeTrace
Comment=Application time tracking and screenshot capture
Exec=timetrace.ui.avalonia
Icon=timetrace
Type=Application
Categories=Utility;
Terminal=false
EOF

# Copy icon if exists
if [ -f "${PROJECT_DIR}/Assets/timeTrace.ico" ]; then
    cp "${PROJECT_DIR}/Assets/timeTrace.ico" \
        "${PUBLISH_DIR}/AppDir/usr/share/icons/hicolor/256x256/apps/timetrace.png" 2>/dev/null || true
fi

# Create symlinks for desktop integration
ln -sf usr/share/applications/timetrace.desktop "${PUBLISH_DIR}/AppDir/timetrace.desktop"
ln -sf usr/share/icons/hicolor/256x256/apps/timetrace.png "${PUBLISH_DIR}/AppDir/timetrace.png" 2>/dev/null || true

# Create AppRun script
cat > "${PUBLISH_DIR}/AppDir/AppRun" << 'EOF'
#!/bin/bash
HERE="$(dirname "$(readlink -f "${0}")")"
export PATH="${HERE}/usr/bin:${PATH}"
export LD_LIBRARY_PATH="${HERE}/usr/lib:${LD_LIBRARY_PATH}"
exec "${HERE}/usr/bin/timetrace.ui.avalonia" "$@"
EOF
chmod +x "${PUBLISH_DIR}/AppDir/AppRun"

# Download appimagetool if not present
APPIMAGETOOL="${PROJECT_DIR}/tools/appimagetool-x86_64.AppImage"
if [ ! -f "${APPIMAGETOOL}" ]; then
    echo "Downloading appimagetool..."
    mkdir -p "${PROJECT_DIR}/tools"
    wget -q -O "${APPIMAGETOOL}" \
        https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage
    chmod +x "${APPIMAGETOOL}"
fi

# Create AppImage
ARCH=x86_64 "${APPIMAGETOOL}" --appimage-extract-and-run \
    "${PUBLISH_DIR}/AppDir" \
    "${OUTPUT_DIR}/TimeTrace-${VERSION}-x86_64.AppImage"

# Summary
echo ""
echo "========================================="
echo "Packaging Complete!"
echo "========================================="
echo ""
echo "Output files in ${OUTPUT_DIR}:"
ls -lh "${OUTPUT_DIR}"
echo ""
echo "To run:"
echo "  tar.gz:   tar -xzf TimeTrace-Linux-*.tar.gz && ./timetrace.ui.avalonia"
echo "  AppImage: chmod +x TimeTrace-*.AppImage && ./TimeTrace-*.AppImage"
