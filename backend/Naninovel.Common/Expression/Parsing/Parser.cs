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
            return (exp = Ternary()!) != null;
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

    private Token Current () => tokens.ElementAtOrDefault(0);

    private Token Next () => tokens.ElementAtOrDefault(1);

    private bool Peek (params string[] arguments)
    {
        if (tokens.Length == 0) return false;
        var first = tokens[0];
        for (var i = 0; i < arguments.Length; i++)
            if (first.Content == arguments[i])
                return true;
        return false;
    }

    private Token Consume ()
    {
        var first = tokens[0];
        var copy = new Token[tokens.Length];
        Array.Copy(tokens, 1, copy, 0, tokens.Length - 1);
        tokens = copy;
        return first;
    }

    private Token Expect (string content)
    {
        if (!Peek(content))
            throw new Error($"Unexpected content: {content}");
        return Consume();
    }

    private bool IsEnd ()
    {
        return tokens.Length == 0;
    }

    private IExpression? Ternary ()
    {
        var predicate = LogicalOr();
        if (Peek("?") && Current().Type == TokenType.Operator)
        {
            Consume();
            var truthy = Ternary();
            Expect(":");
            var falsy = Ternary();
            return new TernaryOperation(predicate, truthy, falsy);
        }
        return predicate;
    }

    private IExpression? LogicalOr ()
    {
        var left = LogicalXor();
        if (Peek("||") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = LogicalOr();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? LogicalXor ()
    {
        var left = LogicalAnd();
        if (Current().Content == "xor" && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = LogicalXor();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? LogicalAnd ()
    {
        var left = BitwiseOr();
        if (Peek("&&") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = LogicalAnd();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? BitwiseOr ()
    {
        var left = BitwiseXor();
        if (Peek("|") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = BitwiseOr();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? BitwiseXor ()
    {
        var left = BitwiseAnd();
        if (Peek("^|") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = BitwiseXor();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? BitwiseAnd ()
    {
        var left = Relational();
        if (Peek("&") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = BitwiseAnd();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? Relational ()
    {
        var left = Shift();
        if (Peek("=", "==", "!=", ">=", "<=", ">", "<") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = Shift();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? Shift ()
    {
        var left = Additive();
        if (Peek(">>", "<<", ">>>") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = Shift();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? Additive ()
    {
        var left = Multiplicative();
        while (Peek("+", "-") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, Multiplicative());
        }
        return left;
    }

    private IExpression? Multiplicative ()
    {
        var left = Unary();
        while (Peek("*", "/", "%") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, Unary());
        }
        return left;
    }

    private IExpression? Unary ()
    {
        if (Peek("-", "+", "!") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = Unary();
            return new UnaryOperation(Operators.Unary[op.Content], right);
        }
        return Pow();
    }

    private IExpression? Pow ()
    {
        var left = Factorial();
        if (Peek("^", "**") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            var right = Unary();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? Factorial ()
    {
        var left = Symbol();
        if (Peek("!") && Current().Type == TokenType.Operator)
        {
            var op = Consume();
            return new UnaryOperation(Operators.Unary[op.Content], left);
        }
        return left;
    }

    private IExpression? Symbol ()
    {
        var cur = Current();
        if (cur.Type == TokenType.Identifier)
        {
            var symbol = Consume();
            var node = FunctionCall(symbol);
            return node;
        }
        return String();
    }

    private IExpression? FunctionCall (Token symbolToken)
    {
        var name = symbolToken.Content;
        if (Peek("("))
        {
            Consume();
            var @params = new List<IExpression>();
            while (!Peek(")") && !IsEnd())
            {
                @params.Add(Ternary());
                if (Peek(",")) Consume();
            }
            Expect(")");
            return new Function(name, @params);
        }

        if (name.Equals(options.Identifiers.True, StringComparison.OrdinalIgnoreCase))
            return new Boolean(true);
        if (name.Equals(options.Identifiers.False, StringComparison.OrdinalIgnoreCase))
            return new Boolean(false);

        return new Variable(name);
    }

    private IExpression? String ()
    {
        if (Current().Type == TokenType.String)
            return new String(Consume().Content);
        return Number();
    }

    private IExpression? Number ()
    {
        var token = Current();
        if (token.Type == TokenType.Number)
        {
            var text = Consume().Content;
            if (!double.TryParse(text, out var num))
                throw new Error($"Failed to parse '{text}' as number");
            return new Numeric(num);
        }
        return Parentheses();
    }

    private IExpression? Parentheses ()
    {
        var token = Current();
        if (token.Content == "(")
        {
            Consume();
            var left = Ternary();
            Expect(")");
            return left;
        }
        return null;
    }
}
