namespace Naninovel.Parsing;

/// <summary>
/// Implementation is able to handle errors occured while parsing.
/// </summary>
public interface IErrorHandler
{
    /// <summary>
    /// Handles provided parsing error.
    /// </summary>
    void HandleError (ParseError error);
}
