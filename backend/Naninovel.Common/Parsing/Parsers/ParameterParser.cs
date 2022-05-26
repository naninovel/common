using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

internal class ParameterParser : LineParser
{
    protected override LineParseState State => state;

    private readonly Stack<Parameter> parameterPool = new();
    private readonly List<Token> lastExpressions = new();

    private LineParseState state;
    private Token lastId, lastValue;

    public void ParseNext (LineParseState state, Command command)
    {
        ResetState(state);
        while (TryNext(command)) continue;
    }

    public Parameter GetParameter ()
    {
        return parameterPool.Count > 0 ? parameterPool.Pop() : new Parameter();
    }

    public void ReturnParameter (Parameter parameter)
    {
        ClearParameter(parameter);
        parameterPool.Push(parameter);
    }

    public void ClearParameter (Parameter parameter)
    {
        ClearLineContent(parameter);
        ClearLineText(parameter.Identifier);
        ClearLineText(parameter.Value);
        foreach (var lineText in parameter.Value.Expressions)
            ReturnLineText(lineText);
        parameter.Value.Expressions.Clear();
    }

    private void ResetState (LineParseState state)
    {
        this.state = state;
        lastId = lastValue = default;
        lastExpressions.Clear();
    }

    private bool TryNext (Command command)
    {
        TryNext(out var token);

        switch (token.Type)
        {
            case ParamId:
                lastId = token;
                return true;
            case ParamValue:
                lastValue = token;
                return true;
            case Expression:
                lastExpressions.Add(token);
                return true;
            case NamelessParam:
            case NamedParam:
                AddParameter();
                ResetState(state);
                return true;
            case CommandBody:
                command.StartIndex = token.StartIndex;
                command.Length = token.Length;
                return false;
            case Error:
                AddError(token);
                return true;
            default: return true;
        }

        void AddParameter ()
        {
            var parameter = GetParameter();
            parameter.StartIndex = token.StartIndex;
            parameter.Length = token.Length;
            parameter.Identifier.Assign(Extract(lastId), lastId.StartIndex);
            parameter.Value.Assign(Extract(lastValue), lastValue.StartIndex);
            foreach (var expression in lastExpressions)
                parameter.Value.Expressions.Add(GetTextWith(expression));
            command.Parameters.Add(parameter);
        }
    }
}
