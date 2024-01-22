using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommentLineParser (ParseHandlers handlers)
{
    private readonly LineWalker walker = new(handlers);
    private PlainText comment = PlainText.Empty;

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
