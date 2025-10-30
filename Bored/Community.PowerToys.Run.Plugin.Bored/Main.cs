using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Community.PowerToys.Run.Plugin.Bored.Services;
using ManagedCommon;
using Wox.Plugin;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.Bored
{
    /// <summary>
    /// Main class of this plugin that implement all used interfaces.
    /// </summary>
    public class Main : IPlugin, IPluginI18n, IContextMenu, IDisposable
    {
        /// <summary>
        /// ID of the plugin.
        /// </summary>
        public static string PluginID => "3548EB2720CF46509A8BA9044F970447";

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        public string Name => "Bored";

        /// <summary>
        /// Description of the plugin.
        /// </summary>
        public string Description => "Get random quotes, jokes, facts, exchange rates, time, dog pictures, and text transformations when you're bored";

        private PluginInitContext Context { get; set; } = null!;

        private string IconPath { get; set; } = null!;

        private bool Disposed { get; set; }

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly CacheService _cache;
        private readonly QuoteService _quoteService;
        private readonly JokeService _jokeService;
        private readonly ExchangeRateService _exchangeService;
        private readonly WorldTimeService _timeService;
        private readonly CatFactService _catFactService;
        private readonly DogService _dogService;
        private readonly TextActionService _textActionService;

        public Main()
        {
            _cache = new CacheService();
            _quoteService = new QuoteService(_cache);
            _jokeService = new JokeService(_cache);
            _exchangeService = new ExchangeRateService(_cache);
            _timeService = new WorldTimeService(_cache);
            _catFactService = new CatFactService(_cache);
            _dogService = new DogService(_cache);
            _textActionService = new TextActionService();
        }

        /// <summary>
        /// Return a filtered list, based on the given query.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query)
        {
            return Query(query, CancellationToken.None);
        }

        /// <summary>
        /// Return a filtered list, based on the given query with cancellation support.
        /// </summary>
        /// <param name="query">The query to filter the list.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A filtered list, can be empty when nothing was found.</returns>
        public List<Result> Query(Query query, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(query);

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var searchTerm = query.Search.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                return GetDefaultResults();
            }

            var parts = searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = parts[0].ToLowerInvariant();

            return command switch
            {
                "quote" => GetQuoteResults(_cancellationTokenSource.Token),
                "joke" => GetJokeResults(_cancellationTokenSource.Token),
                "exchange" => GetExchangeResults(parts.Skip(1).ToArray(), _cancellationTokenSource.Token),
                "time" => GetTimeResults(parts.Skip(1).ToArray(), _cancellationTokenSource.Token),
                "catfact" => GetCatFactResults(_cancellationTokenSource.Token),
                "dog" => GetDogResults(_cancellationTokenSource.Token),
                "text" => GetTextTransformResults(parts.Skip(1).ToArray()),
                _ => GetDefaultResults()
            };
        }

        private List<Result> GetDefaultResults()
        {
            return new List<Result>
            {
                CreateResult("Quote", "Get a random inspirational quote", "quote"),
                CreateResult("Joke", "Get a random joke", "joke"),
                CreateResult("Exchange", "Convert currency - Examples: exchange USD EUR 100", "exchange USD EUR 100"),
                CreateResult("Time", "Get time for timezone - Examples: Europe/London, America/New_York, Asia/Tokyo", "time Europe/London"),
                CreateResult("Cat Fact", "Get a random cat fact", "catfact"),
                CreateResult("Dog", "Get a random dog picture", "dog"),
                CreateResult("Text", "Transform text - Examples: text upper Hello, text reverse World", "text upper ")
            };
        }

        private List<Result> GetQuoteResults(CancellationToken token)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Get Random Quote",
                    SubTitle = "Press Enter to fetch an inspirational quote",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var quote = await _quoteService.GetRandomQuoteAsync(token);
                                if (quote != null && !string.IsNullOrEmpty(quote.QuoteText))
                                {
                                    var fullQuote = $"\"{quote.QuoteText}\" - {quote.QuoteAuthor ?? "Unknown"}";
                                    Application.Current?.Dispatcher.Invoke(() =>
                                    {
                                        Context?.API.ShowMsg("💭 Quote", fullQuote);
                                        try
                                        {
                                            Clipboard.SetText(fullQuote);
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                                        }
                                    });
                                }
                                else
                                {
                                    ShowError("Failed to fetch quote. Please try again.");
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // Operation was cancelled, do nothing
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error fetching quote: {ex.Message}", GetType());
                                // Suppress error popup to avoid duplicate notification after success
                            }
                        }, token);
                        return false;
                    }
                }
            };
        }

        private List<Result> GetJokeResults(CancellationToken token)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Get Random Joke",
                    SubTitle = "Press Enter to fetch a funny joke",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var joke = await _jokeService.GetRandomJokeAsync(token);
                                if (joke != null && !string.IsNullOrEmpty(joke.Setup))
                                {
                                    var fullJoke = $"{joke.Setup}\n\n{joke.Punchline}";
                                    Application.Current?.Dispatcher.Invoke(() =>
                                    {
                                        Context?.API.ShowMsg("😄 Joke", fullJoke);
                                        try
                                        {
                                            Clipboard.SetText(fullJoke);
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                                        }
                                    });
                                }
                                else
                                {
                                    ShowError("Failed to fetch joke. Please try again.");
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // Operation was cancelled, do nothing
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error fetching joke: {ex.Message}", GetType());
                                // Suppress error popup to avoid duplicate notification after success
                            }
                        }, token);
                        return false;
                    }
                }
            };
        }

        private List<Result> GetExchangeResults(string[] args, CancellationToken token)
        {
            if (args.Length < 2)
            {
                return new List<Result>
                {
                    new Result
                    {
                        Title = "💱 Exchange Rate Converter",
                        SubTitle = "Usage: exchange <from> <to> [amount] | Example: exchange USD EUR 100",
                        IcoPath = IconPath,
                        Action = _ =>
                        {
                            Context?.API.ChangeQuery($"{Context.CurrentPluginMetadata.ActionKeyword} exchange USD EUR 100", true);
                            return false;
                        }
                    },
                    CreateResult("💵 USD → EUR", "US Dollar to Euro (100)", "exchange USD EUR 100"),
                    CreateResult("💶 EUR → USD", "Euro to US Dollar (100)", "exchange EUR USD 100"),
                    CreateResult("💷 GBP → USD", "British Pound to US Dollar (100)", "exchange GBP USD 100"),
                    CreateResult("💴 USD → JPY", "US Dollar to Japanese Yen (100)", "exchange USD JPY 100"),
                    CreateResult("🇺🇦 UAH → USD", "Ukrainian Hryvnia to US Dollar (100)", "exchange UAH USD 100"),
                    CreateResult("🇺🇦 UAH → EUR", "Ukrainian Hryvnia to Euro (100)", "exchange UAH EUR 100")
                };
            }

            var from = args[0].ToUpperInvariant();
            var to = args[1].ToUpperInvariant();
            var amount = 1m;

            if (args.Length >= 3 && decimal.TryParse(args[2], out var parsedAmount))
            {
                amount = parsedAmount;
            }

            if (from.Length != 3 || to.Length != 3)
            {
                return new List<Result>
                {
                    CreateErrorResult("❌ Invalid Currency Code", "Currency codes must be 3 letters (e.g., USD, EUR, GBP)", "")
                };
            }

            return new List<Result>
            {
                new Result
                {
                    Title = $"Convert {amount} {from} to {to}",
                    SubTitle = "Press Enter to fetch current exchange rate",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var exchange = await _exchangeService.GetExchangeRateAsync(from, to, amount, token);
                                if (exchange.HasValue)
                                {
                                    var convertedAmount = exchange.Value.rate;
                                    var rate = convertedAmount / amount;
                                    var result = $"{amount} {from} = {convertedAmount:N2} {to}\nRate: 1 {from} = {rate:N6} {to}\nUpdated: {exchange.Value.date}";
                                    
                                    Application.Current?.Dispatcher.Invoke(() =>
                                    {
                                        Context?.API.ShowMsg("💱 Exchange Rate", result);
                                        try
                                        {
                                            Clipboard.SetText($"{amount} {from} = {convertedAmount:N2} {to}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                                        }
                                    });
                                }
                                else
                                {
                                    ShowError($"Currency not supported. {from} or {to} may not be available in the API.");
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // Operation was cancelled, do nothing
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error fetching exchange rate: {ex.Message}", GetType());
                                // Suppress error popup to avoid duplicate notification after success
                            }
                        }, token);
                        return false;
                    }
                }
            };
        }

        private List<Result> GetTimeResults(string[] args, CancellationToken token)
        {
            if (args.Length < 1)
            {
                return new List<Result>
                {
                    new Result
                    {
                        Title = "🌍 World Time",
                        SubTitle = "Usage: time <timezone> | Example: time Europe/London",
                        IcoPath = IconPath,
                        Action = _ =>
                        {
                            Context?.API.ChangeQuery($"{Context.CurrentPluginMetadata.ActionKeyword} time Europe/London", true);
                            return false;
                        }
                    },
                    CreateResult("🇬🇧 London", "Europe/London timezone", "time Europe/London"),
                    CreateResult("🇺🇸 New York", "America/New_York timezone", "time America/New_York"),
                    CreateResult("🇯🇵 Tokyo", "Asia/Tokyo timezone", "time Asia/Tokyo"),
                    CreateResult("🇦🇺 Sydney", "Australia/Sydney timezone", "time Australia/Sydney"),
                    CreateResult("🇩🇪 Berlin", "Europe/Berlin timezone", "time Europe/Berlin"),
                    CreateResult("🇨🇳 Shanghai", "Asia/Shanghai timezone", "time Asia/Shanghai"),
                    CreateResult("🇫🇷 Paris", "Europe/Paris timezone", "time Europe/Paris"),
                    CreateResult("🇮🇳 Mumbai", "Asia/Kolkata timezone", "time Asia/Kolkata")
                };
            }

            var location = string.Join("_", args);

            return new List<Result>
            {
                new Result
                {
                    Title = $"Get time for {location}",
                    SubTitle = "Press Enter to fetch current time",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var timeInfo = await _timeService.GetTimeAsync(location, token);
                                if (timeInfo != null && !string.IsNullOrEmpty(timeInfo.DateTime))
                                {
                                    var dateTime = DateTime.Parse(timeInfo.DateTime);
                                    var result = $"Time: {dateTime:HH:mm:ss}\nDate: {dateTime:dddd, MMMM dd, yyyy}\nTimezone: {timeInfo.Timezone}\nUTC Offset: {timeInfo.UtcOffset}";
                                    
                                    Application.Current?.Dispatcher.Invoke(() =>
                                    {
                                        Context?.API.ShowMsg($"🕒 Time in {timeInfo.Timezone}", result);
                                        try
                                        {
                                            Clipboard.SetText($"{dateTime:yyyy-MM-dd HH:mm:ss} {timeInfo.Timezone}");
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                                        }
                                    });
                                }
                                else
                                {
                                    ShowError("Failed to fetch time. Please check the timezone format.");
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // Operation was cancelled, do nothing
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error fetching time: {ex.Message}", GetType());
                                // Suppress error popup to avoid duplicate notification after success
                            }
                        }, token);
                        return false;
                    }
                }
            };
        }

        private List<Result> GetCatFactResults(CancellationToken token)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Get Random Cat Fact",
                    SubTitle = "Press Enter to fetch an interesting cat fact",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var fact = await _catFactService.GetRandomFactAsync(token);
                                if (fact != null && !string.IsNullOrEmpty(fact.Fact))
                                {
                                    Application.Current?.Dispatcher.Invoke(() =>
                                    {
                                        Context?.API.ShowMsg("🐱 Cat Fact", fact.Fact);
                                        try
                                        {
                                            Clipboard.SetText(fact.Fact);
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                                        }
                                    });
                                }
                                else
                                {
                                    ShowError("Failed to fetch cat fact. Please try again.");
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // Operation was cancelled, do nothing
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error fetching cat fact: {ex.Message}", GetType());
                                // Suppress error popup to avoid duplicate notification after success
                            }
                        }, token);
                        return false;
                    }
                }
            };
        }

        private List<Result> GetDogResults(CancellationToken token)
        {
            return new List<Result>
            {
                new Result
                {
                    Title = "Fetching dog picture...",
                    SubTitle = "Please wait",
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                var dog = await _dogService.GetRandomDogImageAsync(token);
                                if (dog != null && !string.IsNullOrEmpty(dog.Message))
                                {
                                    var imageUrl = dog.Message;
                                    Application.Current?.Dispatcher.Invoke(() =>
                                    {
                                        Context?.API.ChangeQuery(string.Empty, true);
                                        try
                                        {
                                            Process.Start(new ProcessStartInfo
                                            {
                                                FileName = imageUrl,
                                                UseShellExecute = true
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Log.Error($"Error opening dog image: {ex.Message}", GetType());
                                            ShowError("Failed to open dog image.");
                                        }
                                    });
                                }
                                else
                                {
                                    ShowError("Failed to fetch dog picture. Please try again.");
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                // Operation was cancelled, do nothing
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error fetching dog picture: {ex.Message}", GetType());
                                // Suppress error popup to avoid duplicate notification after success
                            }
                        }, token);
                        return false;
                    }
                }
            };
        }

        private List<Result> GetTextTransformResults(string[] args)
        {
            if (args.Length == 0)
            {
                var commands = _textActionService.GetAvailableCommands();
                return new List<Result>
                {
                    new Result
                    {
                        Title = "Text Transformations",
                        SubTitle = $"Available commands: {string.Join(", ", commands)}",
                        IcoPath = IconPath,
                        Action = _ =>
                        {
                            Context?.API.ChangeQuery($"{Context.CurrentPluginMetadata.ActionKeyword} text ", true);
                            return false;
                        }
                    }
                };
            }

            var parseResult = _textActionService.ParseTextCommand(args);

            if (!parseResult.success)
            {
                if (string.IsNullOrEmpty(parseResult.subcommand))
                {
                    var commands = _textActionService.GetAvailableCommands();
                    return new List<Result>
                    {
                        new Result
                        {
                            Title = "Invalid Text Command",
                            SubTitle = $"Available commands: {string.Join(", ", commands)}",
                            IcoPath = IconPath
                        }
                    };
                }
                else if (string.IsNullOrEmpty(parseResult.text))
                {
                    return new List<Result>
                    {
                        new Result
                        {
                            Title = $"Text {parseResult.subcommand}",
                            SubTitle = "Please provide text to transform",
                            IcoPath = IconPath,
                            Action = _ =>
                            {
                                Context?.API.ChangeQuery($"{Context.CurrentPluginMetadata.ActionKeyword} text {parseResult.subcommand} ", true);
                                return false;
                            }
                        }
                    };
                }
                else
                {
                    return new List<Result>
                    {
                        CreateErrorResult("Text Transform Error", parseResult.errorMessage ?? "Unknown error", parseResult.errorMessage ?? "")
                    };
                }
            }

            var transformResult = _textActionService.TransformText(parseResult.subcommand!, parseResult.text!, parseResult.format);

            if (!transformResult.success)
            {
                return new List<Result>
                {
                    CreateErrorResult("Text Transform Error", transformResult.errorMessage ?? "Unknown error", transformResult.errorMessage ?? "")
                };
            }

            var formatSuffix = parseResult.format switch
            {
                PowerToysTextActions.Parsing.OutputFormat.Markdown => " (Markdown)",
                PowerToysTextActions.Parsing.OutputFormat.Json => " (JSON)",
                PowerToysTextActions.Parsing.OutputFormat.Html => " (HTML)",
                _ => ""
            };

            var description = _textActionService.GetTransformationDescription(parseResult.subcommand!);

            return new List<Result>
            {
                new Result
                {
                    Title = transformResult.result!,
                    SubTitle = description + formatSuffix,
                    IcoPath = IconPath,
                    Action = _ =>
                    {
                        try
                        {
                            Clipboard.SetText(transformResult.result!);
                            return true;
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                            return false;
                        }
                    }
                }
            };
        }

        private Result CreateResult(string title, string subtitle, string command)
        {
            return new Result
            {
                Title = title,
                SubTitle = subtitle,
                IcoPath = IconPath,
                Action = _ =>
                {
                    Context?.API.ChangeQuery($"{Context.CurrentPluginMetadata.ActionKeyword} {command}", true);
                    return false;
                }
            };
        }

        private Result CreateErrorResult(string title, string subtitle, string copyText)
        {
            return new Result
            {
                Title = title,
                SubTitle = subtitle,
                IcoPath = IconPath,
                Action = _ =>
                {
                    try
                    {
                        Clipboard.SetText(copyText);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                    }
                    return true;
                }
            };
        }

        private void ShowResult(string title, string content)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Context?.API.ShowMsg(title, content);
            });
        }

        private void ShowError(string message)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                Context?.API.ShowMsg("❌ Error", message);
            });
        }

        /// <summary>
        /// Initialize the plugin with the given <see cref="PluginInitContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="PluginInitContext"/> for this plugin.</param>
        public void Init(PluginInitContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(Context.API.GetCurrentTheme());
        }

        /// <summary>
        /// Return a list context menu entries for a given <see cref="Result"/> (shown at the right side of the result).
        /// </summary>
        /// <param name="selectedResult">The <see cref="Result"/> for the list with context menu entries.</param>
        /// <returns>A list context menu entries.</returns>
        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (selectedResult.ContextData is string content && !string.IsNullOrEmpty(content))
            {
                return
                [
                    new ContextMenuResult
                    {
                        PluginName = Name,
                        Title = "Copy to clipboard (Ctrl+C)",
                        FontFamily = "Segoe MDL2 Assets",
                        Glyph = "\xE8C8", // Copy icon
                        AcceleratorKey = Key.C,
                        AcceleratorModifiers = ModifierKeys.Control,
                        Action = _ =>
                        {
                            try
                            {
                                Clipboard.SetText(content);
                                return true;
                            }
                            catch (Exception ex)
                            {
                                Log.Error($"Error copying to clipboard: {ex.Message}", GetType());
                                return false;
                            }
                        },
                    }
                ];
            }

            return [];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Wrapper method for <see cref="Dispose()"/> that dispose additional objects and events form the plugin itself.
        /// </summary>
        /// <param name="disposing">Indicate that the plugin is disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed || !disposing)
            {
                return;
            }

            if (Context?.API != null)
            {
                Context.API.ThemeChanged -= OnThemeChanged;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();

            Disposed = true;
        }

        private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/bored.light.png" : "Images/bored.dark.png";

        private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);

        public string GetTranslatedPluginTitle()
        {
            return Name;
        }

        public string GetTranslatedPluginDescription()
        {
            return Description;
        }
    }
}