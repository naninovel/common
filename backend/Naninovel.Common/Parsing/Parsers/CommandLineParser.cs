﻿using static Naninovel.Parsing.ParsingErrors;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class CommandLineParser (ParseHandlers handlers)
{
    private static readonly Command emptyBody = new(PlainText.Empty, Array.Empty<Parameter>());
    private readonly CommandParser commandParser = new();
    private readonly LineWalker walker = new(handlers);
    private Command command = emptyBody;

    public CommandLine Parse (string lineText, IReadOnlyList<Token> tokens)
    {
        Reset(lineText, tokens);
        if (!walker.Next(LineId, out _))
            walker.Error(MissingLineId);
        else command = commandParser.Parse(walker);
        return new CommandLine(command);
    }

    private void Reset (string lineText, IReadOnlyList<Token> tokens)
    {
        command = emptyBody;
        walker.Reset(lineText, tokens);
    }
}
