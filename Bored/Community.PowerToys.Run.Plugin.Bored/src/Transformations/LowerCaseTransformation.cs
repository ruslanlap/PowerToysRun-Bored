using System;

namespace PowerToysTextActions.Transformations;

public class LowerCaseTransformation : ITextTransformation
{
    public string Description => "Convert to lower case";

    public string Transform(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        return input.ToLowerInvariant();
    }
}
