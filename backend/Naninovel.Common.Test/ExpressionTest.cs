namespace Naninovel.Expression.Test;

public class ExpressionTest
{
    private readonly Parser parser = new(new());
    private readonly Evaluator evaluator = new(new() {
        ResolveVariable = ResolveVariable,
        ResolveFunction = ResolveFunction
    });

    [
        Theory,
        InlineData("1", 1.0),
        InlineData("-1", -1.0),
        InlineData("+1", 1.0),
        InlineData("- 1.0", -1.0),
        InlineData("1+2", 1.0 + 2.0),
        InlineData("1+2+3", 1.0 + 2.0 + 3.0),
        InlineData("1+2*3", 1.0 + 2.0 * 3.0),
        InlineData("(1+2)*-3", (1.0 + 2.0) * -3.0),
        InlineData("(1+2)*(3+4)", (1.0 + 2.0) * (3.0 + 4.0)),
        InlineData("-(1+2) * -(3+4)", -(1.0 + 2.0) * -(3.0 + 4.0)),
        InlineData(" - ( 1 + 2 ) * - ( 3 + 4 ) ", -(1.0 + 2.0) * -(3.0 + 4.0)),
        InlineData("-((1+2) / (3+4))", -((1.0 + 2.0) / (3.0 + 4.0))),
        InlineData("-(-(1+2) * (3+4))", -(-(1.0 + 2.0) * (3.0 + 4.0))),
        InlineData("-(-(1+2) / -(3+4))", -(-(1.0 + 2.0) / -(3.0 + 4.0))),
        InlineData("1 /2-1+2*-(3-1.5)/ 2+-1", 1.0 / 2.0 - 1.0 + 2.0 * -(3.0 - 1.5) / 2.0 + -1.0),
        InlineData("num_10+num_2-2", 10.0),
    ]
    public void CanEvaluateNumericExpressions (string text, double expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.Equal(expected, evaluator.Evaluate<double>(expression));
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
    ]
    public void CanEvaluateBooleanExpressions (string text, bool expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.Equal(expected, evaluator.Evaluate<bool>(expression));
    }

    [
        Theory,
        InlineData("\"foo\"", "foo"),
        InlineData("foo", "foo"),
        InlineData("bar", "bar"),
        InlineData("true ? \"foo\" : \"bar\"", "foo"),
        InlineData("!negative?foo:bar", "foo"),
        InlineData("join( \",\", \"foo\" , bar )", "foo,bar"),
        InlineData("num_10 <= num_2 ? \"\" : join(\"!\", positive ? join(\":\",foo,bar,\"nya\") : join(\"\"), \"\")", "foo:bar:nya!")
    ]
    public void CanEvaluateStringExpressions (string text, string expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.Equal(expected, evaluator.Evaluate<string>(expression));
    }

    [
        Theory,
        InlineData("1", 1),
        InlineData("1", 1.0),
        InlineData("1.0", 1),
        InlineData("\"1\"", "1"),
        InlineData("true", true),
    ]
    public void CanEvaluateDynamicExpressions (string text, object expected)
    {
        Assert.True(parser.TryParse(text, out var expression));
        Assert.Equal(expected, evaluator.Evaluate(expression).GetValue(expected.GetType()));
    }

    #pragma warning disable CS8509 // Ignore missing default arm.
    private static IOperand ResolveVariable (string name) => name switch {
        "foo" => new String("foo"),
        "bar" => new String("bar"),
        "num_10" => new Numeric(10),
        "num_2" => new Numeric(2),
        "positive" => new Boolean(true),
        "negative" => new Boolean(false)
    };
    private static IOperand ResolveFunction (string name, IReadOnlyList<IOperand> args) => name switch {
        "join" => new String(string.Join(args[0].GetValue<string>(), args.Skip(1).Select(a => a.GetValue<string>())))
    };
    #pragma warning restore CS8509
}
