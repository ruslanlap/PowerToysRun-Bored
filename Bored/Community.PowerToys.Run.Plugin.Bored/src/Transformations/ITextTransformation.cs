namespace PowerToysTextActions.Transformations;

public interface ITextTransformation
{
    string Transform(string input);
    string Description { get; }
}
