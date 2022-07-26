namespace Naninovel.Parsing;

/// <summary>
/// An error occured while parsing a script text line.
/// </summary>
public class ParseError
{
    /// <summary>
    /// Human-readable description of the error.
    /// </summary>
    public string Message { get; }
    /// <summary>
    /// Character index inside the parsed text line associated with the start of the error.
    /// </summary>
    public int StartIndex { get; }
    /// <summary>
    /// Character length of the parsed text line area associated with the error.
    /// </summary>
    public int Length { get; }
    /// <summary>
    /// Character index inside the parsed text line associated with the end of the error.
    /// </summary>
    public int EndIndex => StartIndex + Length - 1;

    public ParseError (string message, int startIndex, int length)
    {
        Message = message;
        StartIndex = startIndex;
        Length = length;
    }

    public ParseError (Token token)
    {
        Message = LexingErrors.GetFor(token.Error);
        StartIndex = token.StartIndex;
        Length = token.Length;
    }
}
