namespace Naninovel.Parsing;

/// <summary>
/// Optional handlers (hooks) for <see cref="ScriptParser"/>.
/// </summary>
public class ParseHandlers
{
    // TODO: Change to init-only on runtime upgrade.
    /// <inheritdoc cref="IErrorHandler"/>
    public IErrorHandler? ErrorHandler { get; set; }
    /// <inheritdoc cref="IRangeAssociator"/>
    public IRangeAssociator? RangeAssociator { get; set; }
    /// <inheritdoc cref="ITextIdentifier"/>
    public ITextIdentifier? TextIdentifier { get; set; }
}
