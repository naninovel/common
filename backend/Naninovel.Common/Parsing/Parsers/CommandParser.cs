using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing;

internal class CommandParser
{
    private readonly MixedValueParser valueParser = new(true);
    private readonly List<Parameter> parameters = new();
    private readonly List<IMixedValue> value = new();
    private LineWalker walker = null!;
    private PlainText commandId = null!;
    private PlainText paramId;

    public Command Parse (LineWalker walker)
    {
        ResetCommandState(walker);
        if (TryCommandId())
            while (TryParameter())
                continue;
        return new Command(commandId, parameters.ToArray());
    }

    private void ResetCommandState (LineWalker walker)
    {
        this.walker = walker;
        commandId = PlainText.Empty;
        parameters.Clear();
        ResetParameterState();
    }

    private void ResetParameterState ()
    {
        paramId = null;
        value.Clear();
        valueParser.ClearAddedExpressions();
    }

    private bool TryCommandId ()
    {
        if (!walker.Next(CommandId | Error, MissingCommandId, out var token))
            walker.Error(MissingCommandTokens);
        else if (token.IsError(MissingCommandId)) walker.Error(token);
        else commandId = new(walker.Extract(token));
        return commandId != PlainText.Empty;
    }

    private bool TryParameter ()
    {
        walker.Next(out var token);

        switch (token.Type)
        {
            case ParamId:
                paramId = new(walker.Extract(token));
                return true;
            case TokenType.Expression:
                valueParser.AddExpressionToken(token);
                return true;
            case ParamValue:
                value.AddRange(valueParser.Parse(token, walker));
                return true;
            case NamelessParam:
            case NamedParam:
                AddParameter();
                return true;
            case CommandBody:
                return false;
            case Error:
                walker.Error(token);
                return true;
            default: return true;
        }
    }

    private void AddParameter ()
    {
        parameters.Add(new(paramId, value.ToArray()));
        ResetParameterState();
    }
}
