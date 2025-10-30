using System;
using System.Globalization;
using System.Text;

namespace PowerToysTextActions.Transformations;

public class CapitalizeTransformation : ITextTransformation
{
    public string Description => "Capitalize Each Word (Title Case)";

    public string Transform(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var textInfo = CultureInfo.InvariantCulture.TextInfo;
        return textInfo.ToTitleCase(input.ToLowerInvariant());
    }
}
