namespace Naninovel.Expression.Test;

public class ExpressionTest
{
    private class UnsupportedExpression : IExpression;

    private readonly Stack<ParseDiagnostic> diagnostics = [];
    private readonly Parser parser;
    private readonly Evaluator evaluator;

    public ExpressionTest ()
    {
        parser = new Parser(new() {
            HandleDiagnostic = diagnostics.Push
        });
        evaluator = new(new() {
            ResolveVariable = ResolveVariable,
            ResolveFunction = ResolveFunction
        });
    }

    [
        Theory,
        InlineData("1", 1.0),
        InlineData("-1", -1.0),
        InlineData("+1", 1.0),
        InlineData("- 1.0", -1.0),
        InlineData("1+2", 1.0 + 2.0),
        InlineData("5%2", 5.0 % 2.0),
        InlineData("2^3", 2 * 2 * 2),
        InlineData("1+2+3", 1.0 + 2.0 + 3.0),
        InlineData("1+2*3", 1.0 + 2.0 * 3.0),
        InlineData("(1+2)*-3", (1.0 + 2.0) * -3.0),
        InlineData("(1+2)*(3+4)", (1.0 + 2.0) * (3.0 + 4.0)),
        InlineData("-(1+2) * -(3+4)", -(1.0 + 2.0) * -(3.0 + 4.0)),
        InlineData(" - ( 1 + 2 ) * - ( 3 + 4 ) ", -(1.0 + 2.0) * -(3.0 + 4.0)),
        InlineData("-((1+2) / (3+4))", -((1.0 + 2.0) / (3.0 + 4.0))),
        InlineData("-(-(1+2) * (3+4))", -(-(1.0 + 2.0) * (3.0 + 4.0))),
        InlineData("-(-(1+2) / -(3+4))", -(-(1.0 + 2.0) / -(3.0 + 4.0))),
        InlineData("2 * (2*(2*(2+1)))", 2.0 * (2.0 * (2.0 * (2.0 + 1.0)))),
        InlineData("1 /2-1+2*-(3-1.5)/ 2+-1", 1.0 / 2.0 - 1.0 + 2.0 * -(3.0 - 1.5) / 2.0 + -1.0),
        InlineData("num_10+num_2-2", 10.0),
    ]
    public void CanEvaluateNumericExpressions (string text, double result)
    {
        Assert.True(parser.TryParse(text, out var exp));
        Assert.Equal(result, evaluator.Evaluate<double>(exp));
    }

    [
        Theory,
        InlineData("1 < 2", true),
        InlineData("1 > 2", false),
        InlineData("1 = 1", true),
        InlineData("1 = 2", false),
        InlineData("1 != 1", false),
        InlineData("1 != 2", true),
        InlineData("true", true),
        InlineData("false", false),
        InlineData("!true", false),
        InlineData("!false", true),
        InlineData("true = true", true),
        InlineData("true != true", false),
        InlineData("true = false", false),
        InlineData("true != false", true),
        InlineData("false = false", true),
        InlineData("false != false", false),
        InlineData("\"foo\" = \"foo\"", true),
        InlineData("\"foo\" == \"foo\"", true),
        InlineData("\"foo\" != \"foo\"", false),
        InlineData("\"foo\" = \"bar\"", false),
        InlineData("\"foo\" != \"bar\"", true),
        InlineData("1 <= 2", true),
        InlineData("1 >= 2", false),
        InlineData("1 >= 1", true),
        InlineData("1 <= 1", true),
        InlineData("1 < 2 | 1 > 2", true),
        InlineData("1 < 2 & 1 > 2", false),
        InlineData("1 < 2 || 1 > 2", true),
        InlineData("1 < 2 && 1 > 2", false),
        InlineData("!(1 > 2)", true),
        InlineData("(1+2) < (3+4)", true),
        InlineData("(1+2) > (3+4)", false),
        InlineData("(3+4) > (1+2)", true),
        InlineData("(3+4) < (1+2)", false),
        InlineData("(1+2) <= (3+4)", true),
        InlineData("(3+4) <= (1+2)", false),
        InlineData("(1+2) >= (3+4)", false),
        InlineData("(3+4) >= (1+2)", true),
        InlineData("(1+2) == (3+4)", false),
        InlineData("(1+2) == (2+1)", true),
        InlineData("(1+2) >= (2+1)", true),
        InlineData("(1+2) <= (2+1)", true),
        InlineData("positive", true),
        InlineData("negative", false),
        InlineData("!positive", false),
        InlineData("!negative", true),
        InlineData("positive & negative", false),
        InlineData("num_10 > 5 & num_2 < 5 && foo = \"foo\"", true),
        InlineData("num_10 >= 5 & num_2 <= 5 && foo = \"bar\" | bar != \"bar\" || foo == \"foo\"", true),
        InlineData("num_10 >= 5 & num_2 <= 5 && foo = \"bar\" | bar != \"bar\" || foo == \"bar\"", false),
    ]
    public void CanEvaluateBooleanExpressions (string text, bool result)
    {
        Assert.True(parser.TryParse(text, out var exp));
        Assert.Equal(result, evaluator.Evaluate<bool>(exp));
    }

