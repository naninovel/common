using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class LabelLineParser (ParseHandlers handlers)
{
    private readonly LineWalker walker = new(handlers);
    private PlainText label = PlainText.Empty;

    public LabelLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        Reset(lineText, tokens);
        if (!walker.Next(LineId, out _))
            walker.Error(MissingLineId);
        else if (walker.Next(MissingLabel, out var missingError))
            walker.Error(missingError);
        else if (walker.Next(LabelText, out var labelToken))
            if (walker.Next(SpaceInLabel, out var spaceError))
                walker.Error(spaceError);
            else ParseLabel(labelToken);
        return new LabelLine(label, walker.GetIndent());
    }

    private void Reset (string lineText, IReadOnlyList<Token> tokens)
    {
        label = PlainText.Empty;
        walker.Reset(lineText, tokens);
    }

    private void ParseLabel (Token labelToken)
    {
        label = walker.Extract(labelToken);
        walker.Associate(label, labelToken);
    }
}
