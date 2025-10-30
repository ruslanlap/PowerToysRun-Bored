using System;
using System.Linq;

namespace PowerToysTextActions.Parsing;

public class QueryParser
{
    public ParsedQuery Parse(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        query = query.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            return new ParsedQuery(null, null, OutputFormat.Plain);
        }

        var parts = query.Split(' ', 2, StringSplitOptions.TrimEntries);
        
        if (parts.Length == 0)
        {
            return new ParsedQuery(null, null, OutputFormat.Plain);
        }

        var subcommand = parts[0].ToLowerInvariant();
        string? text = parts.Length > 1 ? parts[1] : null;

        OutputFormat format = OutputFormat.Plain;

        if (!string.IsNullOrWhiteSpace(text))
        {
            var textParts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
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

                    text = string.Join(" ", textParts.Take(textParts.Length - 1)).Trim();
                }
            }
        }

        var validSubcommands = new[] { "upper", "lower", "capitalize", "sentence", "toggle" };
        
        if (!validSubcommands.Contains(subcommand))
        {
            return new ParsedQuery(null, null, OutputFormat.Plain);
        }

        return new ParsedQuery(subcommand, text, format);
    }
}

public record ParsedQuery(string? Subcommand, string? Text, OutputFormat Format);

public enum OutputFormat
{
    Plain,
    Markdown,
    Json,
    Html
}
