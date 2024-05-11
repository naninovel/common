namespace Naninovel.Parsing;

internal class TextIdentifierLexer (ISyntax stx)
{
    private LexState state = null!;
    private int startIndex;
    private bool hasBody;

    public static bool IsOpening (LexState state, ISyntax stx)
    {
        return state.IsUnescaped(stx.TextIdOpen[0]) && state.IsNext(stx.TextIdOpen[1]);
    }

    public void AddIdentifier (LexState state)
    {
        Reset(state);
        AddOpening();
        AddBody();
        if (IsClosing()) AddClosing();
        AddIdentifier();
        CheckMissingBody();
    }

    private void Reset (LexState state)
    {
        this.state = state;
        startIndex = state.Index;
        hasBody = false;
    }

    private void AddOpening ()
    {
        state.AddToken(TokenType.TextIdOpen, state.Index, 2);
        state.Move();
        state.Move();
    }

    private void AddClosing ()
    {
        state.AddToken(TokenType.TextIdClose, state.Index, 1);
        state.Move();
    }

    private void AddBody ()
    {
        var bodyStart = state.Index;
        while (IsInside()) Move();
        if (!hasBody) return;
        var bodyLength = state.Index - bodyStart;
        state.AddToken(TokenType.TextIdBody, bodyStart, bodyLength);

        bool IsInside () => !state.EndReached && !IsClosing() && state.IsNotSpace;

        void Move ()
        {
            if (state.IsNotSpace) hasBody = true;
            state.Move();
        }
    }

    private void AddIdentifier ()
    {
        var length = state.Index - startIndex;
        state.AddToken(TokenType.TextId, startIndex, length);
    }

    private void CheckMissingBody ()
    {
        if (hasBody) return;
        var length = state.Index - startIndex;
        state.AddError(ErrorType.MissingTextIdBody, startIndex, length);
    }

    private bool IsClosing () => state.Is(stx.TextIdClose[0]);
}
