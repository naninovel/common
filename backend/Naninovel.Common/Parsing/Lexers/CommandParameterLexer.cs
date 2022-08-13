using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal class CommandParameterLexer
{
    private readonly ExpressionLexer expressionLexer;

    private LexState state = null!;
    private int startIndex;
    private int idEndIndex;
    private bool quoted;
    private bool addedNameless;

    public CommandParameterLexer (ExpressionLexer expressionLexer)
    {
        this.expressionLexer = expressionLexer;
    }

    public void AddParameters (LexState state, bool inlined)
    {
        Reset(state);
        while (!CommandBodyLexer.IsEndReached(state, inlined))
            if (!TryToggleQuoted() && !TryAddExpression() && !TryAddIdentifier() && !TryAddValue())
                state.Move();
        AddFinalValue();
    }

    private void Reset (LexState state)
    {
        this.state = state;
        addedNameless = false;
        ResetParameterState();
    }

    private void ResetParameterState ()
    {
        startIndex = state.Index;
        idEndIndex = -1;
        quoted = false;
    }

    private bool TryToggleQuoted ()
    {
        if (!state.IsUnescaped('"')) return false;
        quoted = !quoted;
        state.Move();
        return true;
    }

    private bool TryAddExpression ()
    {
        if (!ExpressionLexer.IsOpening(state)) return false;
        expressionLexer.AddExpression(state);
        return true;
    }

    private bool TryAddIdentifier ()
    {
        if (quoted || !state.IsUnescaped(ParameterAssign[0])) return false;
        var idLength = state.Index - startIndex;
        if (idLength == 0) state.AddError(ErrorType.MissingParamId, state.Index, 1);
        else state.AddToken(TokenType.ParamId, startIndex, idLength);
        state.AddToken(TokenType.ParamAssign, state.Index, 1);
        idEndIndex = state.Move();
        return true;
    }

    private bool TryAddValue ()
    {
        if (IsInsideValue()) return false;
        AddValue();
        state.Move();
        state.SkipSpace();
        ResetParameterState();
        return true;

        bool IsInsideValue () => quoted || !state.IsSpace;
    }

    private void AddValue ()
    {
        if (WasIdentifierAdded()) AddNamed();
        else AddNameless();

        bool WasIdentifierAdded () => idEndIndex > -1;

        void AddNamed ()
        {
            var valueLength = state.Index - idEndIndex;
            var paramLength = state.Index - startIndex;
            if (valueLength <= 0) state.AddError(ErrorType.MissingParamValue, startIndex, paramLength);
            else state.AddToken(TokenType.ParamValue, idEndIndex, valueLength);
            state.AddToken(TokenType.NamedParam, startIndex, state.Index - startIndex);
        }

        void AddNameless ()
        {
            var length = state.Index - startIndex;
            if (CheckMultiple()) return;
            state.AddToken(TokenType.ParamValue, startIndex, length);
            state.AddToken(TokenType.NamelessParam, startIndex, length);
            addedNameless = true;

            bool CheckMultiple ()
            {
                if (!addedNameless) return false;
                state.AddError(ErrorType.MultipleNameless, startIndex, length);
                return true;
            }
        }
    }

    private void AddFinalValue ()
    {
        if (!quoted && state.IsPreviousSpace) return;
        AddValue();
    }
}
