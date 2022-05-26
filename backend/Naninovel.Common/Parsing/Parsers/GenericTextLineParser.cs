using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public class GenericTextLineParser : LineParser<GenericTextLine>
{
    private readonly CommandParser commandParser = new();
    private readonly Stack<GenericText> textPool = new();
    private readonly Stack<InlinedCommand> inlinedPool = new();
    private readonly List<Token> lastExpressions = new();

    protected override void Parse (GenericTextLine line)
    {
        while (TryNext(line)) continue;
    }

    protected override void ClearLine (GenericTextLine line)
    {
        ClearLineText(line.Prefix.AuthorIdentifier);
        ClearLineText(line.Prefix.AuthorAppearance);
        foreach (var content in line.Content)
            if (content is GenericText text) ReturnGenericText(text);
            else ReturnInlined(content as InlinedCommand);
        line.Content.Clear();
    }

    private GenericText GetGenericText ()
    {
        return textPool.Count > 0 ? textPool.Pop() : new GenericText();
    }

    private void ReturnGenericText (GenericText genericText)
    {
        ClearLineText(genericText);
        foreach (var lineText in genericText.Expressions)
            ReturnLineText(lineText);
        genericText.Expressions.Clear();
        textPool.Push(genericText);
    }

    private bool TryNext (GenericTextLine line)
    {
        if (!TryNext(out var token)) return false;

        switch (token.Type)
        {
            case AuthorId:
                line.Prefix.AuthorIdentifier.Assign(Extract(token), token.StartIndex);
                return true;
            case AuthorAppearance:
                line.Prefix.AuthorAppearance.Assign(Extract(token), token.StartIndex);
                return true;
            case AuthorAssign:
                AddPrefix();
                return true;
            case TokenType.GenericText:
                AddText();
                return true;
            case Expression:
                lastExpressions.Add(token);
                return true;
            case InlinedOpen:
                AddInlined();
                return true;
            case Error:
                AddError(token);
                return true;
            default: return true;
        }

        void AddPrefix ()
        {
            line.Prefix.StartIndex = line.Prefix.AuthorIdentifier.StartIndex;
            line.Prefix.Length = token.EndIndex - line.Prefix.StartIndex;
        }

        void AddText ()
        {
            var text = GetGenericText();
            text.Assign(Extract(token), token.StartIndex);
            foreach (var expression in lastExpressions)
                text.Expressions.Add(GetTextWith(expression));
            line.Content.Add(text);
            lastExpressions.Clear();
        }

        void AddInlined ()
        {
            var inlined = GetInlined();
            commandParser.ParseNext(State, inlined.Command);
            inlined.StartIndex = token.StartIndex;
            inlined.Length = inlined.Command.Length + 2;
            line.Content.Add(inlined);
        }
    }

    private InlinedCommand GetInlined ()
    {
        return inlinedPool.Count > 0 ? inlinedPool.Pop() : new InlinedCommand();
    }

    private void ReturnInlined (InlinedCommand inlined)
    {
        ClearInlined(inlined);
        inlinedPool.Push(inlined);
    }

    private void ClearInlined (InlinedCommand inlined)
    {
        commandParser.ClearCommand(inlined.Command);
    }
}
