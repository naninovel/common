using System.Collections.Generic;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommentLineParser
{
    private readonly TokenWalker walker = new();
    private string comment = "";

    public CommentLine Parse (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null)
    {
        ResetState(lineText, tokens, errors);
        if (!walker.Next(LineId, out _))
            walker.AddError(MissingLineId);
        if (walker.Next(CommentText, out var token))
            comment = walker.Extract(token);
        return new CommentLine(comment);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null)
    {
        comment = "";
        walker.Reset(lineText, tokens, errors);
    }
}
