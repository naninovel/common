namespace Naninovel.Parsing;

/// <summary>
/// Collects errors occured while parsing.
/// </summary>
public class ErrorCollector : List<ParseError>, IErrorHandler
{
    public void HandleError (ParseError error)
    {
        Add(error);
    }
}
