using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using PowerToysTextActions.Transformations;
using PowerToysTextActions.Parsing;

namespace Community.PowerToys.Run.Plugin.Bored.Services
{
    public class TextActionService
    {
        private readonly Dictionary<string, ITextTransformation> _transformations;

        public TextActionService()
        {
            _transformations = new Dictionary<string, ITextTransformation>
            {
                ["upper"] = new UpperCaseTransformation(),
                ["lower"] = new LowerCaseTransformation(),
                ["capitalize"] = new CapitalizeTransformation(),
                ["sentence"] = new SentenceCaseTransformation(),
                ["toggle"] = new ToggleCaseTransformation()
            };
        }

        public (bool success, string? subcommand, string? text, OutputFormat format, string? errorMessage) ParseTextCommand(string[] args)
        {
            if (args.Length == 0)
            {
                return (false, null, null, OutputFormat.Plain, "No subcommand provided");
            }

            var subcommand = args[0].ToLowerInvariant();
            
            if (!_transformations.ContainsKey(subcommand))
            {
                return (false, subcommand, null, OutputFormat.Plain, $"Unknown text subcommand: {subcommand}");
            }

            if (args.Length == 1)
            {
                return (false, subcommand, null, OutputFormat.Plain, "No text provided");
            }

            var textParts = args.Skip(1).ToArray();
            var format = OutputFormat.Plain;
            
            if (textParts.Length > 0)
            {
                var lastPart = textParts[^1].ToLowerInvariant();
                
                if (lastPart == "md" || lastPart == "json" || lastPart == "html")
                {
                    format = lastPart switch
                    {
                        "md" => OutputFormat.Markdown,
                        "json" => OutputFormat.Json,
                        "html" => OutputFormat.Html,
                        _ => OutputFormat.Plain
                    };

                    textParts = textParts.Take(textParts.Length - 1).ToArray();
                }
            }

            var text = string.Join(" ", textParts).Trim();
            
            if (string.IsNullOrWhiteSpace(text))
            {
                return (false, subcommand, null, format, "No text provided");
            }

            return (true, subcommand, text, format, null);
        }

        public (bool success, string? result, string? errorMessage) TransformText(string subcommand, string text, OutputFormat format)
        {
            try
            {
                if (!_transformations.TryGetValue(subcommand, out var transformation))
                {
                    return (false, null, $"Unknown transformation: {subcommand}");
                }

                var transformed = transformation.Transform(text);
                var formatted = FormatOutput(transformed, format);
                
                return (true, formatted, null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Error transforming text: {ex.Message}");
            }
        }

        public string GetTransformationDescription(string subcommand)
        {
            if (_transformations.TryGetValue(subcommand, out var transformation))
            {
                return transformation.Description;
            }
            return "Text transformation";
        }

        public string[] GetAvailableCommands()
        {
            return _transformations.Keys.ToArray();
        }

        private string FormatOutput(string text, OutputFormat format)
        {
            return format switch
            {
                OutputFormat.Markdown => $"**{text}**",
                OutputFormat.Json => JsonSerializer.Serialize(new { result = text }),
                OutputFormat.Html => $"<p>{WebUtility.HtmlEncode(text)}</p>",
                _ => text
            };
        }
    }
}
