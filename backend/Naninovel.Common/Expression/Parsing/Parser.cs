namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class Parser (ParseOptions options)
{
    private readonly Lexer lexer = new();
    private Token[] tokens = [];

    /// <summary>
    /// Attempts to parse specified text as expression.
    /// </summary>
    /// <param name="text">Expression text to parse.</param>
    /// <param name="exp">Parsed expression, when successful.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParse (string text, out IExpression exp)
    {
        try
        {
            tokens = lexer.Lex(text);
            return (exp = ternary()!) != null;
        }
        catch (Error err)
        {
            exp = null!;
            var index = err.Index ?? 0;
            var length = err.Length ?? text.Length - index;
            options.HandleDiagnostic?.Invoke(new(index, length, err.Message));
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse specified text as expression assigned to a variable
    /// or multiple such statements separated with ";".
    /// </summary>
    /// <param name="text">Assignment statement(s) text to parse.</param>
    /// <param name="assignments">Parsed statements.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParseAssignments (string text, out IReadOnlyList<Assignment> assignments)
    {
        assignments = [];
        return false;
    }

    private Token current () => tokens.ElementAtOrDefault(0);
    private Token next () => tokens.ElementAtOrDefault(1);

    private bool peek (params string[] arguments)
    {
        if (tokens.Length == 0) return false;
        var first = tokens[0];
        for (var i = 0; i < arguments.Length; i++)
            if (first.Content == arguments[i])
                return true;
        return false;
    }

    private Token consume ()
    {
        var first = tokens[0];
        var copy = new Token[tokens.Length];
        Array.Copy(tokens, 1, copy, 0, tokens.Length - 1);
        tokens = copy;
        return first;
    }

    private Token expect (string content)
    {
        if (!peek(content))
            throw new Error($"Unexpected content: {content}");
        return consume();
    }

    private bool isEOF ()
    {
        return current().Type == TokenType.EOF;
    }

    private IExpression? ternary ()
    {
        var predicate = logicalOR();
        if (peek("?"))
        {
            consume();
            var truthy = ternary();
            expect(":");
            var falsy = ternary();
            return new TernaryOperation(predicate, truthy, falsy);
        }
        return predicate;
    }

    private IExpression? logicalOR ()
    {
        var left = logicalXOR();
        if (peek("||"))
        {
            var op = consume();
            var right = logicalOR();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? logicalXOR ()
    {
        var left = logicalAND();
        if (current().Content == "xor")
        {
            var op = consume();
            var right = logicalXOR();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? logicalAND ()
    {
        var left = bitwiseOR();
        if (peek("&&"))
        {
            var op = consume();
            var right = logicalAND();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? bitwiseOR ()
    {
        var left = bitwiseXOR();
        if (peek("|"))
        {
            var op = consume();
            var right = bitwiseOR();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? bitwiseXOR ()
    {
        var left = bitwiseAND();
        if (peek("^|"))
        {
            var op = consume();
            var right = bitwiseXOR();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? bitwiseAND ()
    {
        var left = relational();
        if (peek("&"))
        {
            var op = consume();
            var right = bitwiseAND();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? relational ()
    {
        var left = shift();
        if (peek("==", "==", "!=", "!==", ">=", "<=", ">", "<"))
        {
            var op = consume();
            var right = shift();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? shift ()
    {
        var left = additive();
        if (peek(">>", "<<", ">>>"))
        {
            var op = consume();
            var right = shift();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? additive ()
    {
        var left = multiplicative();
        while (peek("+", "-"))
        {
            var op = consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, multiplicative());
        }
        return left;
    }

    private IExpression? multiplicative ()
    {
        var left = unary();
        while (peek("*", "/", "%"))
        {
            var op = consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, unary());
        }
        return left;
    }

    private IExpression? unary ()
    {
        if (peek("-", "+", "~"))
        {
            var op = consume();
            var right = unary();
            return new UnaryOperation(Operators.Unary[op.Content], right);
        }
        return pow();
    }

    private IExpression? pow ()
    {
        var left = factorial();
        if (peek("^", "**"))
        {
            var op = consume();
            var right = unary();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? factorial ()
    {
        var left = symbol();
        if (peek("!"))
        {
            var op = consume();
            return new UnaryOperation(Operators.Unary[op.Content], left);
        }
        return left;
    }

    private IExpression? symbol ()
    {
        var cur = current();
        if (cur.Type == TokenType.SYMBOL)
        {
            var symbol = consume();
            var node = functionCall(symbol);
            return node;
        }
        return String();
    }

    private IExpression? functionCall (Token symbolToken)
    {
        var name = symbolToken.Content;
        if (peek("("))
        {
            consume();
            var @params = new List<IExpression>();
            while (!peek(")") && !isEOF())
            {
                @params.Add(ternary());
                if (peek(",")) consume();
            }
            expect(")");
            return new Function(name, @params);
        }
        return new Variable(name);
    }

    private IExpression? String ()
    {
        if (current().Type == TokenType.STRING)
            return new String(consume().Content);
        return number();
    }

    private IExpression? number ()
    {
        var token = current();
        if (token.Type == TokenType.NUMBER)
        {
            var text = consume().Content;
            if (!double.TryParse(text, out var num))
                throw new Error($"Failed to parse '{text}' as number");
            return new Numeric(num);
        }
        return parentheses();
    }

    private IExpression? parentheses ()
    {
        var token = current();
        if (token.Content == "(")
        {
            consume();
            var left = ternary();
            expect(")");
            return left;
        }
        return null;
    }
}
