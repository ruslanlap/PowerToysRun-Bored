using System;
using System.Text;

namespace PowerToysTextActions.Transformations;

public class ToggleCaseTransformation : ITextTransformation
{
    public string Description => "Toggle CASE of each character";

    public string Transform(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var result = new StringBuilder(input.Length);

        foreach (char c in input)
        {
            if (char.IsUpper(c))
            {
                result.Append(char.ToLowerInvariant(c));
            }
            else if (char.IsLower(c))
            {
                result.Append(char.ToUpperInvariant(c));
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
