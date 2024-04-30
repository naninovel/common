namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class Parser (ParseOptions options)
{
    private Token current => tokens.ElementAtOrDefault(0);
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

    private bool Is (string content)
    {
        return current.Content == content;
    }

    private bool IsOperator ()
    {
        return current.Type == TokenType.Operator;
    }

    private Token Consume ()
    {
        var first = tokens[0];
        var copy = new Token[tokens.Length];
        Array.Copy(tokens, 1, copy, 0, tokens.Length - 1);
        tokens = copy;
        return first;
    }

    private void Expect (string content)
    {
        if (!Is(content))
            throw new Error($"Unexpected content: {content}");
        Consume();
    }

    private bool IsEnd ()
    {
        return tokens.Length == 0;
    }

    private IExpression? Ternary ()
    {
        var predicate = LogicalOr();
        if (IsOperator() && Is("?"))
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
        var left = LogicalAnd();
        if (IsOperator() && (Is("||") || Is("|")))
        {
            var op = Consume();
            var right = LogicalOr();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? LogicalAnd ()
    {
        var left = Relational();
        if (IsOperator() && (Is("&&") || Is("&")))
        {
            var op = Consume();
            var right = LogicalAnd();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? Relational ()
    {
        var left = Additive();
        if (IsOperator() && (Is("=") || Is("==") || Is("!=") || Is(">=") || Is("<=") || Is(">") || Is("<")))
        {
            var op = Consume();
            var right = Additive();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? Additive ()
    {
        var left = Multiplicative();
        while (IsOperator() && (Is("+") || Is("-")))
        {
            var op = Consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, Multiplicative());
        }
        return left;
    }

    private IExpression? Multiplicative ()
    {
        var left = Unary();
        while (IsOperator() && (Is("*") || Is("/") || Is("%")))
        {
            var op = Consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, Unary());
        }
        return left;
    }

    private IExpression? Unary ()
    {
        if (IsOperator() && (Is("-") || Is("+") || Is("!")))
        {
            var op = Consume();
            var right = Unary();
            return new UnaryOperation(Operators.Unary[op.Content], right);
        }
        return Pow();
    }

    private IExpression? Pow ()
    {
        var left = Symbol();
        if (IsOperator() && Is("^"))
        {
            var op = Consume();
            var right = Unary();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? Symbol ()
    {
        if (current.Type == TokenType.Identifier)
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
        if (Is("("))
        {
            Consume();
            var @params = new List<IExpression>();
            while (!Is(")") && !IsEnd())
            {
                @params.Add(Ternary());
                if (Is(",")) Consume();
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
        if (current.Type == TokenType.String)
            return new String(Consume().Content);
        return Number();
    }

    private IExpression? Number ()
    {
        if (current.Type == TokenType.Number)
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
        if (Is("("))
        {
            Consume();
            var left = Ternary();
            Expect(")");
            return left;
        }
        return null;
    }
}
