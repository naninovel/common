using static Naninovel.Parsing.Identifiers;

namespace Naninovel.Parsing;

internal class GenericTextLineLexer
{
    private bool hasAppearance => firstAppearance > -1;

    private readonly ExpressionLexer expressionLexer;
    private readonly CommandBodyLexer commandLexer;

    private LexState state;
    private bool authorAdded;
    private bool canAddAuthor;
    private int startIndex;
    private int lastTextStart;
    private int lastNotSpace;
    private int firstAppearance;

    public GenericTextLineLexer (ExpressionLexer expressionLexer, CommandBodyLexer commandLexer)
    {
        this.expressionLexer = expressionLexer;
        this.commandLexer = commandLexer;
    }

    public LineType AddGenericTextLine (LexState state)
    {
        ResetState(state);
        while (!state.EndReached)
            if (!TryAddAuthorPrefix() && !TryAddInlinedCommand() && !TryAddExpression())
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
        firstAppearance = -1;
    }

    private bool TryAddAuthorPrefix ()
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

        void AddIdentifier ()
        {
            var endIndex = hasAppearance ? firstAppearance + 1 : state.Index;
            var length = endIndex - startIndex - 1;
            state.AddToken(TokenType.AuthorId, startIndex, length);
        }

        void AddAppearance ()
        {
            if (!hasAppearance) return;
            state.AddToken(TokenType.AppearanceAssign, firstAppearance, 1);
            var length = state.Index - firstAppearance - 2;
            if (length <= 0) state.AddError(ErrorType.MissingAppearance, firstAppearance, 1);
            else state.AddToken(TokenType.AuthorAppearance, firstAppearance + 1, length);
        }
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
        if (state.Is(AuthorAppearance[0]) && !hasAppearance) firstAppearance = state.Index;
        state.Move();

        bool IsValidAuthor () => !state.IsSpace && !state.Is('"') && !state.Is('\\');
    }

    private void AddPrecedingText ()
    {
        var length = lastNotSpace - lastTextStart + 1;
        if (length <= 0) return;
        state.AddToken(TokenType.GenericText, lastTextStart, length);
    }
}
