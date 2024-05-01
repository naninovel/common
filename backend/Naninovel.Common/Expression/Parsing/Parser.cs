namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="TryExpression"/> that can be evaluated.
/// </summary>
public class Parser (ParseOptions options)
{
    private Token token => IsEnd() ? default : tokens.Peek();
    private readonly Lexer lexer = new();
    private readonly Stack<Token> tokens = [];
    private readonly AssignmentParser assParser = new();
    private readonly List<(string var, string exp)> asses = [];
    private int lastIdx, errOffsetIdx;

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
            HandleError(text, err);
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse specified text as expression assigned to a variable
    /// or multiple such statements separated with ";".
    /// </summary>
    /// <param name="text">Assignment statement(s) text to parse.</param>
    /// <param name="assignments">Collection to store parsed assignments.</param>
    /// <returns>Whether all the assignments were parsed successfully.</returns>
    public bool TryParseAssignments (string text, IList<Assignment> assignments)
    {
        errOffsetIdx = 0;
        asses.Clear();

        try { assParser.Parse(text, asses); }
        catch (Error err)
        {
            HandleError(text, err);
            return false;
        }
        if (asses.Count == 0) return false;

        errOffsetIdx = text.IndexOf(asses[0].exp, StringComparison.Ordinal);

        foreach (var (var, assExp) in asses)
            if (TryParse(assExp, out var exp))
                assignments.Add(new(var, exp));
            else return false;

        errOffsetIdx = 0;
        return true;
    }

    private void Reset (string text)
    {
        lastIdx = 0;
        tokens.Clear();
        var lexed = lexer.Lex(text);
        for (var i = lexed.Length - 1; i >= 0; i--)
            tokens.Push(lexed[i]);
    }

    private void HandleError (string text, Error err)
    {
        var index = errOffsetIdx + (err.Index ?? 0);
        var length = (err.Length ?? text.Length - index) + errOffsetIdx;
        options.HandleDiagnostic?.Invoke(new(index, length, err.Message));
    }

    // Begin walking known expression morphemes, collapsing in order of associativity.
    private IExpression? TryExpression () => TryTernary();

    private IExpression? TryTernary ()
    {
        var predicate = TryLogicalOr();
        if (!IsOp() || !Is("?")) return predicate;

        if (predicate is null) throw Err("Missing ternary predicate.");
        Consume();
        var truthy = TryExpression() ?? throw Err("Missing truthy ternary branch.");
        Expect(":");
        var falsy = TryExpression() ?? throw Err("Missing falsy ternary branch.");
        return new TernaryOperation(predicate, truthy, falsy);
    }

    private IExpression? TryLogicalOr ()
    {
        var lhs = TryLogicalAnd();
        if (!IsOp() || !(Is("||") || Is("|"))) return lhs;

        if (lhs is null) throw Err("Missing left logical 'or' operand.");
        var op = Consume();
        var rhs = TryLogicalOr() ?? throw Err("Missing right logical 'or' operand.");
        return new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
    }

    private IExpression? TryLogicalAnd ()
    {
        var lhs = TryRelational();
        if (!IsOp() || !(Is("&&") || Is("&"))) return lhs;

        if (lhs is null) throw Err("Missing left logical 'and' operand.");
        var op = Consume();
        var rhs = TryLogicalAnd() ?? throw Err("Missing right logical 'and' operand.");
        return new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
    }

    private IExpression? TryRelational ()
    {
        var lhs = TryAdditive();
        if (!IsOp() || !(Is("=") || Is("==") || Is("!=") || Is(">=") ||
                         Is("<=") || Is(">") || Is("<"))) return lhs;

        if (lhs is null) throw Err("Missing left relational operand.");
        var op = Consume();
        var rhs = TryAdditive() ?? throw Err("Missing right relational operand.");
        return new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
    }

    private IExpression? TryAdditive ()
    {
        var lhs = TryMultiplicative();
        while (IsOp() && (Is("+") || Is("-")))
        {
            var op = Consume();
            var rhs = TryMultiplicative() ?? throw Err("Missing right additive operand.");
            lhs = new BinaryOperation(Operators.Binary[op.Content], lhs!, rhs);
        }
        return lhs;
    }

    private IExpression? TryMultiplicative ()
    {
        var lhs = TryUnary();
        while (IsOp() && (Is("*") || Is("/") || Is("%")))
        {
            if (lhs is null) throw Err("Missing left multiplicative operand.");
            var op = Consume();
            var rhs = TryUnary() ?? throw Err("Missing right multiplicative operand.");
            lhs = new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
        }
        return lhs;
    }

    private IExpression? TryUnary ()
    {
        if (!IsOp() || !(Is("-") || Is("+") || Is("!"))) return TryPow();

        var op = Consume();
        var operand = TryUnary() ?? throw Err("Missing unary operand.");
        return new UnaryOperation(Operators.Unary[op.Content], operand);
    }

    private IExpression? TryPow ()
    {
        var lhs = TrySymbol();
        if (!IsOp() || !Is("^")) return lhs;

        if (lhs is null) throw Err("Missing left pow operand.");
        var op = Consume();
        var rhs = TryUnary() ?? throw Err("Missing right pow operand.");
        return new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
    }

    private IExpression? TrySymbol ()
    {
        if (token.Type == TokenType.Identifier)
            return TryFunction(Consume().Content);
        return TryString();
    }

    private IExpression TryFunction (string name)
    {
        if (!Is("(")) return TryBoolean(name);

        Consume();
        var args = new List<IExpression>();
        while (!Is(")") && !IsEnd())
        {
            args.Add(TryExpression() ?? throw Err("Missing function parameter."));
            if (Is(",")) Consume();
        }
        Expect(")");
        return new Function(name, args);
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
            return new Numeric(double.Parse(Consume().Content));
        return TryClosure();
    }

    private IExpression? TryClosure ()
    {
        if (!Is("(")) return null; // Walked all supported morphemes, none found.

        Consume();
        var exp = TryExpression() ?? throw Err("Empty closure.");
        Expect(")");
        return exp;
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
