using System;

namespace PowerToysTextActions.Models;

public class Result
{
    public string Title { get; set; } = string.Empty;
    public string SubTitle { get; set; } = string.Empty;
    public string? CopyText { get; set; }
    public Action? Action { get; set; }
    public int Score { get; set; }
}
