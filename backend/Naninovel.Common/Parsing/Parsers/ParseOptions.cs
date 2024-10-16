namespace Naninovel.Parsing;

/// <summary>
/// Preferences for NaniScript parsing operation.
/// </summary>
public class ParseOptions
{
    /// <summary>
    /// Control symbols identifying NaniScript lexical artifacts.
    /// </summary>
    public ISyntax Syntax { get; set; } = Parsing.Syntax.Default;
    /// <summary>
    /// Parsing hooks.
    /// </summary>
    public ParseHandlers Handlers { get; set; } = new();
}
