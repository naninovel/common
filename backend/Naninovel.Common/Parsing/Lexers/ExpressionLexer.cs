using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal class ExpressionLexer
{
    private LexState state;
    private int startIndex;
    private bool hasBody;

    public static bool IsOpening (LexState state)
    {
        return state.IsUnescaped(ExpressionOpen[0]);
    }

    public void AddExpression (LexState state)
    {
        ResetState(state);
        AddOpening();
        AddBody();
        AddClosing();
        AddExpression();
        CheckMissingBody();
    }

    private void ResetState (LexState state)
    {
        this.state = state;
        startIndex = state.Index;
        hasBody = false;
    }

    private void AddOpening ()
    {
        state.AddToken(TokenType.ExpressionOpen, state.Index, 1);
        state.Move();
    }

    private void AddBody ()
    {
        var bodyStart = state.Index;
        while (IsInside()) Move();
        if (!hasBody) return;
        var bodyLength = state.Index - bodyStart;
        state.AddToken(TokenType.ExpressionBody, bodyStart, bodyLength);

        bool IsInside () => !state.EndReached && !IsClosing();

        void Move ()
        {
            if (state.IsNotSpace) hasBody = true;
            state.Move();
        }
    }

    private bool IsClosing () => state.IsUnescaped(ExpressionClose[0]);

    private void AddClosing ()
    {
        if (!IsClosing()) return;
        state.AddToken(TokenType.ExpressionClose, state.Index, 1);
        state.Move();
    }

    private void AddExpression ()
    {
        var length = state.Index - startIndex;
        state.AddToken(TokenType.Expression, startIndex, length);
    }

    private void CheckMissingBody ()
    {
        if (hasBody) return;
        var length = state.Index - startIndex;
        state.AddError(ErrorType.MissingExpressionBody, startIndex, length);
    }
}