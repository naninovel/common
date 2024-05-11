namespace Naninovel.Parsing;

internal class CommandBodyLexer (CommandParameterLexer parameterLexer, ISyntax stx)
{
    private LexState state = null!;
    private int bodyStartIndex;
    private bool inlined;

    public static bool IsInlinedOpening (LexState state, ISyntax stx)
    {
        return state.IsUnescaped(stx.InlinedOpen[0]);
    }

    public static bool IsEndReached (LexState state, bool inlined, ISyntax stx)
    {
        if (inlined && state.IsUnescaped(stx.InlinedClose[0])) return true;
        return state.EndReached;
    }

    public static bool IsLast (LexState state, bool inlined, ISyntax stx)
    {
        if (inlined && state.IsNextUnescaped(stx.InlinedClose[0])) return true;
        return state.IsLast;
    }

    public void AddCommandBody (LexState state, bool inlined)
    {
        Reset(state, inlined);
        AddIdentifier();
        AddParameters();
        AddBody();
    }

    private void Reset (LexState state, bool inlined)
    {
        this.state = state;
        this.inlined = inlined;
        bodyStartIndex = state.Index;
    }

    private void AddIdentifier ()
    {
        state.SkipSpace();
        var startIndex = state.Index;
        while (ShouldMove()) state.Move();
        var length = state.Index - startIndex;
        if (length <= 0) AddMissingId();
        else state.AddToken(TokenType.CommandId, startIndex, length);

        bool ShouldMove () => state.IsNotSpace && !IsEndReached(state, inlined, stx);

        void AddMissingId ()
        {
            var errorStart = bodyStartIndex - 1;
            var errorLength = state.Index - errorStart;
            state.AddError(ErrorType.MissingCommandId, errorStart, errorLength);
        }
    }

    private void AddParameters ()
    {
        state.SkipSpace();
        if (IsEndReached(state, inlined, stx)) return;
        parameterLexer.AddParameters(state, inlined);
    }

    private void AddBody ()
    {
        var length = state.Index - bodyStartIndex;
        if (length == 0) return;
        state.AddToken(TokenType.CommandBody, bodyStartIndex, length);
    }
}
