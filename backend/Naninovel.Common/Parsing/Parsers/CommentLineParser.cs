using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommentLineParser : LineParser<CommentLine>
{
    protected override void Parse (CommentLine line)
    {
        if (TryNext(CommentText, out var token))
            line.CommentText.Assign(Extract(token), token.StartIndex);
    }

    protected override void ClearLine (CommentLine line)
    {
        ClearLineText(line.CommentText);
    }
}