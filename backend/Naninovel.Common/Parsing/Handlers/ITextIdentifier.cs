namespace Naninovel.Parsing;

/// <summary>
/// Handles identification of text parameter values (<see cref="IdentifiedText"/>).
/// </summary>
public interface ITextIdentifier
{
    /// <summary>
    /// Handles association between specified ID and plain text.
    /// </summary>
    void Identify (string id, string text);
}
