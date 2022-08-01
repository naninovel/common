using System.Collections.Generic;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class LabelLineParser
{
    private readonly LineWalker walker;
    private PlainText label = null!;

    public LabelLineParser (IErrorHandler errorHandler = null, IAssociator associator = null)
    {
        walker = new(errorHandler, associator);
    }

    public LabelLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        ResetState(lineText, tokens);
        if (!walker.Next(LineId, out _))
            walker.Error(MissingLineId);
        else if (walker.Next(MissingLabel, out var missingError))
            walker.Error(missingError);
        else if (walker.Next(LabelText, out var labelToken))
            if (walker.Next(SpaceInLabel, out var spaceError))
                walker.Error(spaceError);
            else ParseLabel(labelToken);
        return new LabelLine(label);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens)
    {
        label = PlainText.Empty;
        walker.Reset(lineText, tokens);
    }

    private void ParseLabel (Token labelToken)
    {
        label = new(walker.Extract(labelToken));
        walker.Associate(label, labelToken);
    }
}
