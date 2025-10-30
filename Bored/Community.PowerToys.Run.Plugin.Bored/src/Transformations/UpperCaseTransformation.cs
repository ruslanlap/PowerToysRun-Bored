using System;

namespace PowerToysTextActions.Transformations;

public class UpperCaseTransformation : ITextTransformation
{
    public string Description => "Convert to UPPER CASE";

    public string Transform(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.ToUpperInvariant();
    }
}
