using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal class GenericTextLineLexer
{
    private readonly ExpressionLexer expressionLexer;
    private readonly CommandBodyLexer commandLexer;

    private LexState state;
    private bool authorAdded;
    private bool canAddAuthor;
    private int startIndex;
    private int lastTextStart;
    private int lastNotSpace;
    private int lastAppearance;

    public GenericTextLineLexer (ExpressionLexer expressionLexer, CommandBodyLexer commandLexer)
    {
        this.expressionLexer = expressionLexer;
        this.commandLexer = commandLexer;
    }

    public LineType AddGenericTextLine (LexState state)
    {
        ResetState(state);
        while (!state.EndReached)
            if (!TryAddAuthor() && !TryAddInlinedCommand() && !TryAddExpression())
                Move();
        AddPrecedingText();
        return LineType.GenericText;
    }

    private void ResetState (LexState state)
    {
        this.state = state;
        authorAdded = false;
        canAddAuthor = true;
        startIndex = state.Index;
        lastTextStart = startIndex;
        lastNotSpace = -1;
        lastAppearance = -1;
    }

    private bool TryAddAuthor ()
    {
        if (!ShouldTryAdd()) return false;
        Move();
        if (!state.IsSpace) return true;
        AddIdentifier();
        AddAppearance();
        state.AddToken(TokenType.AuthorAssign, state.Index - 1, 2);
        authorAdded = true;
        lastTextStart = state.Move();
        return true;

        bool ShouldTryAdd () => !authorAdded
                                && canAddAuthor
                                && state.Is(AuthorAssign[0])
                                && state.Index > startIndex;

        void AddAppearance ()
        {
            if (!HasAppearance()) return;
            state.AddToken(TokenType.AppearanceAssign, lastAppearance, 1);
            var length = state.Index - lastAppearance - 2;
            if (length <= 0) state.AddError(ErrorType.MissingAppearance, lastAppearance, 1);
            else state.AddToken(TokenType.AuthorAppearance, lastAppearance + 1, length);
        }

        void AddIdentifier ()
        {
            var idEndIndex = HasAppearance() ? lastAppearance + 1 : state.Index;
            var length = idEndIndex - startIndex - 1;
            state.AddToken(TokenType.AuthorId, startIndex, length);
        }

        bool HasAppearance () => lastAppearance > -1;
    }

    private bool TryAddInlinedCommand ()
    {
        if (!CommandBodyLexer.IsInlinedOpening(state)) return false;
        lastNotSpace = state.Index - 1;
        AddPrecedingText();
        var startIndex = state.Index;
        AddOpen();
        commandLexer.AddCommandBody(state, true);
        AddClose();
        state.AddToken(TokenType.Inlined, startIndex, state.Index - startIndex);
        lastTextStart = state.Index;
        return true;

        void AddOpen ()
        {
            state.AddToken(TokenType.InlinedOpen, state.Index, 1);
            state.Move();
        }

        void AddClose ()
        {
            if (state.EndReached) return;
            state.AddToken(TokenType.InlinedClose, state.Index, 1);
            state.Move();
        }
    }

    private bool TryAddExpression ()
    {
        if (!ExpressionLexer.IsOpening(state)) return false;
        expressionLexer.AddExpression(state);
        lastNotSpace = state.Index - 1;
        return true;
    }

    private void Move ()
    {
        if (state.IsNotSpace) lastNotSpace = state.Index;
        if (!IsValidAuthor()) canAddAuthor = false;
        if (state.Is(AuthorAppearance[0])) lastAppearance = state.Index;
        state.Move();

        bool IsValidAuthor ()
        {
            if (state.IsLetterOrDigit) return true;
            return state.Is('_') || state.Is('-') || state.Is('.') || state.Is('/');
        }
    }

    private void AddPrecedingText ()
    {
        var length = lastNotSpace - lastTextStart + 1;
        if (length <= 0) return;
        state.AddToken(TokenType.GenericText, lastTextStart, length);
    }
}