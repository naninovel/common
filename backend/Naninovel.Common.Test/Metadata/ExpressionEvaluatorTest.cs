using Naninovel.TestUtilities;
using static Naninovel.Metadata.ExpressionEvaluator;

namespace Naninovel.Metadata.Test;

public class ExpressionEvaluatorTest
{
    private readonly MetadataMock meta = new();
    private Func<string> getInspectedScript = () => "";
    private GetParamValue getParamValue = (id, idx) => null;

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
    public void CanResolveInspectedScript ()
    {
        getInspectedScript = () => "foo";
        Assert.Equal("foo", Evaluate($"{{{InspectedScript}}}"));
        getInspectedScript = () => null;
        Assert.Null(Evaluate($"{{{InspectedScript}}}"));
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
        getInspectedScript = () => "bar";
        Assert.Equal("foo/bar", Evaluate($"foo/{{{InspectedScript}}}"));
    }

    [Fact]
    public void CanInjectParamValues ()
    {
        getParamValue = (id, idx) => id == "1" ? "foo" : "bar";
        Assert.Equal("foo/bar", Evaluate("{:1}/{:2}"));
    }

    [Fact]
    public void WhenParamValueIsNullEmptyOrNullIsReturned ()
    {
        getParamValue = (id, idx) => null;
        Assert.Null(Evaluate("{:foo}"));
        Assert.Empty(EvaluateMany("{:foo}"));
    }

    [Fact]
    public void CanUseNullCoalescing ()
    {
        getInspectedScript = () => "bar";
        getParamValue = (id, idx) => id == "foo" ? "foo" : null;
        Assert.Equal("foobar", Evaluate($"{{:foo??:bar}}{{:bar??{InspectedScript}}}"));
    }

    [Fact]
    public void CanGetEnumerableParamValue ()
    {
        getInspectedScript = () => "nya";
        getParamValue = (id, idx) => idx == 0 ? "foo" : idx == 1 ? "bar" : null;
        Assert.Equal("foo/bar/nya", Evaluate($"{{:foo[0]}}/{{:bar[1]}}/{{:nya[2]??{InspectedScript}}}"));
    }

    [Fact]
    public void CanConcatenate ()
    {
        getInspectedScript = () => "nya";
        getParamValue = (id, idx) => idx == 0 ? "foo" : idx == 1 ? "bar" : null;
        Assert.Equal(["foo/bar/nya", "nya/foo", "foo"],
            EvaluateMany($"{{:foo[0]}}/{{:bar[1]}}/{{:nya[2]??{InspectedScript}}}+{{{InspectedScript}}}/foo+{{:foo[0]}}"));
    }

    [Fact]
    public void WhenIncorrectExpressionReturnsNullOrEmpty ()
    {
        Assert.Null(Evaluate("{:foo??bar}"));
        Assert.Empty(EvaluateMany("{:foo??bar}"));
    }

    private IReadOnlyList<string> EvaluateMany (string expression)
    {
        var results = new List<string>();
        var eval = new ExpressionEvaluator(meta, getInspectedScript, getParamValue);
        eval.Evaluate(expression, results);
        return results;
    }

    private string Evaluate (string expression)
    {
        var eval = new ExpressionEvaluator(meta, getInspectedScript, getParamValue);
        return eval.Evaluate(expression);
    }
}
