namespace Naninovel.Parsing;

/// <summary>
/// An error occured while parsing a script text line.
/// </summary>
public class ParseError (string message, int startIndex, int length)
{
    /// <summary>
    /// Human-readable description of the error.
    /// </summary>
    public string Message { get; } = message;
    /// <summary>
    /// Character index inside the parsed text line associated with the start of the error.
    /// </summary>
    public int StartIndex { get; } = startIndex;
    /// <summary>
    /// Character length of the parsed text line area associated with the error.
    /// </summary>
    public int Length { get; } = length;
    /// <summary>
    /// Character index inside the parsed text line associated with the end of the error.
    /// </summary>
    public int EndIndex => StartIndex + Length - 1;

    public ParseError (Token token) : this(LexingErrors.GetFor(token.Error), token.Start, token.Length) { }
}
