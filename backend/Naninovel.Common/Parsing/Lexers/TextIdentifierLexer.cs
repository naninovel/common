using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal class TextIdentifierLexer
{
    private LexState state = null!;
    private int startIndex;
    private bool hasBody;

    public static bool IsDelimiter (LexState state)
    {
        return state.IsUnescaped(TextIdDelimiter[0]);
    }

    public void AddIdentifier (LexState state)
    {
        Reset(state);
        AddDelimiter();
        AddBody();
        if (IsDelimiter(state)) AddDelimiter();
        AddIdentifier();
        CheckMissingBody();
    }

    private void Reset (LexState state)
    {
        this.state = state;
        startIndex = state.Index;
        hasBody = false;
    }

    private void AddDelimiter ()
    {
        state.AddToken(TokenType.TextIdDelimiter, state.Index, 1);
        state.Move();
    }

    private void AddBody ()
    {
        var bodyStart = state.Index;
        while (IsInside()) Move();
        if (!hasBody) return;
        var bodyLength = state.Index - bodyStart;
        state.AddToken(TokenType.TextIdBody, bodyStart, bodyLength);

        bool IsInside () => !state.EndReached && !IsDelimiter(state) && state.IsNotSpace;

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
}
