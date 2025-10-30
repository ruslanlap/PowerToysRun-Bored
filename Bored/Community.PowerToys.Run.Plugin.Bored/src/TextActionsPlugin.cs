using System;
using System.Collections.Generic;
using PowerToysTextActions.Formatting;
using PowerToysTextActions.Models;
using PowerToysTextActions.Parsing;
using PowerToysTextActions.Transformations;

namespace PowerToysTextActions;

public class TextActionsPlugin
{
    private readonly QueryParser _parser;
    private readonly ResultFormatter _formatter;
    private readonly Dictionary<string, ITextTransformation> _transformations;

    public TextActionsPlugin()
    {
        _parser = new QueryParser();
        _formatter = new ResultFormatter();
        _transformations = new Dictionary<string, ITextTransformation>
        {
            ["upper"] = new UpperCaseTransformation(),
            ["lower"] = new LowerCaseTransformation(),
            ["capitalize"] = new CapitalizeTransformation(),
            ["sentence"] = new SentenceCaseTransformation(),
            ["toggle"] = new ToggleCaseTransformation()
        };
    }

    public List<Result> Query(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        var results = new List<Result>();
        var parsed = _parser.Parse(query);

        if (string.IsNullOrWhiteSpace(parsed.Subcommand))
        {
            results.Add(new Result
            {
                Title = "Text Actions",
                SubTitle = "Available commands: upper, lower, capitalize, sentence, toggle",
                Score = 100
            });
            return results;
        }

        if (string.IsNullOrWhiteSpace(parsed.Text))
        {
            results.Add(new Result
            {
                Title = $"Text Actions - {parsed.Subcommand}",
                SubTitle = "Please provide text to transform",
                Score = 100
            });
            return results;
        }

        if (!_transformations.TryGetValue(parsed.Subcommand, out var transformation))
        {
            results.Add(new Result
            {
                Title = "Invalid command",
                SubTitle = "Available commands: upper, lower, capitalize, sentence, toggle",
                Score = 100
            });
            return results;
        }

        try
        {
            var transformed = transformation.Transform(parsed.Text);
            var formatted = _formatter.Format(transformed, parsed.Format);

            var formatSuffix = parsed.Format switch
            {
                OutputFormat.Markdown => " (Markdown)",
                OutputFormat.Json => " (JSON)",
                OutputFormat.Html => " (HTML)",
                _ => ""
            };

            results.Add(new Result
            {
                Title = formatted,
                SubTitle = transformation.Description + formatSuffix,
                CopyText = formatted,
                Score = 100,
                Action = () => CopyToClipboard(formatted)
            });
        }
        catch (Exception ex)
        {
            results.Add(new Result
            {
                Title = "Error",
                SubTitle = $"Failed to transform text: {ex.Message}",
                Score = 100
            });
        }

        return results;
    }

    private void CopyToClipboard(string text)
    {
    }
}
