using System;
using System.Collections.Generic;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommandLineParser
{
    private readonly LineWalker walker = new();
    private readonly CommandParser commandParser = new();
    private Command command = new("", Array.Empty<Parameter>());

    public CommandLine Parse (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null)
    {
        ResetState(lineText, tokens, errors);
        if (!walker.Next(LineId, out _))
            walker.AddError(MissingLineId);
        else command = commandParser.Parse(walker);
        return new CommandLine(command);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null)
    {
        command = new("", Array.Empty<Parameter>());
        walker.Reset(lineText, tokens, errors);
    }
}
