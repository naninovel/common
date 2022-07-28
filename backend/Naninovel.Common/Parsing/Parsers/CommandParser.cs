using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing;

internal class CommandParser
{
    private readonly List<Parameter> parameters = new();
    private LineWalker walker = null!;
    private string identifier = null!;

    public Command Parse (LineWalker walker)
    {
        ResetState(walker);
        if (TryParseIdentifier()) ParseParameters();
        return new Command(identifier, parameters.ToArray());
    }

    private void ResetState (LineWalker walker)
    {
        this.walker = walker;
        parameters.Clear();
        identifier = "";
    }

    private bool TryParseIdentifier ()
    {
        if (!walker.Next(CommandId | Error, MissingCommandId, out var token))
            walker.AddError(MissingCommandTokens);
        else if (token.IsError(MissingCommandId)) walker.AddError(token);
        else identifier = walker.Extract(token);
        return !string.IsNullOrEmpty(identifier);
    }

    private void ParseParameters () { }
}
