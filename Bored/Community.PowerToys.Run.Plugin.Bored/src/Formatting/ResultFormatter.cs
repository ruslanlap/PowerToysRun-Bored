using System;
using System.Text.Json;
using PowerToysTextActions.Parsing;

namespace PowerToysTextActions.Formatting;

public class ResultFormatter
{
    public string Format(string text, OutputFormat format)
    {
        ArgumentNullException.ThrowIfNull(text);

        return format switch
        {
            OutputFormat.Plain => text,
            OutputFormat.Markdown => FormatMarkdown(text),
            OutputFormat.Json => FormatJson(text),
            OutputFormat.Html => FormatHtml(text),
            _ => text
        };
    }

    private string FormatMarkdown(string text)
    {
        return $"**{text}**";
    }

    private string FormatJson(string text)
    {
        var obj = new { result = text };
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions 
        { 
            WriteIndented = false 
        });
    }

    private string FormatHtml(string text)
    {
        return $"<p>{System.Net.WebUtility.HtmlEncode(text)}</p>";
    }
}
