using System;
using System.Collections.Generic;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommandLineParser
{
    private readonly LineWalker walker = new();
    private readonly CommandParser commandParser = new();
    private Command command = null!;

    public CommandLine Parse (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors = null, TokenResolver resolver = null)
    {
        ResetState(lineText, tokens, errors, resolver);
        if (!walker.Next(LineId, out _))
            walker.AddError(MissingLineId);
        else command = commandParser.Parse(walker);
        return new CommandLine(command);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens,
        ICollection<ParseError> errors, TokenResolver resolver)
    {
        command = new(PlainText.Empty, Array.Empty<Parameter>());
        walker.Reset(lineText, tokens, errors, resolver);
    }
}
