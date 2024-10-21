namespace Naninovel.Parsing;

public class Lexer
{
    private readonly LexState state;
    private readonly CommandBodyLexer commandLexer;
    private readonly GenericLineLexer genericLineLexer;
    private readonly ISyntax stx;

    public Lexer (ISyntax syntax)
    {
        stx = syntax;
        state = new LexState(stx);
        var expressionLexer = new ExpressionLexer(stx);
        var textIdLexer = new TextIdentifierLexer(stx);
        var parameterLexer = new CommandParameterLexer(expressionLexer, textIdLexer, stx);
        commandLexer = new CommandBodyLexer(parameterLexer, stx);
        genericLineLexer = new GenericLineLexer(expressionLexer, commandLexer, textIdLexer, stx);
    }

    public LineType TokenizeLine (string text, ICollection<Token> tokens)
    {
        state.Reset(text, tokens);
        return TryEmptyLine() ??
               TryCommentLine() ??
               TryLabelLine() ??
               TryCommandLine() ??
               genericLineLexer.AddGenericLine(state);
    }

    public void TokenizeCommandBody (string text, ICollection<Token> tokens)
    {
        state.Reset(text, tokens);
        commandLexer.AddCommandBody(state, false);
    }

    private LineType? TryEmptyLine ()
    {
        state.SkipIndent();
        if (state.EndReached) return LineType.Generic;
        return null;
    }

    private LineType? TryCommentLine ()
    {
        if (!state.Is(stx.CommentLine[0])) return null;
        AddLineIdentifier();
        AddCommentText();
        return LineType.Comment;

        void AddCommentText ()
        {
            state.SkipSpace();
            if (state.EndReached) return;
            int startIndex = state.Index, lastNotSpace = startIndex;
            for (; !state.EndReached; state.Move())
                if (state.IsNotSpace)
                    lastNotSpace = state.Index;
            var length = lastNotSpace - startIndex + 1;
            state.AddToken(TokenType.CommentText, startIndex, length);
        }
    }

    private LineType? TryLabelLine ()
    {
        if (!state.Is(stx.LabelLine[0])) return null;
        AddLineIdentifier();
        AddLabelText();
        CheckSpaceInText();
        return LineType.Label;

        void AddLabelText ()
        {
            state.SkipSpace();
            var startIndex = state.Index;
            while (state.IsNotSpace) state.Move();
            var length = state.Index - startIndex;
            if (length <= 0) state.AddError(ErrorType.MissingLabel, 0, state.Length);
            else state.AddToken(TokenType.LabelText, startIndex, length);
        }

        void CheckSpaceInText ()
        {
            var textEnd = state.Index;
            state.SkipSpace();
            if (state.EndReached) return;
            state.AddError(ErrorType.SpaceInLabel, textEnd, state.EndIndex - textEnd);
        }
    }

    private LineType? TryCommandLine ()
    {
        if (!state.Is(stx.CommandLine[0])) return null;
        AddLineIdentifier();
        commandLexer.AddCommandBody(state, false);
        return LineType.Command;
    }

    private void AddLineIdentifier ()
    {
        state.AddToken(TokenType.LineId, state.Index, 1);
        state.Move();
    }
}
