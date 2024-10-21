namespace Naninovel.Expression;

/// <summary>
/// Parses expression text into <see cref="IExpression"/> that can be evaluated.
/// </summary>
public class Parser
{
    private Token token => IsEnd() ? default : tokens.Peek();
    private readonly Stack<Token> tokens = [];
    private readonly List<(string var, string exp)> asses = [];
    private readonly Lexer lexer;
    private readonly ParseOptions options;
    private readonly AssignmentParser assParser;
    private Token lastToken;
    private string text = "";
    private int assOffset;
    private bool anyError;

    public Parser (ParseOptions options)
    {
        this.options = options;
        lexer = new(Err);
        assParser = new(Err);
    }

    /// <summary>
    /// Attempts to parse specified text as expression.
    /// </summary>
    /// <param name="text">Expression text to parse.</param>
    /// <param name="exp">Parsed expression, when successful.</param>
    /// <returns>Whether the text was parsed successfully.</returns>
    public bool TryParse (string text, out IExpression exp)
    {
        Reset(text);
        return (exp = TryExpression()!) != null && !anyError;
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
        this.anyError = false;
        assParser.Parse(text, asses);
        if (asses.Count == 0) return false;

        var anyError = this.anyError;
        for (var i = 0; i < asses.Count; i++)
        {
            var (var, assExp) = asses[i];
            assOffset = text.IndexOf(asses[i].exp, StringComparison.Ordinal);
            if (TryParse(assExp, out var exp))
                assignments.Add(new(var, exp));
            else anyError = true;
        }

        assOffset = 0;
        asses.Clear();

        return !anyError;
    }

    private void Reset (string text)
    {
        this.text = text;
        lastToken = default;
        anyError = false;
        tokens.Clear();
        var lexed = lexer.Lex(text);
        for (var i = lexed.Length - 1; i >= 0; i--)
            tokens.Push(lexed[i]);
    }

    private IExpression Map (IExpression exp, int idx, int length)
    {
        options.HandleRange?.Invoke(new(exp, idx + assOffset, length));
        return exp;
    }

    // Begin walking known expression morphemes, collapsing in order of associativity.
    private IExpression? TryExpression () => TryTernary();

    private IExpression? TryTernary ()
    {
        var predicate = TryLogicalOr();
        if (!IsOp() || !Is("?")) return predicate;

        predicate ??= Err("Missing ternary predicate.");
        Consume();
        var truthy = TryExpression() ?? Err("Missing truthy ternary branch.");
        Expect(":");
        var falsy = TryExpression() ?? Err("Missing falsy ternary branch.");
        return new TernaryOperation(predicate, truthy, falsy);
    }

    private IExpression? TryLogicalOr ()
    {
        var lhs = TryLogicalAnd();
        if (!IsOp() || !(Is("||") || Is("|"))) return lhs;

        lhs ??= Err("Missing left logical 'or' operand.");
        var op = Consume();
        var rhs = TryLogicalOr() ?? Err("Missing right logical 'or' operand.");
        return new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
    }

    private IExpression? TryLogicalAnd ()
    {
        var lhs = TryRelational();
        if (!IsOp() || !(Is("&&") || Is("&"))) return lhs;

        lhs ??= Err("Missing left logical 'and' operand.");
        var op = Consume();
        var rhs = TryLogicalAnd() ?? Err("Missing right logical 'and' operand.");
        return new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
    }

    private IExpression? TryRelational ()
    {
        var lhs = TryAdditive();
        if (!IsOp() || !(Is("=") || Is("==") || Is("!=") || Is(">=") ||
                         Is("<=") || Is(">") || Is("<"))) return lhs;

        lhs ??= Err("Missing left relational operand.");
        var op = Consume();
        var rhs = TryAdditive() ?? Err("Missing right relational operand.");
        return new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
    }

    private IExpression? TryAdditive ()
    {
        var lhs = TryMultiplicative();
        while (IsOp() && (Is("+") || Is("-")))
        {
            var op = Consume();
            var rhs = TryMultiplicative() ?? Err("Missing right additive operand.");
            lhs = new BinaryOperation(Operators.Binary[op.Content], lhs!, rhs);
        }
        return lhs;
    }

