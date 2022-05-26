using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

public abstract class LineParser
{
    protected abstract LineParseState State { get; }

    private readonly Stack<LineText> textPool = new();

    protected LineText GetLineText ()
    {
        return textPool.Count > 0 ? textPool.Pop() : new LineText();
    }

    protected LineText GetTextWith (Token token)
    {
        var lineText = GetLineText();
        lineText.Assign(Extract(token), token.StartIndex);
        return lineText;
    }

    protected void ReturnLineText (LineText lineText)
    {
        ClearLineText(lineText);
        textPool.Push(lineText);
    }

    protected void ClearLineText (LineText lineText)
    {
        lineText.Text = string.Empty;
        ClearLineContent(lineText);
    }

    protected void ClearLineContent (LineContent lineContent)
    {
        lineContent.StartIndex = 0;
        lineContent.Length = 0;
    }

    protected string Extract (Token token)
    {
        return State.LineText.Substring(token.StartIndex, token.Length);
    }

    protected void AddError (string description)
    {
        State.Errors?.Add(new ParseError(description));
    }

    protected void AddError (Token token)
    {
        State.Errors?.Add(new ParseError(token));
    }

    protected bool TryNext (TokenType types, ErrorType errors, out Token token)
    {
        return TryGet(types, errors, ++State.Index, out State.Index, out token);
    }

    protected bool TryNext (TokenType types, out Token token)
    {
        return TryGet(types, null, ++State.Index, out State.Index, out token);
    }

    protected bool TryNext (ErrorType errors, out Token token)
    {
        return TryGet(Error, errors, ++State.Index, out State.Index, out token);
    }

    protected bool TryNext (out Token token)
    {
        return TryGet(null, null, ++State.Index, out State.Index, out token);
    }

    private bool TryGet (TokenType? types, ErrorType? errors, int index, out int resultIndex, out Token result)
    {
        for (int i = index; i < State.Tokens.Count; i++)
        {
            var token = State.Tokens[i];
            if (!IsMatch(token)) continue;
            resultIndex = i;
            result = token;
            return true;
        }
        resultIndex = -1;
        result = default;
        return false;

        bool IsMatch (Token token)
        {
            if (!types.HasValue) return true;
            if (!types.Value.HasFlag(token.Type)) return false;
            if (types.Value.HasFlag(Error) && token.Type == Error &&
                errors.HasValue && !errors.Value.HasFlag(token.Error)) return false;
            return true;
        }
    }
}

public abstract class LineParser<TLine> : LineParser
    where TLine : LineContent, IScriptLine, new()
{
    protected override LineParseState State { get; } = new();

    private readonly Stack<TLine> linePool = new();

    /// <summary>
    /// Returns instance of the line parsed over the provided
    /// text representation using the associated lexing data.
    /// </summary>
    /// <param name="lineText">Text representation of the line to parse.</param>
    /// <param name="tokens">Lexing tokens describing structure of the line text.</param>
    /// <param name="errors">When provided, will add parse errors to the collection.</param>
    public TLine Parse (string lineText, IReadOnlyList<Token> tokens, ICollection<ParseError> errors = null)
    {
        State.Reset(lineText, tokens, errors);
        var line = linePool.Count > 0 ? linePool.Pop() : new TLine();
        Parse(line);
        line.StartIndex = 0;
        line.Length = lineText.Length;
        return line;
    }

    /// <summary>
    /// Returns line instance to the pool, so it can be re-used later.
    /// </summary>
    public void ReturnLine (TLine line)
    {
        ClearLineContent(line);
        ClearLine(line);
        linePool.Push(line);
    }

    protected abstract void Parse (TLine line);
    protected abstract void ClearLine (TLine line);
}
