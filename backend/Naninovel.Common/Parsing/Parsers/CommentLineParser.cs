using System.Collections.Generic;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommentLineParser
{
    private readonly LineWalker walker;
    private PlainText comment = PlainText.Empty;

    public CommentLineParser (ParseHandlers handlers)
    {
        walker = new(handlers);
    }

    public CommentLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        Reset(lineText, tokens);
        if (!walker.Next(LineId, out _))
            walker.Error(MissingLineId);
        else if (walker.Next(CommentText, out var commentToken))
            ParseComment(commentToken);
        return new CommentLine(comment);
    }

    private void Reset (string lineText, IReadOnlyList<Token> tokens)
    {
        comment = PlainText.Empty;
        walker.Reset(lineText, tokens);
    }

    private void ParseComment (Token commentToken)
    {
        comment = walker.Extract(commentToken);
        walker.Associate(comment, commentToken);
    }
}
