using System;
using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing;

internal class CommandParser
{
    private static readonly Command emptyBody = new(PlainText.Empty);
    private static readonly MixedValue emptyValue = new(Array.Empty<IValueComponent>());
    private readonly MixedValueParser valueParser = new(true);
    private readonly List<Parameter> parameters = new();
    private LineWalker walker = null!;
    private Command commandBody = emptyBody;
    private PlainText commandId = PlainText.Empty;
    private MixedValue paramValue = emptyValue;
    private PlainText? paramId;

    public Command Parse (LineWalker walker)
    {
        ResetCommandState(walker);
        if (TryCommandId())
            while (TryParameter())
                continue;
        return commandBody;
    }

    private void ResetCommandState (LineWalker walker)
    {
        this.walker = walker;
        commandId = PlainText.Empty;
        commandBody = emptyBody;
        parameters.Clear();
        ResetParameterState();
    }

    private void ResetParameterState ()
    {
        paramId = null;
        paramValue = emptyValue;
    }

    private bool TryCommandId ()
    {
        if (!walker.Next(CommandId | TokenType.Error, MissingCommandId, out var token))
            walker.Error(MissingCommandTokens);
        else if (token.IsError(MissingCommandId)) walker.Error(token);
        else ParseCommandId(token);
        return commandId != PlainText.Empty;
    }

    private void ParseCommandId (Token commandIdToken)
    {
        commandId = walker.Extract(commandIdToken);
        walker.Associate(commandId, commandIdToken);
    }

    private bool TryParameter ()
    {
        walker.Next(out var token);

        switch (token.Type)
        {
            case ParamId:
                ParseParameterId(token);
                return true;
            case TokenType.Expression:
                valueParser.AddExpressionToken(token);
                return true;
            case TextIdBody:
                valueParser.AddTextIdBodyToken(token);
                return true;
            case TextId:
                valueParser.AddTextIdToken(token);
                return true;
            case ParamValue:
                ParseParameterValue(token);
                return true;
            case NamelessParam:
            case NamedParam:
                ParseParameter(token);
                return true;
            case CommandBody:
                ParseCommandBody(token);
                return false;
            case TokenType.Error:
                walker.Error(token);
                return true;
            default: return true;
        }
    }

    private void ParseParameterId (Token paramIdToken)
    {
        paramId = walker.Extract(paramIdToken);
        walker.Associate(paramId, paramIdToken);
    }

    private void ParseParameterValue (Token valueToken)
    {
        paramValue = new MixedValue(valueParser.Parse(valueToken, walker));
        walker.Associate(paramValue, valueToken);
        walker.Identify(paramValue);
    }

    private void ParseParameter (Token paramToken)
    {
        var param = new Parameter(paramId, paramValue);
        parameters.Add(param);
        walker.Associate(param, paramToken);
        ResetParameterState();
    }

    private void ParseCommandBody (Token bodyToken)
    {
        commandBody = new Command(commandId, parameters.ToArray());
        walker.Associate(commandBody, bodyToken);
    }
}
