
#!/bin/bash

set -e

# Clean up previous builds
rm -rf ./Bored/Publish
rm -rf ./Bored/Community.PowerToys.Run.Plugin.Bored/obj
rm -rf ./Bored/Community.PowerToys.Run.Plugin.Bored/bin
rm -rf ./Bored-*.zip

PROJECT_PATH="Bored/Community.PowerToys.Run.Plugin.Bored/Community.PowerToys.Run.Plugin.Bored.csproj"
PLUGIN_NAME="Bored"

# Get version from plugin.json
VERSION=$(grep '"Version"' Bored/Community.PowerToys.Run.Plugin.Bored/plugin.json | sed 's/.*"Version": "\([^"]*\)".*/\1/')

echo "📋 Plugin: $PLUGIN_NAME"
echo "📋 Version: $VERSION"

# Dependencies to exclude (these are provided by PowerToys)
DEPENDENCIES_TO_EXCLUDE="PowerToys.Common.UI.* PowerToys.ManagedCommon.* PowerToys.Settings.UI.Lib.* Wox.Infrastructure.* Wox.Plugin.*"

# Build for x64
echo "🛠️  Building for x64..."
dotnet publish "$PROJECT_PATH" -c Release -r win-x64 --self-contained false

# Build for ARM64  
echo "🛠️  Building for ARM64..."
dotnet publish "$PROJECT_PATH" -c Release -r win-arm64 --self-contained false

# Package x64
echo "📦 Packaging x64..."
PUBLISH_X64="./Bored/Community.PowerToys.Run.Plugin.Bored/bin/Release/net9.0-windows10.0.22621.0/win-x64/publish"
DEST_X64="./Bored/Publish/x64"
ZIP_X64="./${PLUGIN_NAME}-${VERSION}-x64.zip"

rm -rf "$DEST_X64"
mkdir -p "$DEST_X64"

# Copy files excluding unnecessary dependencies
cp -r "$PUBLISH_X64"/* "$DEST_X64/"

# Remove unnecessary dependencies
for dep in $DEPENDENCIES_TO_EXCLUDE; do
    find "$DEST_X64" -name "$dep" -delete 2>/dev/null || true
done


# Create zip
(cd "$DEST_X64" && zip -r "../../$(basename "$ZIP_X64")" .)

# Package ARM64
echo "📦 Packaging ARM64..."
PUBLISH_ARM64="./Bored/Community.PowerToys.Run.Plugin.Bored/bin/Release/net9.0-windows10.0.22621.0/win-arm64/publish"
DEST_ARM64="./Bored/Publish/arm64"
ZIP_ARM64="./${PLUGIN_NAME}-${VERSION}-arm64.zip"

rm -rf "$DEST_ARM64"
mkdir -p "$DEST_ARM64"

# Copy files excluding unnecessary dependencies
cp -r "$PUBLISH_ARM64"/* "$DEST_ARM64/"

# Remove unnecessary dependencies
for dep in $DEPENDENCIES_TO_EXCLUDE; do
    find "$DEST_ARM64" -name "$dep" -delete 2>/dev/null || true
done



# Create zip
(cd "$DEST_ARM64" && zip -r "../../$(basename "$ZIP_ARM64")" .)

# Move zips to root
mv "$DEST_X64/../$(basename "$ZIP_X64")" .
mv "$DEST_ARM64/../$(basename "$ZIP_ARM64")" .

echo "✅ Done! Created:"
echo " - $ZIP_X64"
echo " - $ZIP_ARM64"

# Generate checksums
echo "🔐 Generating checksums..."
echo "x64 SHA256: $(sha256sum "$ZIP_X64" | cut -d' ' -f1)"
echo "ARM64 SHA256: $(sha256sum "$ZIP_ARM64" | cut -d' ' -f1)"
