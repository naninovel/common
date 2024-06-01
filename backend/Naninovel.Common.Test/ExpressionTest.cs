namespace Naninovel.Expression.Test;

public class ExpressionTest
{
    private class UnsupportedExpression : IExpression;

    private readonly Stack<ParseDiagnostic> diagnostics = [];
    private readonly List<ExpressionRange> ranges = [];
    private readonly Parser parser;
    private readonly Evaluator evaluator;

    public ExpressionTest ()
    {
        parser = new Parser(new() {
            HandleDiagnostic = diagnostics.Push,
            HandleRange = ranges.Add
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
        InlineData("\"", 0, 1, "Unclosed string."),
        InlineData("~", 0, 1, "Unexpected character: ~"),
        InlineData("()", 0, 2, "Empty closure."),
        InlineData("(", 0, 1, "Empty closure."),
        InlineData("f(", 1, 1, "Missing content: )"),
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
        Assert.Contains(new ParseDiagnostic(idx, length, message), diagnostics);
    }

    [
        Theory,
        InlineData("num_2=1", "num_2", 1),
        InlineData("num_2++", "num_2", 3),
        InlineData("num_2--", "num_2", 1),
        InlineData("num_2+=2", "num_2", 4),
        InlineData("num_2-=2", "num_2", 0),
        InlineData("num_2*=3", "num_2", 6),
        InlineData("num_2/=2", "num_2", 1),
        InlineData("positive=negative", "positive", false),
        InlineData("\tpositive = negative == negative", "positive", true),
        InlineData("positive=negative=negative", "positive", true)
    ]
    public void EvaluatesAssignments (string text, string var, object result)
    {
        var asses = new List<Assignment>();
        Assert.True(parser.TryParseAssignments(text, asses));
        Assert.Equal(var, asses[0].Variable);
        Assert.Equal(result, evaluator.Evaluate(asses[0].Expression).GetValue(result.GetType()));
    }

    [Fact]
    public void CanAssignMultipleVariables ()
    {
        var asses = new List<Assignment>();
        Assert.True(parser.TryParseAssignments("foo=bar;bar=foo", asses));
        Assert.Equal(2, asses.Count);
        Assert.Equal("foo", asses[0].Variable);
        Assert.Equal("bar", asses[1].Variable);
        Assert.Equal("bar", evaluator.Evaluate(asses[0].Expression).GetValue<string>());
        Assert.Equal("foo", evaluator.Evaluate(asses[1].Expression).GetValue<string>());
    }

    [
        Theory,
        InlineData("=1", 0, 2, "Missing assigned variable name."),
        InlineData("x=", 0, 2, "Missing expression to assign."),
        InlineData("x=+", 2, 1, "Missing unary operand."),
        InlineData("foo=\"", 4, 1, "Unclosed string.")
    ]
    public void DiagnosesAssignmentErrors (string text, int idx, int length, string message)
    {
        var asses = new List<Assignment>();
        Assert.False(parser.TryParseAssignments(text, asses));
        Assert.Contains(new ParseDiagnostic(idx, length, message), diagnostics);
    }

    [Fact]
    public void ReturnsFalseWhenEmptyExpression ()
    {
        Assert.False(parser.TryParse("", out _));
        Assert.False(parser.TryParseAssignments("", []));
        Assert.False(parser.TryParseAssignments(";", []));
        Assert.Empty(diagnostics);
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

    [Fact]
    public void MapsRanges ()
    {
        Assert.True(parser.TryParse(""" foo("bar", 2, true, baz) """, out _));
        Assert.Contains(ranges, r => r.Expression is Function { Name: "foo" } && r.Index == 1 && r.Length == 24);
        Assert.Contains(ranges, r => r.Expression is String { Value: "bar" } && r.Index == 5 && r.Length == 5);
        Assert.Contains(ranges, r => r.Expression is Numeric { Value: 2 } && r.Index == 12 && r.Length == 1);
        Assert.Contains(ranges, r => r.Expression is Boolean { Value: true } && r.Index == 15 && r.Length == 4);
        Assert.Contains(ranges, r => r.Expression is Variable { Name: "baz" } && r.Index == 21 && r.Length == 3);
    }

    [Fact]
    public void MapsRangesInAssignments ()
    {
        Assert.True(parser.TryParseAssignments("""foo=foo();bar=bar("baz")""", []));
        Assert.Contains(ranges, r => r.Expression is Function { Name: "foo" } && r.Index == 4 && r.Length == 5);
        Assert.Contains(ranges, r => r.Expression is Function { Name: "bar" } && r.Index == 14 && r.Length == 10);
        Assert.Contains(ranges, r => r.Expression is String { Value: "baz" } && r.Index == 18 && r.Length == 5);
    }

    [Fact]
    public void MapsRangeOfIncompleteFunction ()
    {
        _ = parser.TryParse("foo(", out _);
        Assert.Contains(ranges, r => r.Expression is Function { Name: "foo" } && r.Index == 0 && r.Length == 4);
    }

    [Fact]
    public void MapsRangeOfIncompleteFunctionInAssignment ()
    {
        _ = parser.TryParseAssignments("foo=bar(", []);
        Assert.Contains(ranges, r => r.Expression is Function { Name: "bar" } && r.Index == 4 && r.Length == 4);
    }

    [Fact]
    public void MapsRangeOfFirstIncompleteFunctionParameter ()
    {
        _ = parser.TryParse("foo(\"", out _);
        Assert.Contains(ranges, r => r.Expression is Function { Parameters: [String { Value: "" }] } && r.Index == 0 && r.Length == 5);
        Assert.Contains(ranges, r => r.Expression is String { Value: "" } && r.Index == 4 && r.Length == 1);
    }

    [Fact]
    public void MapsRangeOfSecondIncompleteFunctionParameter ()
    {
        _ = parser.TryParse("""foo("x", "y""", out _);
        Assert.Contains(ranges, r => r.Expression is Function { Parameters: [String { Value: "x" }, String { Value: "y" }] } && r.Index == 0 && r.Length == 11);
        Assert.Contains(ranges, r => r.Expression is String { Value: "x" } && r.Index == 4 && r.Length == 3);
        Assert.Contains(ranges, r => r.Expression is String { Value: "y" } && r.Index == 9 && r.Length == 2);
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
