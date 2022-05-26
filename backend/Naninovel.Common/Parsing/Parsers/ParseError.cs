namespace Naninovel.Parsing;

public class ParseError
{
    public string Message { get; }
    public int StartIndex { get; }
    public int Length { get; }
    public int EndIndex => StartIndex + Length - 1;

    public ParseError (string message)
    {
        Message = message;
    }

    public ParseError (Token token)
    {
        Message = LexingErrors.GetFor(token.Error);
        StartIndex = token.StartIndex;
        Length = token.Length;
    }
}