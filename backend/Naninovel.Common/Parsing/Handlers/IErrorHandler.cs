namespace Naninovel.Parsing;

/// <summary>
/// Handles errors occured while parsing.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Handles provided parsing error.
    /// </summary>
    void HandleError (ParseError error);
}
