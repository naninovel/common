using System.Collections.Generic;
using static Naninovel.Parsing.TokenType;

namespace Naninovel.Parsing;

internal class LineWalker
{
    private string lineText;
    private IReadOnlyList<Token> tokens;
    private ICollection<ParseError> errors;
    private int index = -1;

    public void Reset (string lineText, IReadOnlyList<Token> tokens, ICollection<ParseError> errors = null)
    {
        index = -1;
        this.lineText = lineText;
        this.tokens = tokens;
        this.errors = errors;
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

    public void AddError (string message)
    {
        errors?.Add(new ParseError(message, index, lineText.Length - index));
    }

    public void AddError (Token token)
    {
        errors?.Add(new ParseError(token));
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
        return MoveUntilMatch(Error, errors, out token);
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
        if (types.Value.HasFlag(Error) && token.Type == Error &&
            errors.HasValue && !errors.Value.HasFlag(token.Error)) return false;
        return true;
    }
}
