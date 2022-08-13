using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal class CommandBodyLexer
{
    private readonly CommandParameterLexer parameterLexer;

    private LexState state = null!;
    private int bodyStartIndex;
    private bool inlined;

    public CommandBodyLexer (CommandParameterLexer parameterLexer)
    {
        this.parameterLexer = parameterLexer;
    }

    public static bool IsInlinedOpening (LexState state)
    {
        return state.IsUnescaped(InlinedOpen[0]);
    }

    public static bool IsEndReached (LexState state, bool inlined)
    {
        return state.EndReached || inlined && state.IsUnescaped(InlinedClose[0]);
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

        bool ShouldMove () => state.IsNotSpace && !IsEndReached(state, inlined);

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
        if (IsEndReached(state, inlined)) return;
        parameterLexer.AddParameters(state, inlined);
    }

    private void AddBody ()
    {
        var length = state.Index - bodyStartIndex;
        if (length == 0) return;
        state.AddToken(TokenType.CommandBody, bodyStartIndex, length);
    }
}
