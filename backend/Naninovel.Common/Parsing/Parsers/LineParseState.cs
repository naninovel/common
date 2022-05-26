using System.Collections.Generic;

namespace Naninovel.Parsing;

public class LineParseState
{
    public int Index;
    public string LineText { get; private set; }
    public IReadOnlyList<Token> Tokens { get; private set; }
    public ICollection<ParseError> Errors { get; private set; }

    public void Reset (string lineText, IReadOnlyList<Token> tokens, ICollection<ParseError> errors)
    {
        Index = -1;
        LineText = lineText;
        Tokens = tokens;
        Errors = errors;
    }
}