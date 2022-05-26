using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class LabelLineParser : LineParser<LabelLine>
{
    protected override void Parse (LabelLine line)
    {
        if (!TryNext(LabelText, out var token)) AddError(new Token(MissingLabel, 0, 1));
        else line.LabelText.Assign(Extract(token), token.StartIndex);
        if (TryNext(SpaceInLabel, out token)) AddError(token);
    }

    protected override void ClearLine (LabelLine line)
    {
        ClearLineText(line.LabelText);
    }
}