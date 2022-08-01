using System;
using System.Collections.Generic;

namespace Naninovel.Parsing;

internal class LineWalker
{
    private readonly IErrorHandler errorHandler;
    private readonly IAssociator associator;

    private string lineText = "";
    private IReadOnlyList<Token> tokens = Array.Empty<Token>();
    private int index = -1;

    public LineWalker (IErrorHandler errorHandler, IAssociator associator)
    {
        this.errorHandler = errorHandler;
        this.associator = associator;
    }

    public void Reset (string lineText, IReadOnlyList<Token> tokens)
    {
        index = -1;
        this.lineText = lineText;
        this.tokens = tokens;
    }

    public char GetCharAt (int index) => lineText[index];

    public string Extract (int startIndex, int length)
    {
        return lineText.Substring(startIndex, length);
    }

    public string Extract (Token token)
    {
        return Extract(token.StartIndex, token.Length);
    }

    public void Error (string message)
    {
        errorHandler?.HandleError(new(message, index, lineText.Length - index));
    }

    public void Error (Token token)
    {
        errorHandler?.HandleError(new(token));
    }

    public void Associate (ILineComponent component, LineRange range)
    {
        associator?.Associate(component, range);
    }

    public void Associate (ILineComponent component, Token token)
    {
        Associate(component, token.Range);
    }

    public bool Next (TokenType types, ErrorType errors, out Token token)
    {
        return MoveUntilMatch(types, errors, out token);
    }

    public bool Next (TokenType types, out Token token)
    {
        return MoveUntilMatch(types, null, out token);
    }

    public bool Next (ErrorType errors, out Token token)
    {
        return MoveUntilMatch(TokenType.Error, errors, out token);
    }

    public bool Next (out Token token)
    {
        return MoveUntilMatch(null, null, out token);
    }

    private bool MoveUntilMatch (TokenType? types, ErrorType? errors, out Token result)
    {
        index++;
        for (int i = index; i < tokens.Count; i++)
        {
            if (!IsMatch(types, errors, tokens[i])) continue;
            index = i;
            result = tokens[i];
            return true;
        }
        index = -1;
        result = default;
        return false;
    }

    private static bool IsMatch (TokenType? types, ErrorType? errors, Token token)
    {
        if (!types.HasValue) return true;
        if (!types.Value.HasFlag(token.Type)) return false;
        if (types.Value.HasFlag(TokenType.Error) && token.Type == TokenType.Error &&
            errors.HasValue && !errors.Value.HasFlag(token.Error)) return false;
        return true;
    }
}
