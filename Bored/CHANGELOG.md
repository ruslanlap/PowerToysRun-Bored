# Changelog

All notable changes to the Bored PowerToys Run Plugin will be documented in this file.

## [1.0.0] - 2025-10-29

### 🚨 Fixed
- **Critical**: Fixed plugin loading issue due to missing `Microsoft.Extensions.Caching.Memory.dll`
  - Added `<CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>` to ensure all NuGet dependencies are included
  - Plugin now loads correctly without "Cannot initialize Bored plugin as it was not loaded" error

### ✨ Added
- **Auto-copy feature**: All results (quotes, jokes, time, exchange rates, facts) are now automatically copied to clipboard when you press Enter
- **Context menu**: Added copy button (Ctrl+C) for all results
- **Emoji indicators**: Added emojis for better visual identification:
  - 💭 for quotes
  - 😄 for jokes
  - 💱 for exchange rates
  - 🕒 for time
  - 🐱 for cat facts
  - 🌍 for world time
- **Popular timezone quick access**: Added suggestions for common timezones:
  - 🇬🇧 London (Europe/London)
  - 🇺🇸 New York (America/New_York)
  - 🇯🇵 Tokyo (Asia/Tokyo)
  - 🇦🇺 Sydney (Australia/Sydney)
  - 🇩🇪 Berlin (Europe/Berlin)
  - 🇨🇳 Shanghai (Asia/Shanghai)
  - 🇫🇷 Paris (Europe/Paris)
  - 🇮🇳 Mumbai (Asia/Kolkata)
- **Popular currency pairs**: Added quick access to common conversions:
  - 💵 USD → EUR
  - 💶 EUR → USD
  - 💷 GBP → USD
  - 💴 USD → JPY
- **Usage examples**: When entering incomplete commands, the plugin now shows helpful examples
- **Better error messages**: More descriptive error messages with helpful hints

### 🔄 Changed
- **Quote API**: Switched from `quote-garden.onrender.com` to `zenquotes.io`
  - **Reason**: quote-garden service was suspended
  - **Benefit**: Fast, reliable, and free API with fresh quotes
- **Exchange Rate API**: Switched from `api.exchangerate.host` to `open.er-api.com`
  - **Reason**: exchangerate.host now requires API key; open.er-api.com is free and supports 160+ currencies
  - **Benefit**: Plugin works out of the box without any configuration and supports more currencies including UAH, EUR, USD, etc.
- **Result presentation**: Improved how results are displayed in PowerToys Run
- **User experience**: Commands now show "Press Enter to fetch..." instead of requiring double interaction

### 🛠️ Technical Changes
- Updated `QuoteService.cs` to use ZenQuotes API
- Added `ZenQuoteResponse` model for new quote API
- Updated `ExchangeRateService.cs` to use open.er-api.com
- Added `OpenExchangeRateResponse` model for new exchange API
- Refactored `GetQuoteResults()`, `GetJokeResults()`, `GetExchangeResults()`, `GetTimeResults()`, and `GetCatFactResults()` methods
- Improved `LoadContextMenus()` with better error handling
- Enhanced clipboard operations with exception handling

### 🧪 API Testing Results
All APIs tested and working:
- ✅ **Quote**: zenquotes.io/api/random
- ✅ **Joke**: official-joke-api.appspot.com
- ✅ **Cat Fact**: catfact.ninja/fact
- ✅ **Dog**: dog.ceo/api/breeds/image/random
- ✅ **Time**: worldtimeapi.org
- ✅ **Exchange**: open.er-api.com (supports UAH + 160 currencies)

### 📦 Dependencies
All required Microsoft.Extensions.* packages are now properly included:
- Microsoft.Extensions.Caching.Memory (v9.0.0)
- Microsoft.Extensions.Caching.Abstractions (v9.0.0)
- Microsoft.Extensions.Options (v9.0.0)
- Microsoft.Extensions.Primitives (v9.0.0)
- Microsoft.Extensions.DependencyInjection.Abstractions (v9.0.0)
- Microsoft.Extensions.Logging.Abstractions (v9.0.0)

### 📝 Documentation
- Updated INSTALLATION_FIX.md with new features and usage examples
- Updated README.md with improvements list
- Added this CHANGELOG.md

## Usage Examples

### Time
```
bored time Europe/London      # Time in London
bored time America/New_York   # Time in New York
bored time Asia/Tokyo         # Time in Tokyo
```

### Currency Exchange
```
bored exchange USD EUR 100    # Convert 100 USD to EUR
bored exchange UAH EUR 100    # Convert 100 UAH to EUR
bored exchange UAH USD 1000   # Convert 1000 UAH to USD
bored exchange EUR GBP 50     # Convert 50 EUR to GBP
```

### Other Commands
```
bored quote      # Random inspirational quote
bored joke       # Random joke
bored catfact    # Random cat fact
bored dog        # Random dog picture
bored text       # Text transformation
```

---

### Checksums (SHA256)

**x64**: `2dfaec964b6856f83dbd570c4b215435fc5461c770e674afc12d61848d464cdf`

**ARM64**: `df70fca239fef46bd534fa0765764b05fb9e7238db7b3e28a98109d2f8f5bb84`
