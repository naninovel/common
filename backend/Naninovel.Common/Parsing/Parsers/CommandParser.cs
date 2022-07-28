using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;
using static Naninovel.Parsing.ErrorType;
using static Naninovel.Parsing.ParsingErrors;

namespace Naninovel.Parsing;

internal class CommandParser
{
    private readonly List<Parameter> parameters = new();
    private readonly List<IMixedValue> value = new();
    private readonly Queue<Token> expressions = new();
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
        expressions.Clear();
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
                expressions.Enqueue(token);
                return true;
            case ParamValue:
                AddValue(token);
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

    private void AddValue (Token valueToken)
    {
        var index = valueToken.StartIndex;
        var textStartIndex = -1;
        for (; index <= valueToken.EndIndex; index++)
            if (ShouldProcessExpression()) ProcessExpression();
            else if (!IsTextStarted()) textStartIndex = index;
        if (IsTextStarted()) AddText(valueToken.EndIndex - textStartIndex + 1);

        bool ShouldProcessExpression ()
        {
            return expressions.Count > 0 &&
                   expressions.Peek().StartIndex == index;
        }

        void ProcessExpression ()
        {
            var expression = expressions.Dequeue();
            if (IsTextStarted()) AddText(expression.StartIndex - textStartIndex);
            AddExpression(expression);
            index = expression.EndIndex;
        }

        bool IsTextStarted () => textStartIndex != -1;

        void AddText (int endIndex)
        {
            var text = ValueCoder.Decode(walker.Extract(textStartIndex, endIndex));
            value.Add(new PlainText(text));
            textStartIndex = -1;
        }

        void AddExpression (Token expression)
        {
            var startIndex = expression.StartIndex + 1;
            var length = expression.EndIndex - expression.StartIndex - 1;
            value.Add(new Expression(walker.Extract(startIndex, length)));
        }
    }

    private void AddParameter ()
    {
        parameters.Add(new(paramId, value.ToArray()));
        ResetParameterState();
    }
}
