namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="Expression"/> that can be evaluated.
/// </summary>
public class Parser (ParseOptions options)
{
    private Token token => IsEnd() ? default : tokens.Peek();
    private readonly Lexer lexer = new();
    private readonly Stack<Token> tokens = [];
    private int lastIndex;

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
            Reset(text);
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

    private void Reset (string text)
    {
        lastIndex = 0;
        tokens.Clear();
        var lexed = lexer.Lex(text);
        for (var i = lexed.Length - 1; i >= 0; i--)
            tokens.Push(lexed[i]);
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
        if (token.Type == TokenType.Identifier)
        {
            var symbol = Consume();
            var node = FunctionCall(symbol);
            return node;
        }
        return String();
    }

    private IExpression FunctionCall (Token symbolToken)
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
        if (token.Type == TokenType.String)
            return new String(Consume().Content);
        return Number();
    }

    private IExpression? Number ()
    {
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
        if (Is("("))
        {
            Consume();
            var left = Ternary();
            Expect(")");
            return left;
        }
        return null;
    }

    private Token Consume ()
    {
        var popped = tokens.Pop();
        lastIndex = popped.Index;
        return popped;
    }

    private void Expect (string content)
    {
        if (!Is(content))
            throw new Error($"Missing content: {content}", lastIndex);
        Consume();
    }

    private bool Is (string content) => token.Content == content;
    private bool IsOperator () => token.Type == TokenType.Operator;
    private bool IsEnd () => tokens.Count == 0;
}
