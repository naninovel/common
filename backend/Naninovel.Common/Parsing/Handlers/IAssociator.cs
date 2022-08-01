namespace Naninovel.Parsing;

/// <summary>
/// Implementation is able to handle associations between lex tokens and parse models.
/// </summary>
public interface IAssociator
{
    /// <summary>
    /// Handles association between the provided component and token.
    /// </summary>
    void Associate (ILineComponent component, Token token);
}
