using System;
using System.Text;
using System.Text.RegularExpressions;

namespace PowerToysTextActions.Transformations;

public class SentenceCaseTransformation : ITextTransformation
{
    public string Description => "Convert to Sentence case";

    public string Transform(string input)
    {
        ArgumentNullException.ThrowIfNull(input);
        
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        input = input.ToLowerInvariant();

        var result = new StringBuilder(input.Length);
        bool capitalizeNext = true;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];
            
            if (capitalizeNext && char.IsLetter(c))
            {
                result.Append(char.ToUpperInvariant(c));
                capitalizeNext = false;
            }
            else
            {
                result.Append(c);
            }

            if (c == '.' || c == '!' || c == '?')
            {
                capitalizeNext = true;
            }
        }

        return result.ToString();
    }
}
