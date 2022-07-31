using System.Collections.Generic;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class LabelLineParser
{
    private readonly LineWalker walker = new();
    private PlainText label = null!;

    public LabelLine Parse (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null, TokenResolver resolver = null)
    {
        ResetState(lineText, tokens, errors, resolver);
        if (!walker.Next(LineId, out _))
            walker.AddError(MissingLineId);
        else if (walker.Next(MissingLabel, out var missingError))
            walker.AddError(missingError);
        else if (walker.Next(LabelText, out var labelToken))
            if (walker.Next(SpaceInLabel, out var spaceError))
                walker.AddError(spaceError);
            else ParseLabel(labelToken);
        return new LabelLine(label);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors, TokenResolver resolver)
    {
        label = PlainText.Empty;
        walker.Reset(lineText, tokens, errors, resolver);
    }

    private void ParseLabel (Token labelToken)
    {
        label = new(walker.Extract(labelToken));
        walker.Associate(label, labelToken);
    }
}
