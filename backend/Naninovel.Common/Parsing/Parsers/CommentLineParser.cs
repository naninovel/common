using System.Collections.Generic;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommentLineParser
{
    private readonly LineWalker walker = new();
    private PlainText comment = null!;

    public CommentLine Parse (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null, TokenResolver resolver = null)
    {
        ResetState(lineText, tokens, errors, resolver);
        if (!walker.Next(LineId, out _))
            walker.AddError(MissingLineId);
        else if (walker.Next(CommentText, out var commentToken))
            ParseComment(commentToken);
        return new CommentLine(comment);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors, TokenResolver resolver)
    {
        comment = PlainText.Empty;
        walker.Reset(lineText, tokens, errors, resolver);
    }

    private void ParseComment (Token commentToken)
    {
        comment = new(walker.Extract(commentToken));
        walker.Associate(comment, commentToken);
    }
}
