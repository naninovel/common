using System.Collections.Generic;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommentLineParser
{
    private readonly LineWalker walker;
    private PlainText comment = null!;

    public CommentLineParser (IErrorHandler errorHandler = null, IAssociator associator = null)
    {
        walker = new(errorHandler, associator);
    }

    public CommentLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        ResetState(lineText, tokens);
        if (!walker.Next(LineId, out _))
            walker.Error(MissingLineId);
        else if (walker.Next(CommentText, out var commentToken))
            ParseComment(commentToken);
        return new CommentLine(comment);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens)
    {
        comment = PlainText.Empty;
        walker.Reset(lineText, tokens);
    }

    private void ParseComment (Token commentToken)
    {
        comment = new(walker.Extract(commentToken));
        walker.Associate(comment, commentToken);
    }
}
