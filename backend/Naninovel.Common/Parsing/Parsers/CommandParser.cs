using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing;

public class CommandParser : LineParser
{
    protected override LineParseState State => state;

    private readonly Stack<Command> commandPool = new();
    private readonly ParameterParser parameterParser = new();
    private readonly LineParseState bodyParseState = new();
    private readonly Lexer bodyParseLexer = new();
    private readonly List<Token> bodyParseTokens = new();
    private readonly List<ParseError> bodyParseErrors = new();

    private LineParseState state;

    public void ParseNext (LineParseState state, Command command)
    {
        this.state = state;
        if (!TryCommandId(command)) return;
        parameterParser.ParseNext(state, command);
    }

    public void ParseBody (string bodyText, Command command)
    {
        bodyParseTokens.Clear();
        bodyParseErrors.Clear();
        bodyParseLexer.TokenizeCommandBody(bodyText, bodyParseTokens);
        bodyParseState.Reset(bodyText, bodyParseTokens, bodyParseErrors);
        ParseNext(bodyParseState, command);
    }

    public Command GetCommand ()
    {
        return commandPool.Count > 0 ? commandPool.Pop() : new Command();
    }

    public void ReturnCommand (Command command)
    {
        ClearCommand(command);
        commandPool.Push(command);
    }

    public void ClearCommand (Command command)
    {
        ClearLineContent(command);
        ClearLineText(command.Identifier);
        foreach (var parameter in command.Parameters)
            parameterParser.ReturnParameter(parameter);
        command.Parameters.Clear();
    }

    private bool TryCommandId (Command command)
    {
        if (!TryNext(CommandId | Error, MissingCommandId, out var token))
            AddError(MissingCommandTokens);
        else if (token.IsError(MissingCommandId)) AddError(token);
        else command.Identifier.Assign(Extract(token), token.StartIndex);
        return !string.IsNullOrEmpty(command.Identifier);
    }
}