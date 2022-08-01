using System;
using System.Collections.Generic;
using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommandLineParser
{
    private readonly CommandParser commandParser = new();
    private readonly LineWalker walker;
    private Command command = null!;

    public CommandLineParser (IErrorHandler errorHandler = null, IAssociator associator = null)
    {
        walker = new(errorHandler, associator);
    }

    public CommandLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        ResetState(lineText, tokens);
        if (!walker.Next(LineId, out _))
            walker.Error(MissingLineId);
        else command = commandParser.Parse(walker);
        return new CommandLine(command);
    }

    private void ResetState (string lineText, IReadOnlyList<Token> tokens)
    {
        command = new(PlainText.Empty, Array.Empty<Parameter>());
        walker.Reset(lineText, tokens);
    }
}