    [
        Theory,
        InlineData("\"foo\"", "foo"),
        InlineData("\"foo\\\"bar\\\"\"", "foo\"bar\""),
        InlineData("foo", "foo"),
        InlineData("bar", "bar"),
        InlineData("foo+bar", "foobar"),
        InlineData("\"foo\"+ \"\\\"\" + \"bar\"", "foo\"bar"),
        InlineData("true ? \"foo\" : \"bar\"", "foo"),
        InlineData("false?\"\":(false?\"\":\"foo\")", "foo"),
        InlineData("!negative?foo:bar", "foo"),
        InlineData("join( \",\", \"foo\" , bar )", "foo,bar"),
        InlineData("num_10 <= num_2 ? \"\" : join(\"!\", positive ? join(\":\",foo,bar,\"nya\") : join(\"\"), \"\")", "foo:bar:nya!"),
        InlineData("фу", "фу"),
        InlineData("join(\" и \", эхо(фу), эхо(\"бар\"), \"ня\")", "фу и бар и ня"),
    ]
    public void CanEvaluateStringExpressions (string text, string result)
    {
        Assert.True(parser.TryParse(text, out var exp));
        Assert.Equal(result, evaluator.Evaluate<string>(exp));
    }

    [
        Theory,
        InlineData("1", 1),
        InlineData("1", 1.0),
        InlineData("1.0", 1),
        InlineData("\"1\"", "1"),
        InlineData("true", true),
    ]
    public void CanEvaluateDynamicExpressions (string text, object result)
    {
        Assert.True(parser.TryParse(text, out var exp));
        Assert.Equal(result, evaluator.Evaluate(exp).GetValue(result.GetType()));
    }

    [
        Theory,
        InlineData("true + false"),
        InlineData("+true"),
        InlineData("+\"\""),
        InlineData("-false"),
        InlineData("1&2"),
        InlineData("\"foo\"&\"bar\""),
        InlineData("\"foo\"&1"),
        InlineData("true/false"),
        InlineData("\"foo\"/\"bar\""),
        InlineData("1=true"),
        InlineData("\"foo\"==num_10"),
        InlineData("1>false"),
        InlineData("false>=\"bar\""),
        InlineData("\"foo\"<\"bar\""),
        InlineData("false<=true"),
        InlineData("1*false"),
        InlineData("!1"),
        InlineData("!\"foo\""),
        InlineData("!foo"),
        InlineData("-bar"),
        InlineData("-true"),
        InlineData("1!=false"),
        InlineData("true|2"),
        InlineData("false||\"foo\""),
        InlineData("true^0"),
        InlineData("false%true"),
        InlineData("1-false")
    ]
    public void ErrsWhenUnsupportedOperand (string text)
    {
        Assert.True(parser.TryParse(text, out var exp));
        Assert.Throws<Error>(() => evaluator.Evaluate(exp));
    }

    [
        Theory,
        InlineData("\"f", 1, 1, "Unclosed string."),
        InlineData("~", 0, 1, "Unexpected character: ~"),
        InlineData("f(", 1, 1, "Missing content: )"),
        InlineData("(", 0, 1, "Missing content: )"),
        InlineData("?1:2", 0, 4, "Missing ternary predicate."),
        InlineData("true?:2", 4, 3, "Missing truthy ternary branch."),
        InlineData("true?1:", 6, 1, "Missing falsy ternary branch."),
        InlineData("|true", 0, 5, "Missing left logical 'or' operand."),
        InlineData("true|", 4, 1, "Missing right logical 'or' operand."),
        InlineData("&true", 0, 5, "Missing left logical 'and' operand."),
        InlineData("true&", 4, 1, "Missing right logical 'and' operand."),
        InlineData("=1", 0, 2, "Missing left relational operand."),
        InlineData("1=", 1, 1, "Missing right relational operand."),
        InlineData("1+", 1, 1, "Missing right additive operand."),
        InlineData("*1", 0, 2, "Missing left multiplicative operand."),
        InlineData("1*", 1, 1, "Missing right multiplicative operand."),
        InlineData("!", 0, 1, "Missing unary operand."),
        InlineData("^1", 0, 2, "Missing left pow operand."),
        InlineData("1^", 1, 1, "Missing right pow operand."),
        InlineData("fn(,)", 2, 3, "Missing function parameter.")
    ]
    public void DiagnosesSyntaxErrors (string text, int idx, int length, string message)
    {
        Assert.False(parser.TryParse(text, out _));
        Assert.Equal(new ParseDiagnostic(idx, length, message), diagnostics.Pop());
    }

    [Fact]
    public void ErrsWhenUnknownExpression ()
    {
        Assert.Contains("Unknown expression",
            Assert.Throws<Error>(() => evaluator.Evaluate(new UnsupportedExpression())).Message);
    }

    [Fact]
    public void CanCastOperandValue ()
    {
        Assert.Equal("foo", new String("foo").GetValue<string>());
        Assert.Equal(1.0, new Numeric(1).GetValue<double>());
        Assert.Equal(0.01f, new Numeric(0.01).GetValue<float>());
        Assert.Equal(1, new Numeric(1.0).GetValue<int>());
        Assert.True(new Boolean(true).GetValue<bool>());
    }

    [Fact]
    public void ErrsWhenOperandValueCastFails ()
    {
        Assert.Throws<Error>(() => new String("foo").GetValue<double>());
    }

    #pragma warning disable CS8509 // Ignore missing default arm.
    private IOperand ResolveVariable (string name) => name switch {
        "foo" => new String("foo"),
        "bar" => new String("bar"),
        "фу" => new String("фу"),
        "num_10" => new Numeric(10),
        "num_2" => new Numeric(2),
        "positive" => new Boolean(true),
        "negative" => new Boolean(false)
    };
    private IOperand ResolveFunction (string name, IReadOnlyList<IOperand> args) => name switch {
        "join" => new String(string.Join(args[0].GetValue<string>(), args.Skip(1).Select(a => a.GetValue<string>()))),
        "эхо" => new String(args[0].GetValue<string>())
    };
    #pragma warning restore CS8509
}
