using Naninovel.Parsing;
using Naninovel.TestUtilities;
using static Naninovel.Metadata.ExpressionEvaluator;

namespace Naninovel.Metadata.Test;

public class ExpressionEvaluatorTest
{
    private readonly MetadataMock meta = new();

    [Fact]
    public void WhenNullOrEmptyReturnsNullOrEmpty ()
    {
        Assert.Null(Evaluate(null));
        Assert.Null(Evaluate(""));
        Assert.Empty(EvaluateMany(null));
        Assert.Empty(EvaluateMany(""));
    }

    [Fact]
    public void WhenDoesntContainExpressionsReturnsUnmodified ()
    {
        Assert.Equal("foo", Evaluate("foo"));
        Assert.Equal(["foo"], EvaluateMany("foo"));
    }

    [Fact]
    public void CanResolveEntryScript ()
    {
        meta.EntryScript = "foo";
        Assert.Equal("foo", Evaluate($"{{{EntryScript}}}"));
        meta.EntryScript = null;
        Assert.Null(Evaluate($"{{{EntryScript}}}"));
    }

    [Fact]
    public void CanResolveTitleScript ()
    {
        meta.TitleScript = "foo";
        Assert.Equal("foo", Evaluate($"{{{TitleScript}}}"));
        meta.TitleScript = null;
        Assert.Null(Evaluate($"{{{TitleScript}}}"));
    }

    [Fact]
    public void DoesntModifyContentOutsideOfExpression ()
    {
        meta.EntryScript = "bar";
        Assert.Equal("foo/bar", Evaluate($"foo/{{{EntryScript}}}"));
    }

    [Fact]
    public void CanResolveCommandParamValues ()
    {
        meta.Commands = [new() { Id = "c", Parameters = [new() { Id = "x" }, new() { Id = "y" }] }];
        var cmd = new Parsing.Command("c", [new("x", new([new PlainText("foo")])), new("y", new([new PlainText("bar")]))]);
        Assert.Equal("foo/bar", Evaluate("{:x}/{:y}", new() { Command = cmd }));
    }

    [Fact]
    public void WhenCommandParamValueIsEmptyEmptyOrNullIsReturned ()
    {
        meta.Commands = [new() { Id = "c", Parameters = [new() { Id = "x" }] }];
        var cmd = new Parsing.Command("c", [new("x", [])]);
        Assert.Null(Evaluate("{:x}", new() { Command = cmd }));
        Assert.Empty(EvaluateMany("{:x}", new() { Command = cmd }));
    }

    [Fact]
    public void WhenCommandParamIsMissingEmptyOrNullIsReturned ()
    {
        meta.Commands = [new() { Id = "c" }];
        var cmd = new Parsing.Command("c");
        Assert.Null(Evaluate("{:x}", new() { Command = cmd }));
        Assert.Empty(EvaluateMany("{:x}", new() { Command = cmd }));
    }

    [Fact]
    public void WhenCommandIsUnknownEmptyOrNullIsReturned ()
    {
        var cmd = new Parsing.Command("c", [new("x", new([new PlainText("foo")]))]);
        Assert.Null(Evaluate("{:x}", new() { Command = cmd }));
        Assert.Empty(EvaluateMany("{:x}", new() { Command = cmd }));
    }

    [Fact]
    public void CanResolveFunctionParamValues ()
    {
        meta.Functions = [new() { Name = "f", Parameters = [new() { Name = "x" }, new() { Name = "y" }] }];
        var fn = new Expression.Function("f", [new Expression.String("foo"), new Expression.String("bar")]);
        Assert.Equal("foo/bar", Evaluate("{:x}/{:y}", new() { Function = fn }));
    }

    [Fact]
    public void WhenFunctionParamValueIsNotStringEmptyOrNullIsReturned ()
    {
        meta.Functions = [new() { Name = "f", Parameters = [new() { Name = "x" }] }];
        var fn = new Expression.Function("f", [new Expression.Numeric(42)]);
        Assert.Null(Evaluate("{:x}", new() { Function = fn }));
        Assert.Empty(EvaluateMany("{:x}", new() { Function = fn }));
    }

    [Fact]
    public void WhenFunctionParamIsMissingEmptyOrNullIsReturned ()
    {
        meta.Functions = [new() { Name = "f", Parameters = [new() { Name = "x" }] }];
        var fn = new Expression.Function("f", []);
        Assert.Null(Evaluate("{:x}", new() { Function = fn }));
        Assert.Empty(EvaluateMany("{:x}", new() { Function = fn }));
    }

    [Fact]
    public void WhenFunctionIsUnknownEmptyOrNullIsReturned ()
    {
        var fn = new Expression.Function("f", [new Expression.String("foo")]);
        Assert.Null(Evaluate("{:x}", new() { Function = fn }));
        Assert.Empty(EvaluateMany("{:x}", new() { Function = fn }));
    }

    [Fact]
    public void WhenParamContextNotSpecifiedEmptyOrNullIsReturned ()
    {
        Assert.Null(Evaluate("{:x}"));
        Assert.Empty(EvaluateMany("{:x}"));
    }

    [Fact]
    public void CanUseNullCoalescing ()
    {
        meta.EntryScript = "bar";
        meta.Commands = [new() { Id = "c", Parameters = [new() { Id = "foo" }] }];
        var cmd = new Parsing.Command("c", [new("foo", new([new PlainText("foo")]))]);
        Assert.Equal("foobar", Evaluate($"{{:foo??:bar}}{{:bar??{EntryScript}}}", new() { Command = cmd }));
    }

    [Fact]
    public void CanGetEnumerableParamValue ()
    {
        meta.EntryScript = "nya";
        meta.Commands = [new() { Id = "c", Parameters = [new() { Id = "foo", ValueContainerType = ValueContainerType.Named }] }];
        var cmd = new Parsing.Command("c", [new("foo", new([new PlainText("foo.bar")]))]);
        Assert.Equal("foo/bar/nya", Evaluate($"{{:foo[0]}}/{{:foo[1]}}/{{:foo[2]??{EntryScript}}}", new() { Command = cmd }));
    }

    [Fact]
    public void CanConcatenate ()
    {
        meta.EntryScript = "nya";
        meta.Commands = [new() { Id = "c", Parameters = [new() { Id = "foo", ValueContainerType = ValueContainerType.Named }] }];
        var cmd = new Parsing.Command("c", [new("foo", new([new PlainText("foo.bar")]))]);
        Assert.Equal(["foo/bar/nya", "nya/foo", "foo"],
            EvaluateMany($"{{:foo[0]}}/{{:foo[1]}}/{{:foo[2]??{EntryScript}}}+{{{EntryScript}}}/foo+{{:foo[0]}}", new() { Command = cmd }));
    }

    [Fact]
    public void WhenIncorrectExpressionReturnsNullOrEmpty ()
    {
        Assert.Null(Evaluate("{:foo??bar}"));
        Assert.Empty(EvaluateMany("{:foo??bar}"));
    }

    private IReadOnlyList<string> EvaluateMany (string expression, Context ctx = default)
    {
        var results = new List<string>();
        var eval = new ExpressionEvaluator(meta);
        eval.Evaluate(expression, results, ctx);
        return results;
    }

    private string Evaluate (string expression, Context ctx = default)
    {
        var eval = new ExpressionEvaluator(meta);
        return eval.Evaluate(expression, ctx);
    }
}
