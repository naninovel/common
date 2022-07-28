namespace Naninovel.Parsing;

/// <summary>
/// Stores human-readable description of parsing errors.
/// </summary>
public static class ParsingErrors
{
    public const string MissingLineId = "Line identifier is missing.";
    public const string MissingCommandTokens = "Failed to find both command ID and the related error tokens.";
}
