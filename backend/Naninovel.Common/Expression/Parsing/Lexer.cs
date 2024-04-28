using System.Text;

namespace Naninovel.Expression;

internal class Lexer
{
    private readonly HashSet<string> delimiters = [
        ",", "(", ")", "[", "]", ":", "?",
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

            if (IsDelimiter(c2))
            {
                tokens.Add(new(TokenType.DELIMITER, c2));
                Consume();
                Consume();
            }
            else if (IsDelimiter(c.ToString()))
            {
                tokens.Add(new(TokenType.DELIMITER, c.ToString()));
                Consume();
            }
            else if (IsDigit(c)) tokens.Add(new(TokenType.NUMBER, ReadNumber()));
            else if (IsQuote(c)) tokens.Add(new(TokenType.STRING, ReadString()));
            else if (IsIdentifier(c)) tokens.Add(new(TokenType.SYMBOL, ReadIdentifier()));
            else throw new Error($"Unexpected character: {c}", index);
        }

        tokens.Add(new Token(TokenType.EOF));

        return tokens.ToArray();
    }

    private void Reset (string text)
    {
        tokens.Clear();
        str.Clear();
        this.text = text;
        index = 0;
    }

    private bool IsDigit (char c)
    {
        return char.IsDigit(c);
    }

    private bool IsIdentifier (char c)
    {
        return char.IsLetter(c) || c == '_';
    }

    private bool IsDelimiter (string str)
    {
        return delimiters.Contains(str);
    }

    private bool IsQuote (char c)
    {
        return c == '"';
    }

    private char Peek (int? nth = null)
    {
        nth ??= 0;
        if (index + nth >= text.Length)
            return default;
        return text[index + nth.Value];
    }

    private char Consume ()
    {
        var current = Peek();
        index += 1;
        return current;
    }

    private string ReadNumber ()
    {
        var number = "";

        if (Peek() == '.')
        {
            number += Consume();
            if (!IsDigit(Peek()))
                throw new Error("Number expected.", index);
        }
        else
        {
            while (IsDigit(Peek()))
                number += Consume();
            if (Peek() == '.')
                number += Consume();
        }

        while (IsDigit(Peek()))
            number += Consume();

        return number;
    }

    private string ReadIdentifier ()
    {
        var text = "";
        while (IsIdentifier(Peek()) || IsDigit(Peek()))
            text += Consume();
        return text;
    }

    private string ReadString ()
    {
        var quote = Consume();
        var str = "";
        var escape = false;
        while (true)
        {
            var c = Consume();
            if (c == default) throw new Error("Unclosed string.", index);

            if (escape)
            {
                str += "\"";
                escape = false;
            }
            else if (c == quote) break;
            else if (c == '\\') escape = true;
            else str += c;
        }
        return str;
    }
}
