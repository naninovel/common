using System.Text;

namespace Naninovel.Expression;

internal class Lexer
{
    private readonly List<Token> tokens = [];
    private readonly StringBuilder str = new();
    private string text = "";
    private int index;

    public Token[] Lex (string text)
    {
        Reset(text);
        while (index < text.Length)
            LexNext(Peek());
        return tokens.ToArray();
    }

    private void Reset (string text)
    {
        tokens.Clear();
        str.Clear();
        this.text = text;
        index = 0;
    }

    private void LexNext (char c)
    {
        if (IsSpace(c)) Consume();
        else if (IsOperator(c, out var op)) LexOperator(op);
        else if (IsNumber(c)) LexNumber();
        else if (IsQuote(c)) LexString();
        else if (IsIdentifier(c)) LexIdentifier();
        else throw new Error($"Unexpected character: {c}", index);
    }

    private char Peek (int offset = 0)
    {
        if (index + offset >= text.Length)
            return default;
        return text[index + offset];
    }

    private char Consume ()
    {
        var current = Peek();
        index += 1;
        return current;
    }

    private void LexOperator (string op)
    {
        tokens.Add(new(TokenType.Operator, index, op));
        for (int i = 0; i < op.Length; i++)
            Consume();
    }

    private void LexNumber ()
    {
        str.Clear();
        while (IsNumber(Peek()))
            str.Append(Consume());
        if (Peek() == '.')
            str.Append(Consume());
        while (IsNumber(Peek()))
            str.Append(Consume());
        tokens.Add(new(TokenType.Number, index, str.ToString()));
    }

    private void LexIdentifier ()
    {
        str.Clear();
        while (IsIdentifier(Peek()) || IsNumber(Peek()))
            str.Append(Consume());
        tokens.Add(new(TokenType.Identifier, index, str.ToString()));
    }

    private void LexString ()
    {
        Consume();
        str.Clear();
        var escape = false;
        while (true)
        {
            var c = Consume();
            if (IsEnd()) throw new Error("Unclosed string.", index - 2);

            if (escape)
            {
                str.Append('\"');
                escape = false;
            }
            else if (IsQuote(c)) break;
            else if (c == '\\') escape = true;
            else str.Append(c);
        }
        tokens.Add(new(TokenType.String, index, str.ToString()));
    }

    private bool IsSpace (char c) => char.IsWhiteSpace(c);
    private bool IsOperator (char c, out string op) => Operators.IsOperator(c, Peek(1), out op);
    private bool IsNumber (char c) => char.IsDigit(c);
    private bool IsIdentifier (char c) => char.IsLetter(c) || c == '_';
    private bool IsQuote (char c) => c == '"';
    private bool IsEnd () => index > text.Length;
}
