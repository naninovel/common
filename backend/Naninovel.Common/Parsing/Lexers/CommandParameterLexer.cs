namespace Naninovel.Parsing;

internal class CommandParameterLexer (ExpressionLexer expressionLexer,
    TextIdentifierLexer textIdLexer, ISyntax stx)
{
    private LexState state = null!;
    private int startIndex;
    private int idEndIndex;
    private bool inlined;
    private bool quoted;
    private bool addedNameless;
    private int lastBoolFlagIndex;
    private bool multipleBoolFlags;

    public void AddParameters (LexState state, bool inlined)
    {
        Reset(state, inlined);
        while (!CommandBodyLexer.IsEndReached(state, inlined, stx))
            if (!TryAddBoolFlag() && !TryToggleQuoted() && !TryAddExpression() &&
                !TryAddTextId() && !TryAddIdentifier() && !TryAddValue())
                state.Move();
        AddFinalValue();
    }

    private void Reset (LexState state, bool inlined)
    {
        this.state = state;
        this.inlined = inlined;
        addedNameless = false;
        ResetParameterState();
    }

    private void ResetParameterState ()
    {
        startIndex = state.Index;
        idEndIndex = -1;
        quoted = false;
        lastBoolFlagIndex = -1;
        multipleBoolFlags = false;
    }

    private bool TryAddBoolFlag ()
    {
        if (!state.Is(stx.BooleanFlag[0]) || quoted || WasIdentifierAdded()) return false;
        if (!state.IsPreviousSpace && !state.IsNextSpace && !CommandBodyLexer.IsLast(state, inlined, stx)) return false;
        if (state.IsPreviousSpace && (state.IsNextSpace || CommandBodyLexer.IsLast(state, inlined, stx))) return false;
        if (state.IsPrevious(stx.BooleanFlag[0]) || state.IsNext(stx.BooleanFlag[0])) return false;
        if (WasBoolFlagAdded()) multipleBoolFlags = true;
        lastBoolFlagIndex = state.Index;
        state.Move();
        return true;
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
        if (!ExpressionLexer.IsOpening(state, stx)) return false;
        expressionLexer.AddExpression(state);
        return true;
    }

    private bool TryAddTextId ()
    {
        if (!TextIdentifierLexer.IsOpening(state, stx)) return false;
        textIdLexer.AddIdentifier(state);
        return true;
    }

    private bool TryAddIdentifier ()
    {
        if (quoted || !state.IsUnescaped(stx.ParameterAssign[0])) return false;
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
        if (WasBoolFlagAdded() && !multipleBoolFlags && !WasIdentifierAdded()) AddFlaggedBool();
        else if (WasIdentifierAdded()) AddNamed();
        else AddNameless();

        void AddFlaggedBool ()
        {
            var positiveFlag = lastBoolFlagIndex == state.Index - 1;
            var idStart = positiveFlag ? startIndex : startIndex + 1;
            var idLength = state.Index - startIndex - 1;
            state.AddToken(TokenType.ParamId, idStart, idLength);
            state.AddToken(TokenType.BoolFlag, lastBoolFlagIndex, 1);
            state.AddToken(TokenType.NamedParam, startIndex, state.Index - startIndex);
        }

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

    private bool WasIdentifierAdded () => idEndIndex > -1;

    private bool WasBoolFlagAdded () => lastBoolFlagIndex > -1;

    private void AddFinalValue ()
    {
        if (!quoted && state.IsPreviousSpace) return;
        AddValue();
    }
}
