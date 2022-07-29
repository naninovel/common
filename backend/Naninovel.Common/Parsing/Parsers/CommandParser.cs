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
    private string commandId = null!;
    private string paramId;

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
        commandId = "";
        parameters.Clear();
        ResetParameterState();
    }

    private void ResetParameterState ()
    {
        paramId = null;
        value.Clear();
    }

    private bool TryCommandId ()
    {
        if (!walker.Next(CommandId | Error, MissingCommandId, out var token))
            walker.AddError(MissingCommandTokens);
        else if (token.IsError(MissingCommandId)) walker.AddError(token);
        else commandId = walker.Extract(token);
        return !string.IsNullOrEmpty(commandId);
    }

    private bool TryParameter ()
    {
        walker.Next(out var token);

        switch (token.Type)
        {
            case ParamId:
                paramId = walker.Extract(token);
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
                walker.AddError(token);
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
