using System.Text;

namespace Naninovel.Expression;

internal class Lexer
{
    private readonly HashSet<string> operators = [
        ",", "(", ")", ":", "?",
        ..Operators.Binary.Keys,
        ..Operators.Unary.Keys
    ];
    private readonly List<Token> tokens = [];
    private readonly StringBuilder str = new();
    private string text = "";
    private int index;

    public Token[] Lex (string text)
    {
        Reset(text);

        while (index < text.Length)
        {
            while (char.IsWhiteSpace(Peek()))
                Consume();
            if (index >= text.Length) break;

            var c = Peek();
            var c2 = $"{c}{Peek(1)}";

            if (IsOperator(c2))
            {
                tokens.Add(new(TokenType.Operator, c2));
                Consume();
                Consume();
            }
            else if (IsOperator(c.ToString()))
            {
                tokens.Add(new(TokenType.Operator, c.ToString()));
                Consume();
            }
            else if (IsNumber(c)) tokens.Add(new(TokenType.Number, ReadNumber()));
            else if (IsQuote(c)) tokens.Add(new(TokenType.String, ReadString()));
            else if (IsIdentifier(c)) tokens.Add(new(TokenType.Identifier, ReadIdentifier()));
            else throw new Error($"Unexpected character: {c}", index);
        }

        return tokens.ToArray();
    }

    private void Reset (string text)
    {
        tokens.Clear();
        str.Clear();
        this.text = text;
        index = 0;
    }

    private char Peek (int nth = 0)
    {
        if (index + nth >= text.Length)
            return default;
        return text[index + nth];
    }

    private char Consume ()
    {
        var current = Peek();
        index += 1;
        return current;
    }

    private string ReadNumber ()
    {
        str.Clear();
        while (IsNumber(Peek()))
            str.Append(Consume());
        if (Peek() == '.')
            str.Append(Consume());
        while (IsNumber(Peek()))
            str.Append(Consume());
        return str.ToString();
    }

    private string ReadIdentifier ()
    {
        str.Clear();
        while (IsIdentifier(Peek()) || IsNumber(Peek()))
            str.Append(Consume());
        return str.ToString();
    }

    private string ReadString ()
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
        return str.ToString();
    }

    private bool IsNumber (char c) => char.IsDigit(c);
    private bool IsIdentifier (char c) => char.IsLetter(c) || c == '_';
    private bool IsOperator (string str) => operators.Contains(str);
    private bool IsQuote (char c) => c == '"';
    private bool IsEnd () => index > text.Length;
}
