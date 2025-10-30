# Bored Plugin for PowerToys Run

A PowerToys Run plugin that helps you when you're bored by suggesting random activities, jokes, facts, and more!

## ✅ Recent Fix (v1.0.0)

### Issue
The plugin was failing to load with the error:
```
System.IO.FileNotFoundException: Could not load file or assembly 'Microsoft.Extensions.Caching.Memory, Version=9.0.0.0'
```

### Solution
Added `<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>` to the project file to ensure all NuGet dependencies are included in the build output.

### Changes Made
In `Community.PowerToys.Run.Plugin.Bored.csproj`:
```xml
<PropertyGroup>
  <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
</PropertyGroup>
```

All Microsoft.Extensions.* dependencies are now properly included in the plugin package.

## 📦 Installation

See [INSTALLATION_FIX.md](../INSTALLATION_FIX.md) for detailed installation instructions.

### Quick Install

1. Download the appropriate ZIP file for your system:
   - x64: `Bored-1.0.0-x64.zip`
   - ARM64: `Bored-1.0.0-arm64.zip`

2. Extract to: `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins\Bored\`

3. Restart PowerToys

## 🔍 Verification

After installation, verify these DLLs exist in the plugin folder:
- ✅ Microsoft.Extensions.Caching.Memory.dll
- ✅ Microsoft.Extensions.Caching.Abstractions.dll
- ✅ Microsoft.Extensions.Options.dll
- ✅ Microsoft.Extensions.Primitives.dll
- ✅ Microsoft.Extensions.DependencyInjection.Abstractions.dll
- ✅ Microsoft.Extensions.Logging.Abstractions.dll

## ✨ New Features & Improvements

### UX Improvements
- 🎯 **Auto-copy to clipboard** - All results automatically copied after pressing Enter
- 🎨 **Emoji indicators** - Better visual identification of results
- 📋 **Context menu** - Copy button (Ctrl+C) for all results
- 💡 **Usage examples** - Quick access to common timezones and currency pairs
- 🌍 **Popular timezones** - London, New York, Tokyo, Sydney, Berlin, Shanghai, Paris, Mumbai
- 💱 **Popular currencies** - USD/EUR, EUR/USD, GBP/USD, USD/JPY

### Technical Improvements
- ✅ **New Exchange API** - Using free open.er-api.com API (no API key required, supports 160+ currencies including UAH)
- ✅ **Better error messages** - More helpful error descriptions
- ✅ **Improved timezone support** - Examples and suggestions for timezone format

## 📋 Checksums (SHA256)

**x64**: `2dfaec964b6856f83dbd570c4b215435fc5461c770e674afc12d61848d464cdf`

**ARM64**: `df70fca239fef46bd534fa0765764b05fb9e7238db7b3e28a98109d2f8f5bb84`

## 🏗️ Building from Source

```bash
./build-and-zip.sh
```

This will create both x64 and ARM64 packages with all dependencies included.
