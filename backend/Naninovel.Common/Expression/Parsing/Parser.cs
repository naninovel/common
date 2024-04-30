namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="TryExpression"/> that can be evaluated.
/// </summary>
public class Parser (ParseOptions options)
{
    private Token token => IsEnd() ? default : tokens.Peek();
    private readonly Lexer lexer = new();
    private readonly Stack<Token> tokens = [];
    private int lastIdx;

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
            return (exp = TryExpression()!) != null;
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
        lastIdx = 0;
        tokens.Clear();
        var lexed = lexer.Lex(text);
        for (var i = lexed.Length - 1; i >= 0; i--)
            tokens.Push(lexed[i]);
    }

    // Begins parsing known expression morphemes, collapsing in order of associativity.
    private IExpression? TryExpression () => TryTernary();

    private IExpression? TryTernary ()
    {
        var predicate = TryLogicalOr();
        if (!(IsOp() && Is("?"))) return predicate;

        if (predicate is null) throw Err("Missing ternary predicate.");
        Consume();
        var truthy = TryExpression() ?? throw Err("Missing truthy ternary branch.");
        Expect(":");
        var falsy = TryExpression() ?? throw Err("Missing falsy ternary branch.");
        return new TernaryOperation(predicate, truthy, falsy);
    }

    private IExpression? TryLogicalOr ()
    {
        var left = TryLogicalAnd();
        if (!(IsOp() && (Is("||") || Is("|")))) return left;

        if (left is null) throw Err("Missing left logical 'or' operand.");
        var op = Consume();
        var right = TryLogicalOr() ?? throw Err("Missing right logical 'or' operand.");
        return new BinaryOperation(Operators.Binary[op.Content], left, right);
    }

    private IExpression? TryLogicalAnd ()
    {
        var left = TryRelational();
        if (IsOp() && (Is("&&") || Is("&")))
        {
            var op = Consume();
            var right = TryLogicalAnd();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? TryRelational ()
    {
        var left = TryAdditive();
        if (IsOp() && (Is("=") || Is("==") || Is("!=") || Is(">=") || Is("<=") || Is(">") || Is("<")))
        {
            var op = Consume();
            var right = TryAdditive();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? TryAdditive ()
    {
        var left = TryMultiplicative();
        while (IsOp() && (Is("+") || Is("-")))
        {
            var op = Consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, TryMultiplicative());
        }
        return left;
    }

    private IExpression? TryMultiplicative ()
    {
        var left = TryUnary();
        while (IsOp() && (Is("*") || Is("/") || Is("%")))
        {
            var op = Consume();
            left = new BinaryOperation(Operators.Binary[op.Content], left, TryUnary());
        }
        return left;
    }

    private IExpression? TryUnary ()
    {
        if (IsOp() && (Is("-") || Is("+") || Is("!")))
        {
            var op = Consume();
            var right = TryUnary();
            return new UnaryOperation(Operators.Unary[op.Content], right);
        }
        return TryPow();
    }

    private IExpression? TryPow ()
    {
        var left = TrySymbol();
        if (IsOp() && Is("^"))
        {
            var op = Consume();
            var right = TryUnary();
            return new BinaryOperation(Operators.Binary[op.Content], left, right);
        }
        return left;
    }

    private IExpression? TrySymbol ()
    {
        if (token.Type == TokenType.Identifier)
        {
            var symbol = Consume();
            var node = TryFunction(symbol.Content);
            return node;
        }
        return TryString();
    }

    private IExpression TryFunction (string name)
    {
        if (Is("("))
        {
            Consume();
            var @params = new List<IExpression>();
            while (!Is(")") && !IsEnd())
            {
                @params.Add(TryExpression());
                if (Is(",")) Consume();
            }
            Expect(")");
            return new Function(name, @params);
        }
        return TryBoolean(name);
    }

    private IExpression TryBoolean (string name)
    {
        if (name.Equals(options.Identifiers.True, StringComparison.OrdinalIgnoreCase))
            return new Boolean(true);
        if (name.Equals(options.Identifiers.False, StringComparison.OrdinalIgnoreCase))
            return new Boolean(false);
        return DoVariable(name);
    }

    private IExpression DoVariable (string name)
    {
        return new Variable(name);
    }

    private IExpression? TryString ()
    {
        if (token.Type == TokenType.String)
            return new String(Consume().Content);
        return TryNumeric();
    }

    private IExpression? TryNumeric ()
    {
        if (token.Type == TokenType.Number)
        {
            var text = Consume().Content;
            if (!double.TryParse(text, out var num))
                throw Err($"Failed to parse '{text}' as number");
            return new Numeric(num);
        }
        return TryClosure();
    }

    private IExpression? TryClosure ()
    {
        if (Is("("))
        {
            Consume();
            var left = TryExpression();
            Expect(")");
            return left;
        }
        return null;
    }

    private Token Consume ()
    {
        var popped = tokens.Pop();
        lastIdx = popped.Index;
        return popped;
    }

    private void Expect (string content)
    {
        if (!Is(content))
            throw Err($"Missing content: {content}");
        Consume();
    }

    private Error Err (string message) => new(message, lastIdx);
    private bool Is (string content) => token.Content == content;
    private bool IsOp () => token.Type == TokenType.Operator;
    private bool IsEnd () => tokens.Count == 0;
}