    private IExpression? TryMultiplicative ()
    {
        var lhs = TryUnary();
        while (IsOp() && (Is("*") || Is("/") || Is("%")))
        {
            lhs ??= Err("Missing left multiplicative operand.");
            var op = Consume();
            var rhs = TryUnary() ?? Err("Missing right multiplicative operand.");
            lhs = new BinaryOperation(Operators.Binary[op.Content], lhs, rhs);
        }
        return lhs;
    }

    private IExpression? TryUnary ()
    {
        if (!IsOp() || !(Is("-") || Is("+") || Is("!"))) return TryPow();

        var op = Consume();
        var operand = TryUnary() ?? Err("Missing unary operand.");
        return new UnaryOperation(Operators.Unary[op.Content], operand);
    }

    private IExpression? TryPow ()
    {
        var lhs = TrySymbol();
        if (!IsOp() || !Is("^")) return lhs;

        lhs ??= Err("Missing left pow operand.");
        var op = Consume();
        var rhs = TryUnary() ?? Err("Missing right pow operand.");
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

        var start = lastToken.Index - name.Length;

        Consume();
        var args = new List<IExpression>();
        while (!Is(")") && !IsEnd())
        {
            args.Add(TryExpression() ?? Err("Missing function parameter."));
            if (Is(",")) Consume();
        }

        var length = Expect(")") ? lastToken.Index - start + 1 : text.Length - start;
        return Map(new Function(name, args), start, length);
    }

    private IExpression TryBoolean (string name)
    {
        if (name.Equals(options.Syntax.True, StringComparison.OrdinalIgnoreCase))
            return Map(new Boolean(true), lastToken.Index - name.Length, name.Length);
        if (name.Equals(options.Syntax.False, StringComparison.OrdinalIgnoreCase))
            return Map(new Boolean(false), lastToken.Index - name.Length, name.Length);
        return DoVariable(name);
    }

    private IExpression DoVariable (string name)
    {
        return Map(new Variable(name), lastToken.Index - name.Length, lastToken.Content.Length);
    }

    private IExpression? TryString ()
    {
        if (token.Type == TokenType.String)
        {
            var content = Consume().Content;
            var value = content;
            if (value.StartsWith("\"", StringComparison.Ordinal)) value = value[1..];
            if (value.EndsWith("\"", StringComparison.Ordinal)) value = value[..^1];
            return Map(new String(value), lastToken.Index - content.Length, content.Length);
        }
        return TryNumeric();
    }

    private IExpression? TryNumeric ()
    {
        if (token.Type == TokenType.Number)
        {
            var content = Consume().Content;
            var number = double.Parse(content);
            return Map(new Numeric(number), lastToken.Index - content.Length, content.Length);
        }
        return TryClosure();
    }

    private IExpression? TryClosure ()
    {
        if (!Is("(")) return null; // Walked all supported morphemes, none found.

        Consume();
        var exp = TryExpression() ?? Err("Empty closure.");
        Expect(")");
        return exp;
    }

    private Token Consume ()
    {
        return lastToken = tokens.Pop();
    }

    private bool Expect (string content)
    {
        if (!Is(content))
        {
            Err($"Missing content: {content}");
            return false;
        }
        Consume();
        return true;
    }

    private Invalid Err (string message)
    {
        var index = assOffset + lastToken.Index;
        var length = text.Length - index + assOffset;
        options.HandleDiagnostic?.Invoke(new(index, length, message));
        anyError = true;
        return new(message);
    }

    private void Err (ParseDiagnostic diag)
    {
        var index = assOffset + diag.Index;
        var length = diag.Length - index + assOffset;
        options.HandleDiagnostic?.Invoke(new(index, length, diag.Message));
        anyError = true;
    }

    private bool Is (string content) => token.Content == content;
    private bool IsOp () => token.Type == TokenType.Operator;
    private bool IsEnd () => tokens.Count == 0;
}
