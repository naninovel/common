using System.Collections.Generic;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class LabelLineParser
{
    private readonly TokenWalker walker = new();
    private string label = "";

    public LabelLine Parse (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null)
    {
        ResetState(lineText, tokens, errors);
        if (!walker.Next(LineId, out _))
            walker.AddError(MissingLineId);
        else if (walker.Next(MissingLabel, out var missingError))
            walker.AddError(missingError);
        else if (walker.Next(LabelText, out var labelToken))
            if (walker.Next(SpaceInLabel, out var spaceError))
                walker.AddError(spaceError);
            else label = walker.Extract(labelToken);
        return new LabelLine(label);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null)
    {
        label = "";
        walker.Reset(lineText, tokens, errors);
    }
}
